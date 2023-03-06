namespace ValueObjectBenchmark;

using System.Text.RegularExpressions;
using Utilities;

public sealed partial class OptimizedServer : IEquatable<OptimizedServer>
{
    // well-known values
    public static readonly OptimizedServer Production = new OptimizedServer("https://example.company.com");

    public static readonly OptimizedServer Test1 = new OptimizedServer("https://example-test1.company.com");

    private static readonly Dictionary<string, OptimizedServer> Cache =
        new Dictionary<string, OptimizedServer>
        {
            ["https://example.company.com"] = Production,
            ["https://example-test1.company.com"] = Test1,
        };

    private readonly string value;

    private OptimizedServer(string value)
    {
        this.value = value;
    }
    
    public static bool operator ==(OptimizedServer left, OptimizedServer right) => Equals(left, right);

    public static bool operator !=(OptimizedServer left, OptimizedServer right) => !Equals(left, right);

    public static OptimizedServer Parse(string value)
    {
        return TryParse(value, out var result) ? result : throw new InvalidOperationException();
    }

    public static bool TryParse(string value, out OptimizedServer result)
    {
        if (value is null)
        {
            result = default;
            return false;
        }

        var trimmed = value.Trim();
        if (Cache.TryGetValue(trimmed, out result))
        {
            return true;
        }

        var match = Format().Match(trimmed);
        if (match.Success)
        {
            result = new OptimizedServer(trimmed);
            return true;
        }

        result = default;
        return false;
    }

    internal static OptimizedServer DangerousParseWithoutFormatValidation(string value)
    {
        Guard.NotNull(value);

        return Cache.TryGetValue(value, out var result) ? result : new OptimizedServer(value);
    }

    public bool Equals(OptimizedServer other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        // Domain names are case insensitive
        return string.Equals(value, other.value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object obj) => obj is OptimizedServer other && Equals(other);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(value);

    public override string ToString() => value;

    [GeneratedRegex(@"^https://example(?:-test\d)\.company\.com$", RegexOptions.Compiled)]
    private static partial Regex Format();
}