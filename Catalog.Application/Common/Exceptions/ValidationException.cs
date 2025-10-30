using System.Collections.Generic;
using System.Linq;

namespace Catalog.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    // helper conveniente
    public ValidationException(string property, string error)
        : this(new Dictionary<string, string[]>
        {
            { property, new[] { error } }
        })
    {
    }

    public override string ToString()
    {
        // útil para logs
        var joined = string.Join("; ",
            Errors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}"));
        return $"{Message} => {joined}";
    }
}
