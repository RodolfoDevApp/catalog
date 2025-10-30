namespace Catalog.Api.Contracts
{
    public sealed record ChangeProductPriceRequest(
        decimal NewPrice
    );
}
