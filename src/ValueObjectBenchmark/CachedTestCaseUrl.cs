namespace ValueObjectBenchmark;

using System.Text.RegularExpressions;
using Flurl;
using Microsoft.Extensions.Caching.Memory;
using Utilities;

public sealed partial class CachedTestCaseUrl : IEquatable<CachedTestCaseUrl>
{
    private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions
    {
        SizeLimit = 100_000,
        TrackStatistics = false,
        TrackLinkedCacheEntries = false,
    });
    
    private string cachedValue;
    
    public CachedTestCaseUrl(OptimizedServer server, OptimizedTenant tenantId, OptimizedResourceUuid projectId, OptimizedResourceUuid resourceId)
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

    private CachedTestCaseUrl(OptimizedServer server, OptimizedTenant tenantId, OptimizedResourceUuid projectId, OptimizedResourceUuid resourceId, string cachedValue)
    {
        Server = server;
        TenantId = tenantId;
        ProjectId = projectId;
        ResourceId = resourceId;
        this.cachedValue = cachedValue;
    }

    public OptimizedServer Server { get; }

    public OptimizedTenant TenantId { get; }

    public OptimizedResourceUuid ProjectId { get; }

    public OptimizedResourceUuid ResourceId { get; }

    public static bool operator ==(CachedTestCaseUrl left, CachedTestCaseUrl right) => Equals(left, right);

    public static bool operator !=(CachedTestCaseUrl left, CachedTestCaseUrl right) => !Equals(left, right);

    public static CachedTestCaseUrl Parse(string value)
    {
        return TryParse(value, out var result)
            ? result
            : throw new InvalidOperationException();
    }

    public static bool TryParse(string value, out CachedTestCaseUrl result)
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
            result = new CachedTestCaseUrl(
                OptimizedServer.DangerousParseWithoutFormatValidation(match.Groups[1].Value),
                OptimizedTenant.DangerousParseWithoutFormatValidation(match.Groups[2].Value),
                OptimizedResourceUuid.DangerousParseWithoutFormatValidation(match.Groups[3].Value),
                OptimizedResourceUuid.DangerousParseWithoutFormatValidation(match.Groups[4].Value),
                match.Groups[0].Value);

            using var entry = Cache.CreateEntry(trimmed);
            entry.Size = 1;
            entry.Value = result;
            return true;
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

    public bool Equals(CachedTestCaseUrl other)
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

    public override bool Equals(object obj) => obj is CachedTestCaseUrl other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Server, TenantId, ProjectId, ResourceId);
    
    public override string ToString() => cachedValue ??= BaseUrl().AppendPathSegment(ResourceId);      

    [GeneratedRegex(
        @"^(https://example(?:-test\d)?\.company\.com)/(qm(?:[1-9]|fed))/oslc_qm/contexts/([\w-]{23})/resources/com\.ibm\.rqm\.planning\.VersionedTestCase/([\w-]{23})$",
        RegexOptions.Compiled)]
    private static partial Regex Format();
}
