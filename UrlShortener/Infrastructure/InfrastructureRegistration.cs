using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using UrlShortener.Common.Extensions;
using UrlShortener.Common.Options;
using UrlShortener.Entities;
using UrlShortener.Features.Analytics.ProcessAccess;
using UrlShortener.Infrastructure.Broker;
using UrlShortener.Infrastructure.Encoder;
using UrlShortener.Infrastructure.Interceptors;
using UrlShortener.Infrastructure.Manager;
using UrlShortener.Infrastructure.Telemetry;

namespace UrlShortener.Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection RegisterInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        var serviceName = config.GetValue<string>("ServiceName");

        if (serviceName == null)
        {
            throw new ArgumentNullException(nameof(serviceName), "Service name is not defined!");
        }

        AddDbContext(services, config.GetNpgslConnectionString());

        services.AddSingleton<ICounterManager, CounterManager>();
        services.AddSingleton<IEncoder, Base62Encoder>();
        services.AddSingleton<IProducerService, ProducerService>();
        services.AddSingleton<ILogHandler>(e => new BrokerLogAdapter(Log.Logger));

        AddKafka(services, config.GetSection(KafkaOptions.Kafka).Get<KafkaOptions>());

        AddOpenTelemetry(services, serviceName);

        return services;
    }

    public static ILoggingBuilder ConfigureInfrastructureLogging(this ILoggingBuilder builder)
    {
        builder.AddOpenTelemetry(logging => logging.AddOtlpExporter());

        return builder;
    }

    #region Private methods

    private static void AddDbContext(IServiceCollection services, string connectionString)
    {
        services.AddSingleton<MetadataInterceptor>();
        services.AddDbContext<UrlShortenerContext>(
            (sp, options) =>
                options
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention()
                    .AddInterceptors(sp.GetRequiredService<MetadataInterceptor>())
                    .UseSeeding((context, _) =>
                    {
                        if (SeedData(context))
                        {
                            context.SaveChanges();
                        }
                    }).UseAsyncSeeding(async (context, _, ct) =>
                    {
                        if (SeedData(context))
                        {
                            await context.SaveChangesAsync(ct);
                        }
                    })
        );
    }

    private static void AddKafka(IServiceCollection services, KafkaOptions? kafkaOptions)
    {
        ArgumentNullException.ThrowIfNull(kafkaOptions, nameof(kafkaOptions));

        services.AddKafka(e => e
        .UseLogHandler<BrokerLogAdapter>()
        .AddCluster(
            cluster => cluster.WithBrokers([kafkaOptions.Bootstrap])
            .CreateTopicIfNotExists(kafkaOptions!.Producer.Topic, 1, 1)
            .AddProducer<ProducerService>(
                        producer => producer
                            .DefaultTopic(kafkaOptions!.Producer.Topic)
                            .AddMiddlewares(m => m.AddSerializer<NewtonsoftJsonSerializer>()))
            .AddConsumer(
                        consumer => consumer
                            .Topic(kafkaOptions!.Consumer.Topic)
                            .WithGroupId(kafkaOptions.Consumer.GroupId)
                            .WithName(kafkaOptions.Consumer.Name)
                            .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                            .WithBufferSize(kafkaOptions!.Consumer.BufferSize)
                            .WithWorkersCount(kafkaOptions.Consumer.NumOfWorkers)
                            .AddMiddlewares(
                                m => m
                                    .AddDeserializer<NewtonsoftJsonDeserializer>()
                                    .AddTypedHandlers(handlers => handlers.AddHandler<ProcessAccessEventHandler>())
                            )
                    )
        ));
    }

    private static void AddOpenTelemetry(IServiceCollection services, string serviceName)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                metrics.AddMeter(UrlAccessDiagnostics.MetricName);

                metrics.AddOtlpExporter();
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSource(ProcessAccessInstrumentation.ActivitySourceName);

                tracing.AddOtlpExporter();
            });
        services.AddSingleton<IActivitySourceInstrumentation, ProcessAccessInstrumentation>();
        services.AddSingleton<ICounterDiagnostics, UrlAccessDiagnostics>();
    }

    private static bool SeedData(DbContext context)
    {
        var counter = context.Set<Counter>().FirstOrDefault(e => !e.Invalidated);

        if (counter == null)
        {
            context.Set<Counter>().Add(new Counter()
            {
                Id = Guid.Parse("e829e214-31bb-495a-8204-ead625f29272"),
                MaxValue = 300_000_00,
                CurrentStartingValue = 0,
                IncrementValue = 1_000_000,
            });
            return true;
        }

        return false;
    }
    #endregion
}
