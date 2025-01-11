namespace UrlShortener.Infrastructure.Encoder;

public interface IEncoder
{
    string Encode(long value);
}
