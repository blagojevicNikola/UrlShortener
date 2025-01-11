using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace UrlShortener.Features.Url.Redirect;

public class RedirectEndpoint : Endpoint<RedirectQuery, Results<NotFound, RedirectHttpResult>>
{
    public override void Configure()
    {
        Get("/{@shortUrl}", x => new { x.ShortUrl });
        AllowAnonymous();
    }

    public override async Task<Results<NotFound, RedirectHttpResult>> ExecuteAsync(
        RedirectQuery req,
        CancellationToken ct
    )
    {
        var result = await req.ExecuteAsync(ct);

        if (result.IsFailure)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Redirect(result.Value!);
    }
}
