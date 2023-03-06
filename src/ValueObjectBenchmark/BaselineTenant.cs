namespace ValueObjectBenchmark;

using System.Text.RegularExpressions;

public sealed class BaselineTenant : IEquatable<BaselineTenant>
{
    private readonly string value;

    private BaselineTenant(string value)
    {
        this.value = value;
    }

    public static bool operator ==(BaselineTenant left, BaselineTenant right) => Equals(left, right);

    public static bool operator !=(BaselineTenant left, BaselineTenant right) => !Equals(left, right);

    public static bool TryParse(string value, out BaselineTenant result)
    {
        if (value is null)
        {
            result = default;
            return false;
        }

        var match = Regex.Match(value.Trim(), @"^qm(?:[1-9]|fed)$");
        if (match.Success)
        {
            result = new BaselineTenant(match.Groups[0].Value);
            return true;
        }

        result = default;
        return false;
    }

    public bool Equals(BaselineTenant other)
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

    public override bool Equals(object obj) => obj is BaselineTenant other && Equals(other);

    public override int GetHashCode() => value.GetHashCode();

    public override string ToString() => value;
}
