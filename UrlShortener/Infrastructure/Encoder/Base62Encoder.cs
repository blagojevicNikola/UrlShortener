using UrlShortener.Common.Exceptions;

namespace UrlShortener.Infrastructure.Encoder;

public class Base62Encoder(IConfiguration config) : IEncoder
{
    private const int _base = 62;
    public string Encode(long value)
    {
        var charSet =
            config.GetSection("Encodings").GetValue<string>("Base62")?.ToCharArray()
            ?? throw new EncodingException();

        string result = string.Empty;

        while (value > 0)
        {
            long reminder = value % _base;
            value /= _base;
            try
            {
                result += charSet[reminder];
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new EncodingException(ex);
            }
        }

        return result.PadLeft(7, '0');
    }
}
