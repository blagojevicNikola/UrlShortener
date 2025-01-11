namespace UrlShortener.Common.Options;

public class KafkaOptions
{
    public const string Kafka = "Kafka";
    public required string Bootstrap { get; set; }
    public required ConsumerOptions Consumer { get; set; }
    public required ProducerOptions Producer { get; set; }
}

public class ConsumerOptions
{
    public required string Topic { get; set; }
    public required string Name { get; set; }
    public required string GroupId { get; set; }
    public int NumOfWorkers { get; set; }
    public int BufferSize { get; set; }
}

public class ProducerOptions
{
    public required string Name { get; set; }
    public required string Topic { get; set; }

}
