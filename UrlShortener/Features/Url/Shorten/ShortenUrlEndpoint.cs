using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace UrlShortener.Features.Url.Shorten;

public class ShortenUrlEndpoint : Endpoint<ShortenUrlCommand, Ok<ShortenUrlResponse>>
{
    public override void Configure()
    {
        Post("/shorten-url");
        AllowAnonymous();
    }

    public async override Task<Ok<ShortenUrlResponse>> ExecuteAsync(ShortenUrlCommand req, CancellationToken ct)
    {
        var result = await req.ExecuteAsync(ct);

        if (result.IsSuccess)
        {
            return TypedResults.Ok(result.Value);
        }

        throw new InvalidOperationException();
    }
}
