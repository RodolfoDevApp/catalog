namespace Catalog.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with key '{key}' was not found.")
    {
        ResourceName = resourceName;
        Key = key;
    }

    public string ResourceName { get; }
    public object Key { get; }
}
