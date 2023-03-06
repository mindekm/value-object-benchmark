namespace ValueObjectBenchmark;

using System.Text.RegularExpressions;
using Utilities;

public sealed partial class OptimizedTenant : IEquatable<OptimizedTenant>
{
    // well-known values
    public static readonly OptimizedTenant Segment1 = new OptimizedTenant("qm1");

    public static readonly OptimizedTenant Federated = new OptimizedTenant("qmfed");

    private static readonly Dictionary<string, OptimizedTenant> Cache =
        new Dictionary<string, OptimizedTenant>
        {
            ["qm1"] = Segment1,
            ["qmfed"] = Federated,
        };

    private readonly string value;

    private OptimizedTenant(string value)
    {
        this.value = value;
    }

    public static bool operator ==(OptimizedTenant left, OptimizedTenant right) => Equals(left, right);

    public static bool operator !=(OptimizedTenant left, OptimizedTenant right) => !Equals(left, right);

    public static bool TryParse(string value, out OptimizedTenant result)
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
            result = new OptimizedTenant(trimmed);
            return true;
        }

        result = default;
        return false;
    }

    internal static OptimizedTenant DangerousParseWithoutFormatValidation(string value)
    {
        Guard.NotNull(value);

        return Cache.TryGetValue(value, out var result) ? result : new OptimizedTenant(value);
    }

    public bool Equals(OptimizedTenant other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(value, other.value, StringComparison.Ordinal);
    }

    public override bool Equals(object obj) => obj is OptimizedTenant other && Equals(other);

    public override int GetHashCode() => value.GetHashCode();

    public override string ToString() => value;

    [GeneratedRegex(@"^qm(?:[1-9]|fed)$", RegexOptions.Compiled)]
    private static partial Regex Format();
}