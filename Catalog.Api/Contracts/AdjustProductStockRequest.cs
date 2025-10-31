namespace Catalog.Api.Contracts
{
    // Sin "Reason" para alinear con el dominio
    public sealed class AdjustProductStockRequest
    {
        public int Delta { get; set; }
    }
}
