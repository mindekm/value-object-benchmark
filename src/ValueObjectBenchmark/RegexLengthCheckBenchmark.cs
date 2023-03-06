using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;

namespace ValueObjectBenchmark;

[Config(typeof(Config))]
[MemoryDiagnoser]
[HideColumns(Column.RatioSD)]
public partial class RegexLengthCheckBenchmark
{
    private class Config : ManualConfig
    {
        public Config()
        {
            SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage);
        }
    }
 
    [Benchmark(Baseline = true)]
    [Arguments("_ghy5svqKIgVB34dc299Xcf")]
    [Arguments("_ghy5svqKIgVB34dc299Xcfc")]
    [Arguments("_ghy5svqKIgVB34dc299Xcfcv")]
    public bool SeparateCheck(string uuid)
    {
        if (uuid.Length is 23 or 25)
        {
            var match = WithoutLength().Match(uuid);
            return match.Success;
        }

        return false;
    }

    [Benchmark]
    [Arguments("_ghy5svqKIgVB34dc299Xcf")]
    [Arguments("_ghy5svqKIgVB34dc299Xcfc")]
    [Arguments("_ghy5svqKIgVB34dc299Xcfcv")]
    public bool RegexCheck(string uuid)
    {
        var match = WithLength().Match(uuid);
        return match.Success;
    }

    [GeneratedRegex(@"^(?:[\w-]{23}|[\w-]{25})$", RegexOptions.Compiled)]
    private static partial Regex WithLength();
    
    [GeneratedRegex(@"^[\w-]+$", RegexOptions.Compiled)]
    private static partial Regex WithoutLength();
}