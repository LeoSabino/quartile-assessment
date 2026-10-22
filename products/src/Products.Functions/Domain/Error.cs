namespace Products.Functions.Domain;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict
}

public sealed record Error(ErrorType Type, string Code, string Message);
