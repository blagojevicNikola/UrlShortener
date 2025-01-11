using FastEndpoints;
using FluentValidation;
using UrlShortener.Common.Results;

namespace UrlShortener.Features.Url.Redirect;

public class RedirectQuery : ICommand<Result<string>>
{
    public string ShortUrl { get; set; } = null!;
}

public class RedirectPairQueryValidator : AbstractValidator<RedirectQuery>
{
    public RedirectPairQueryValidator()
    {
        RuleFor(e => e.ShortUrl)
            .NotEmpty()
            .Length(7)
            .Matches(".*(\\.||\\(||\\)||,||).*");
    }
}