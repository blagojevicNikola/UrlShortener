using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using UrlShortener.Features.Analytics.ProcessAccess;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Broker;
using UrlShortener.Infrastructure.Interceptors;

namespace UrlShortener.IntegrationTests;

public class IntegrationTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithHostname("localhost")
        .WithUsername("test")
        .WithPassword("postgres")
        .WithDatabase("test")
        .WithEnvironment(new Dictionary<string, string>()
        {
            {"POSTGRES_HOST_AUTH_METHOD", "trust" }
        })
        .Build();

    private readonly IContainer _kafkaContainer = new ContainerBuilder()
        .WithImage("confluentinc/cp-kafka:7.8.0")
        .WithName("kafka-kraft")
        .WithHostname("kafka-kraft")
        .WithPortBinding(9092, 9092)
        .WithEnvironment(
            new Dictionary<string, string>
            {
                { "KAFKA_NODE_ID", "1" },
                {
                    "KAFKA_LISTENER_SECURITY_PROTOCOL_MAP",
                    "CONTROLLER:PLAINTEXT,PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT"
                },
                {
                    "KAFKA_ADVERTISED_LISTENERS",
                    "PLAINTEXT://kafka-kraft:29092,PLAINTEXT_HOST://localhost:9092"
                },
                { "KAFKA_JMX_PORT", "9101" },
                { "KAFKA_JMX_HOSTNAME", "localhost" },
                { "KAFKA_PROCESS_ROLES", "broker,controller" },
                { "KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1" },
                { "KAFKA_CONTROLLER_QUORUM_VOTERS", "1@kafka-kraft:29093" },
                {
                    "KAFKA_LISTENERS",
                    "PLAINTEXT://kafka-kraft:29092,CONTROLLER://kafka-kraft:29093,PLAINTEXT_HOST://0.0.0.0:9092"
                },
                { "KAFKA_INTER_BROKER_LISTENER_NAME", "PLAINTEXT" },
                { "KAFKA_CONTROLLER_LISTENER_NAMES", "CONTROLLER" },
                { "CLUSTER_ID", "MkU3OEVBNTcwNTJENDM2Qk" },
            }
        )
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<UrlShortenerContext>));

            services.AddDbContext<UrlShortenerContext>(
                (sp, options) =>
                    options
                        .UseNpgsql(_dbContainer.GetConnectionString())
                        .UseSnakeCaseNamingConvention()
                        .AddInterceptors(sp.GetRequiredService<MetadataInterceptor>())
            );

            var kafkaDescriptor = services.SingleOrDefault(s =>
                s.ServiceType == typeof(KafkaFlowConfigurator)
            );

            if (kafkaDescriptor is not null)
            {
                services.Remove(kafkaDescriptor);
            }

            var kafkaPort = _kafkaContainer.GetMappedPublicPort(9092);
            Console.WriteLine(kafkaPort);
            services.AddKafka(e =>
                e.AddCluster(cluster =>
                    cluster
                        .WithBrokers([$"localhost:9092"])
                        .CreateTopicIfNotExists("url-access-topic", 1, 1)
                        .AddProducer<ProducerService>(producer =>
                            producer
                                .DefaultTopic("url-access-topic")
                                .AddMiddlewares(m => m.AddSerializer<NewtonsoftJsonSerializer>())
                        )
                        .AddConsumer(consumer =>
                            consumer
                                .Topic("url-access-topic")
                                .WithGroupId("url-access-event-group")
                                .WithName("url-access-event-consumer")
                                .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                                .WithBufferSize(1)
                                .WithWorkersCount(1)
                                .AddMiddlewares(m =>
                                    m.AddDeserializer<NewtonsoftJsonDeserializer>()
                                        .AddTypedHandlers(handlers =>
                                            handlers.AddHandler<ProcessAccessEventHandler>()
                                        )
                                )
                        )
                )
            );
        });
    }

    public async Task InitializeAsync()
    {
        await _kafkaContainer.StartAsync();
        await _dbContainer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _kafkaContainer.StopAsync();
        await _dbContainer.StopAsync();
    }
}

[CollectionDefinition("WebApplication Collection")]
public class WebApplicationCollection : ICollectionFixture<IntegrationTestWebFactory>
{

}
