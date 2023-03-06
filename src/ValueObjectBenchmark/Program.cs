using BenchmarkDotNet.Running;

AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromSeconds(1));
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);