using UrlShortener.Common.Results;

namespace UrlShortener.Common.Errors;

public static class CommonErrors
{
    public static Error EntityNotFound => new("EntityNotFound", "Entity was not found!");

    public static Error EntityAlreadyExists => new("EntityAlreadyExists", "Entity already exists!");

    public static Error UrlCouldNotBeCreated => new("Url.CouldNotBeCreated", "Url could not be created");
}
