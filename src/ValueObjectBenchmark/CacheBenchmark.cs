using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;

namespace ValueObjectBenchmark;

[Config(typeof(Config))]
[MemoryDiagnoser]
[HideColumns(Column.RatioSD)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class CacheBenchmark
{
    private class Config : ManualConfig
    {
        public Config()
        {
            SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Percentage);
        }
    }

    public class Wrapper
    {
        private readonly string value;

        public Wrapper(string value)
        {
            this.value = value;
        }
    }

    private Dictionary<string, Wrapper> cache;

    private Wrapper production;
    private Wrapper test2;
    private Wrapper test4;
    private Wrapper test6;
    

    [GlobalSetup]
    public void Setup()
    {
        cache = new Dictionary<string, Wrapper>(StringComparer.OrdinalIgnoreCase)
        {
            ["https://example.company.com"] = new Wrapper("https://example.company.com"),
            ["https://example-test2.company.com"] = new Wrapper("https://example-test2.company.com"),
            ["https://example-test4.company.com"] = new Wrapper("https://example-test4.company.com"),
            ["https://example-test6.company.com"] = new Wrapper("https://example-test6.company.com"),
        };

        production = new Wrapper("https://example.company.com");
        test2 = new Wrapper("https://example-test2.company.com");
        test4 = new Wrapper("https://example-test4.company.com");
        test6 = new Wrapper("https://example-test6.company.com");
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("hit")]
    public Wrapper DictionaryHit()
    {
        return Dictionary("https://example.company.com");
    }

    [Benchmark]
    [BenchmarkCategory("hit")]
    public Wrapper ConditionHitBestCase()
    {
        return Condition("https://example.company.com");
    }
    
    [Benchmark]
    [BenchmarkCategory("hit")]
    public Wrapper ConditionHitWorstCase()
    {
        return Condition("https://example-test6.company.com");
    }
    
    [Benchmark]
    [BenchmarkCategory("hit")]
    public Wrapper SwitchHitBestCase()
    {
        return SwitchExpression("https://example.company.com");
    }
    
    [Benchmark]
    [BenchmarkCategory("hit")]
    public Wrapper SwitchHitWorstCase()
    {
        return SwitchExpression("https://example-test6.company.com");
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("miss")]
    public Wrapper DictionaryMiss()
    {
        return Dictionary("https://example-test8.company.com");
    }

    [Benchmark]
    [BenchmarkCategory("miss")]
    public Wrapper ConditionMiss()
    {
        return Condition("https://example-test8.company.com");
    }
    
    [Benchmark]
    [BenchmarkCategory("miss")]
    public Wrapper SwitchMiss()
    {
        return Condition("https://example-test8.company.com");
    }

    private Wrapper Dictionary(string value)
    {
        if (cache.TryGetValue(value, out var result))
        {
            return result;
        }

        return new Wrapper(value);
    }

    private Wrapper Condition(string value)
    {
        if (string.Equals(value, "https://example.company.com", StringComparison.OrdinalIgnoreCase))
        {
            return production;
        }
        
        if (string.Equals(value, "https://example-test2.company.com", StringComparison.OrdinalIgnoreCase))
        {
            return test2;
        }
        
        if (string.Equals(value, "https://example-test4.company.com", StringComparison.OrdinalIgnoreCase))
        {
            return test4;
        }
        
        if (string.Equals(value, "https://example-test6.company.com", StringComparison.OrdinalIgnoreCase))
        {
            return test6;
        }

        return new Wrapper(value);
    }

    private Wrapper SwitchExpression(string value)
    {
        var lower = value.ToLowerInvariant();
        return lower switch
        {
            "https://example.company.com" => production,
            "https://example-test2.company.com" => test2,
            "https://example-test4.company.com" => test4,
            "https://example-test6.company.com" => test6,
            _ => new Wrapper(value),
        };
    }
}