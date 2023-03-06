namespace ValueObjectBenchmark;

using System.Text.RegularExpressions;
using Utilities;

public sealed partial class OptimizedResourceUuid : IEquatable<OptimizedResourceUuid>
{
    private readonly string value;

    private OptimizedResourceUuid(string value)
    {
        this.value = value;
    }

    public static bool operator ==(OptimizedResourceUuid left, OptimizedResourceUuid right) => Equals(left, right);

    public static bool operator !=(OptimizedResourceUuid left, OptimizedResourceUuid right) => !Equals(left, right);

    public static bool TryParse(string value, out OptimizedResourceUuid result)
    {
        if (value is null)
        {
            result = default;
            return false;
        }

        var trimmed = value.Trim();
        if (trimmed.Length is 23)
        {
            var match = Format().Match(trimmed);
            if (match.Success)
            {
                result = new OptimizedResourceUuid(trimmed);
                return true;
            }
        }

        result = default;
        return false;
    }

    internal static OptimizedResourceUuid DangerousParseWithoutFormatValidation(string value)
    {
        Guard.NotNull(value);

        return new OptimizedResourceUuid(value);
    }

    public bool Equals(OptimizedResourceUuid other)
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

    public override bool Equals(object obj) => obj is OptimizedResourceUuid other && Equals(other);

    public override int GetHashCode() => value.GetHashCode();

    public override string ToString() => value;

    [GeneratedRegex(@"^[\w-]+$", RegexOptions.Compiled)]
    private static partial Regex Format();
}