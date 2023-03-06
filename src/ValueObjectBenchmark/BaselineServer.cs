namespace ValueObjectBenchmark;

using System.Text.RegularExpressions;

public sealed class BaselineServer : IEquatable<BaselineServer>
{
    private readonly string value;

    private BaselineServer(string value)
    {
        this.value = value;
    }
    
    public static bool operator ==(BaselineServer left, BaselineServer right) => Equals(left, right);

    public static bool operator !=(BaselineServer left, BaselineServer right) => !Equals(left, right);

    public static bool TryParse(string value, out BaselineServer result)
    {
        if (value is null)
        {
            result = default;
            return false;
        }

        var match = Regex.Match(value.Trim(), @"^https://example(?:-test\d)?\.company\.com$");
        if (match.Success)
        {
            result = new BaselineServer(match.Groups[0].Value);
            return true;
        }

        result = default;
        return false;
    }

    public bool Equals(BaselineServer other)
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

    public override bool Equals(object obj) => obj is BaselineServer other && Equals(other);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(value);

    public override string ToString() => value;
}
