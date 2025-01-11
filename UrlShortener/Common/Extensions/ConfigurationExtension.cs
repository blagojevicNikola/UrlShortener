namespace UrlShortener.Common.Extensions;

public static class ConfigurationExtension
{
    public static string GetNpgslConnectionString(this IConfiguration config)
    {
        var section = config.GetSection("Npgsql");
        string host = section.GetValue<string>("Host") ?? "";
        string port = section.GetValue<string>("Port") ?? "";
        string database = section.GetValue<string>("Database") ?? "";
        string username = section.GetValue<string>("Username") ?? "";
        string password = section.GetValue<string>("Password") ?? "Password";

        return string.Format(
            "Host={0}; Port={1}; Database={2}; Username={3}; Password={4};",
            host,
            port,
            database,
            username,
            password
        );
    }

    //public static ProducerConfig GetKafkaProducerConfig(this IConfiguration config)
    //{
    //    return new ProducerConfig()
    //    {
    //        BootstrapServers = config.GetSection("Kafka").GetValue<string>("Bootstrap"),
    //        AllowAutoCreateTopics = true,
    //    };
    //}

    //public static ConsumerConfig GetKafkaConsumerConfig(this IConfiguration config)
    //{
    //    return new ConsumerConfig()
    //    {
    //        BootstrapServers = config.GetSection("Kafka").GetValue<string>("Bootstrap"),
    //        AutoOffsetReset = AutoOffsetReset.Earliest,
    //        AllowAutoCreateTopics = true,
    //        GroupId = "my-consumer-group"
    //    };
    //}
}
