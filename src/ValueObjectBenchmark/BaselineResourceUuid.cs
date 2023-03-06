namespace ValueObjectBenchmark;

using System.Text.RegularExpressions;

public sealed class BaselineResourceUuid : IEquatable<BaselineResourceUuid>
{
    private readonly string value;

    private BaselineResourceUuid(string value)
    {
        this.value = value;
    }

    public static bool operator ==(BaselineResourceUuid left, BaselineResourceUuid right) => Equals(left, right);

    public static bool operator !=(BaselineResourceUuid left, BaselineResourceUuid right) => !Equals(left, right);

    public static bool TryParse(string value, out BaselineResourceUuid result)
    {
        if (value is null)
        {
            result = default;
            return false;
        }

        var match = Regex.Match(value.Trim(), @"^[\w-]{23}$");
        if (match.Success)
        {
            result = new BaselineResourceUuid(match.Groups[0].Value);
            return true;
        }

        result = default;
        return false;
    }

    public bool Equals(BaselineResourceUuid other)
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

    public override bool Equals(object obj) => obj is BaselineResourceUuid other && Equals(other);

    public override int GetHashCode() => value.GetHashCode();

    public override string ToString() => value;
}
