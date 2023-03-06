namespace ValueObjectBenchmark;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;

[Config(typeof(Config))]
[MemoryDiagnoser]
[HideColumns(Column.RatioSD)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class TestCaseUrlBenchmark
{
    private const string TestCase = "https://example.company.com/qm1/oslc_qm/contexts/_aogSbv654xehyFRvc476sb/resources/com.ibm.rqm.planning.VersionedTestCase/_ghy5svqKIgVB34dc299Xcf";
    
    private class Config : ManualConfig
    {
        public Config()
        {
            SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage);
        }
    }

    private BaselineTestCaseUrl baseline;
    private OptimizedTestCaseUrl optimized;
    private CachedTestCaseUrl cached;

    [GlobalSetup]
    public void Setup()
    {
        baseline = BaselineTestCaseUrl.Parse(TestCase);
        optimized = OptimizedTestCaseUrl.Parse(TestCase);
        cached = CachedTestCaseUrl.Parse(TestCase);
    }
    
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("parse")]
    public BaselineTestCaseUrl Parse()
    {
        return BaselineTestCaseUrl.Parse(TestCase);
    }

    [Benchmark]
    [BenchmarkCategory("parse")]
    public OptimizedTestCaseUrl ParseOptimized()
    {
        return OptimizedTestCaseUrl.Parse(TestCase);
    }
    
    [Benchmark]
    [BenchmarkCategory("parse")]
    public CachedTestCaseUrl ParseCached()
    {
        return CachedTestCaseUrl.Parse(TestCase);
    }
    
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("toString")]
    public string ConvertToString()
    {
        return baseline.ToString();
    }
    
    [Benchmark]
    [BenchmarkCategory("toString")]
    public string ConvertToStringOptimized()
    {
        return optimized.ToString();
    }
    
    [Benchmark]
    [BenchmarkCategory("toString")]
    public string ConvertToStringCached()
    {
        return cached.ToString();
    }
}