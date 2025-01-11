using FastEndpoints;
using FluentValidation;
using UrlShortener.Common.Results;

namespace UrlShortener.Features.Url.Shorten;

public record ShortenUrlCommand : ICommand<Result<ShortenUrlResponse>>
{
    public string Schema { get; set; } = "https";

    public string Address { get; set; } = null!;
}

public class ShortenUrlRequestValidator : Validator<ShortenUrlCommand>
{
    public ShortenUrlRequestValidator()
    {
        RuleFor(e => e.Schema)
            .NotEmpty()
            .WithMessage("Schema cannot be empty!");

        RuleFor(e => e.Address)
            .NotEmpty()
            .WithMessage("Address cannot be empty!");
    }
}
