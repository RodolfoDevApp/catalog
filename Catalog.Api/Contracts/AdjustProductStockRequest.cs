namespace Catalog.Api.Contracts
{
    public sealed record AdjustProductStockRequest(
        int DeltaQuantity
    );
}
