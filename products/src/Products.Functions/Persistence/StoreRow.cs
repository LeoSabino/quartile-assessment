namespace Products.Functions.Persistence;

internal class StoreRow
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public bool IsActive { get; private set; }
}

internal class CompanyRow
{
    public Guid Id { get; private set; }
    public bool IsActive { get; private set; }
}
