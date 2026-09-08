namespace MyRecipe.Application.Enums
{
    public enum ErrorType
    {
        Error,
        Forbidden,
        Unauthorized,
        Invalid,
        NotFound,
        Conflict,
        Unavailable,
        MultiStatus,
        RequestTimeout,
        ClientClosedRequest,
        UnprocessableEntity,
        BadGateway
    }
}
