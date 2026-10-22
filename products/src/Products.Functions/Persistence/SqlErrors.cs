using Microsoft.Data.SqlClient;
using Products.Functions.Domain;

namespace Products.Functions.Persistence;

internal static class SqlErrors
{
    private const int StoreNotFound = 50001;
    private const int UniqueIndexViolation = 2601;
    private const int UniqueConstraintViolation = 2627;
    private const int ConstraintViolation = 547;

    public static Error? ToError(SqlException exception) => exception.Number switch
    {
        StoreNotFound => ProductErrors.StoreNotFound,
        UniqueIndexViolation or UniqueConstraintViolation => ProductErrors.ProductCodeAlreadyInUse,
        ConstraintViolation => ProductErrors.RejectedByDatabase,
        _ => null
    };
}
