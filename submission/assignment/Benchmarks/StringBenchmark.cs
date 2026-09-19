using System.Text;
using BenchmarkDotNet.Attributes;

namespace AcademyScheduleAnalyzer;

[MemoryDiagnoser]
[ShortRunJob]
public class StringBenchmark
{
    [Params(100, 1000, 10000, 100000)]
    public int Iterations;

    private const string Text = "A";

    [Benchmark(Baseline = true)]
    public string StringConcatenation()
    {
        string result = "";
        for (int i = 0; i < Iterations; i++)
            result += Text;
        return result;
    }

    [Benchmark]
    public string StringBuilderConcatenation()
    {
        var result = new StringBuilder();
        for (int i = 0; i < Iterations; i++)
            result.Append(Text);
        return result.ToString();
    }
}
