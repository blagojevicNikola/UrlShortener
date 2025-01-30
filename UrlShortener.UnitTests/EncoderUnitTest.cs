using FluentAssertions;
using Microsoft.Extensions.Configuration;
using UrlShortener.Infrastructure.Encoder;

namespace UrlShortener.UnitTests;

public class EncoderUnitTest
{
    private IEncoder _encoder;

    public EncoderUnitTest()
    {
        var base62EncCharacters =
            "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>()
                {
                    {
                        "Encodings:Base62",
                        base62EncCharacters
                    },
                }
            )
            .Build();

        _encoder = new Base62Encoder(config);
    }

    [Fact]
    public void EncodingOutOfRangeValue_Should_ThrowAnException()
    {
        var e = Assert.Throws<ArgumentOutOfRangeException>(() => _encoder.Encode(-1));
    }

    [Fact]
    public void EncodingValidValue_Should_ReturnValidNonEmptyString()
    {
        var encodedValue = _encoder.Encode(45345);

        encodedValue.Should().NotBeNullOrEmpty();
        encodedValue.Should().HaveLength(7);
    }

    [Fact]
    public void EncodingZero_Should_EqualExactValue()
    {
        var encodedValue = _encoder.Encode(0);

        encodedValue.Should().BeEquivalentTo("0000000");
    }
}
