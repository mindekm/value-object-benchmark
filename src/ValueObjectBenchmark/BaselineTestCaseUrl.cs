namespace ValueObjectBenchmark;

using System.Text.RegularExpressions;
using Flurl;
using Utilities;

public sealed class BaselineTestCaseUrl : IEquatable<BaselineTestCaseUrl>
{
    public BaselineTestCaseUrl(BaselineServer server, BaselineTenant tenantId, BaselineResourceUuid projectId, BaselineResourceUuid resourceId)
    {
        Guard.NotNull(server);
        Guard.NotNull(tenantId);
        Guard.NotNull(projectId);
        Guard.NotNull(resourceId);
        
        Server = server;
        TenantId = tenantId;
        ProjectId = projectId;
        ResourceId = resourceId;
    }

    public BaselineServer Server { get; }

    public BaselineTenant TenantId { get; }

    public BaselineResourceUuid ProjectId { get; }

    public BaselineResourceUuid ResourceId { get; }

    public static bool operator ==(BaselineTestCaseUrl left, BaselineTestCaseUrl right) => Equals(left, right);

    public static bool operator !=(BaselineTestCaseUrl left, BaselineTestCaseUrl right) => !Equals(left, right);

    public static BaselineTestCaseUrl Parse(string value)
    {
        return TryParse(value, out var result)
            ? result
            : throw new InvalidOperationException();
    }

    public static bool TryParse(string value, out BaselineTestCaseUrl result)
    {
        if (value is null)
        {
            result = default;
            return false;
        }

        var match = Regex.Match(
            value.Trim(),
            @"^(https://.+)/(\w+)/oslc_qm/contexts/([\w-]+)/resources/com\.ibm\.rqm\.planning\.VersionedTestCase/([\w-]+)$");
        if (match.Success)
        {
            var serverResult = BaselineServer.TryParse(match.Groups[1].Value, out var server);
            var tenantResult = BaselineTenant.TryParse(match.Groups[2].Value, out var tenant);
            var projectResult = BaselineResourceUuid.TryParse(match.Groups[3].Value, out var project);
            var idResult = BaselineResourceUuid.TryParse(match.Groups[4].Value, out var id);

            if (serverResult && tenantResult && projectResult && idResult)
            {
                result = new BaselineTestCaseUrl(server, tenant, project, id);
                return true;
            }
        }

        result = default;
        return false;
    }
    
    public Url BaseUrl()
    {
        return new Url(Server.ToString())
            .AppendPathSegment(TenantId.ToString())
            .AppendPathSegment("oslc_qm")
            .AppendPathSegment("contexts")
            .AppendPathSegment(ProjectId.ToString())
            .AppendPathSegment("resources")
            .AppendPathSegment("com.ibm.rqm.planning.VersionedTestCase");
    }

    public bool Equals(BaselineTestCaseUrl other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Server.Equals(other.Server) &&
               TenantId.Equals(other.TenantId) &&
               ProjectId.Equals(other.ProjectId) &&
               ResourceId.Equals(other.ResourceId);
    }

    public override bool Equals(object obj) => obj is BaselineTestCaseUrl other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Server, TenantId, ProjectId, ResourceId);

    public override string ToString() => BaseUrl().AppendPathSegment(ResourceId);
}
