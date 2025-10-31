namespace Catalog.Api.Contracts
{
    public sealed class SimpleOkResponse
    {
        public bool Success { get; init; } = true;
        public int Status { get; init; } = 200;
        public string Message { get; init; } = "OK";
        public object? Data { get; init; }
        public object? Meta { get; init; }
        public string? TraceId { get; init; }
        public string Service { get; init; } = "catalog-api";
        public System.DateTime TimestampUtc { get; init; } = System.DateTime.UtcNow;
    }
}
