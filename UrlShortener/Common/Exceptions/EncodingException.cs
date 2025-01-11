namespace UrlShortener.Common.Exceptions;

public class EncodingException : Exception
{
    public EncodingException() : base("Encoding error!") { }

    public EncodingException(Exception innerException) : base("Encoding error!", innerException) { }
}
