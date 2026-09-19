# Benchmark: string vs StringBuilder

## Measurement method

Measured locally on 19 September 2026 using BenchmarkDotNet 0.15.8 in Release mode. The table below is the actual generated output from this machine, not example numbers.

The recorded run used ShortRun: one launch, three warmups, and three measured iterations. The workload remains in `Benchmarks/StringBenchmark.cs`; benchmark execution is not part of the application menu.

Both methods in `Benchmarks/StringBenchmark.cs` append the same constant string `"A"` exactly `Iterations` times and return the completed string. The parameter values are 100, 1,000, 10,000, and 100,000. Neither measured loop performs console output or handwritten timing. `[MemoryDiagnoser]` records managed allocations.

This is a repeated-append workload, not a benchmark of date formatting or console rendering. A one-character fragment makes the intentionally allocation-heavy 100,000-iteration concatenation test practical while retaining equivalent work.

## Machine and complete results

```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2)
AMD Ryzen 7 7800X3D 4.20GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 10.0.401
  [Host]   : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4
  ShortRun : .NET 10.0.12 (10.0.12, 10.0.1226.42308), X64 RyuJIT x86-64-v4

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  

```
| Method                     | Iterations | Mean             | Error             | StdDev          | Ratio | RatioSD | Gen0         | Gen1         | Gen2         | Allocated     | Alloc Ratio |
|--------------------------- |----------- |-----------------:|------------------:|----------------:|------:|--------:|-------------:|-------------:|-------------:|--------------:|------------:|
| **StringConcatenation**        | **100**        |         **738.1 ns** |         **219.22 ns** |        **12.02 ns** |  **1.00** |    **0.02** |       **0.2499** |            **-** |            **-** |       **12576 B** |        **1.00** |
| StringBuilderConcatenation | 100        |         107.0 ns |          24.80 ns |         1.36 ns |  0.14 |    0.00 |       0.0153 |            - |            - |         768 B |        0.06 |
|                            |            |                  |                   |                 |       |         |              |              |              |               |             |
| **StringConcatenation**        | **1000**       |      **31,976.7 ns** |       **2,358.16 ns** |       **129.26 ns** |  **1.00** |    **0.00** |      **20.4468** |       **0.0610** |            **-** |     **1025976 B** |       **1.000** |
| StringBuilderConcatenation | 1000       |         766.3 ns |         297.09 ns |        16.28 ns |  0.02 |    0.00 |       0.0906 |            - |            - |        4576 B |       0.004 |
|                            |            |                  |                   |                 |       |         |              |              |              |               |             |
| **StringConcatenation**        | **10000**      |   **2,411,624.1 ns** |     **466,194.73 ns** |    **25,553.71 ns** | **1.000** |    **0.01** |    **1992.1875** |      **93.7500** |            **-** |   **100259976 B** |       **1.000** |
| StringBuilderConcatenation | 10000      |       7,236.6 ns |       2,122.29 ns |       116.33 ns | 0.003 |    0.00 |       1.0529 |       0.1144 |            - |       53200 B |       0.001 |
|                            |            |                  |                   |                 |       |         |              |              |              |               |             |
| **StringConcatenation**        | **100000**     | **448,412,966.7 ns** | **150,548,997.96 ns** | **8,252,098.62 ns** | **1.000** |    **0.02** | **2581000.0000** | **2552000.0000** | **2546000.0000** | **10003454424 B** |       **1.000** |
| StringBuilderConcatenation | 100000     |     144,373.1 ns |      70,037.20 ns |     3,838.98 ns | 0.000 |    0.00 |      62.2559 |      62.2559 |      62.2559 |      410013 B |       0.000 |

## إجابات أسئلة التحليل

1. **أي الطريقتين كانت أسرع عند 100 تكرار؟**

   كانت `StringBuilder` أسرع في هذه التجربة؛ بلغ متوسط زمنها `107.0 ns`، مقابل `738.1 ns` لدمج النصوص باستخدام `string`. وبمقارنة المتوسطين، كانت أسرع بنحو 6.9 مرات.

2. **أي الطريقتين كانت أسرع عند 100,000 تكرار؟**

   كانت `StringBuilder` أسرع أيضًا؛ بلغ متوسط زمنها `144,373.1 ns`، أي نحو `0.144 ms`، مقابل `448,412,966.7 ns`، أي نحو `448.413 ms` لطريقة `string`. الفرق بين المتوسطين يعادل نحو 3,106 مرات في هذه التجربة، وليس نسبة ثابتة تنطبق على كل البرامج.

3. **أي الطريقتين خصصت ذاكرة أكثر؟**

   طريقة `string` خصصت ذاكرة أكثر في جميع الأحجام التي جرى قياسها. عند 100 تكرار خصصت `12,576 bytes` مقابل `768 bytes` لطريقة `StringBuilder`. وعند 100,000 تكرار خصصت `10,003,454,424 bytes` مقابل `410,013 bytes`.

   هذه الأرقام تمثل مجموع تخصيصات الذاكرة المُدارة خلال استدعاء كامل للدالة، بما فيها الكائنات المؤقتة. لا تمثل حجم النص النهائي أو أقصى ذاكرة مستخدمة في لحظة واحدة؛ فالنص النهائي يحتوي على 100,000 حرف في الطريقتين.

4. **ماذا حدث لأداء دمج النصوص مع زيادة عدد التكرارات؟**

   ازداد زمن طريقة `string` بوضوح: من نحو `0.738` ميكروثانية عند 100 تكرار، إلى `31.977` ميكروثانية عند 1,000، ثم `2.412` مللي ثانية عند 10,000، وصولًا إلى `448.413` مللي ثانية عند 100,000 تكرار.

   نمت تخصيصات الذاكرة بصورة تقارب النمو التربيعي، لأن كل عملية دمج تعيد نسخ النص المتراكم. كما أظهرت الحالة الأكبر نشاطًا ملحوظًا لجامع القمامة في الأجيال الأعلى. يتأثر الزمن الفعلي بأحجام التخصيص وحدود تشغيل جامع القمامة، لذلك لا يلزم أن يتبع نسبة تربيعية دقيقة.

5. **لماذا يؤدي تكرار دمج النصوص إلى تخصيصات إضافية؟**

   لأن `string` غير قابلة للتعديل بعد إنشائها. عند تنفيذ `result += text`، يُنشأ عادةً نص جديد يحتوي على النص السابق والجزء المضاف. وتصبح النصوص السابقة قابلة للتنظيف بواسطة جامع القمامة عندما لا يعود هناك مرجع إليها.

   عند إضافة أجزاء ثابتة الحجم داخل حلقة، تُنسخ نصوص يتزايد طولها مع كل تكرار. مجموع هذا النسخ والتخصيص ينمو تقريبًا بمعدل تربيعي مع عدد التكرارات. بعض الحالات الخاصة، مثل الدمج مع نص فارغ، لا تغيّر هذا الاتجاه العام.

6. **لماذا يكون StringBuilder أفضل عادةً عند إضافة النصوص بشكل متكرر؟**

   لأنه يبني النص داخل مخازن قابلة للتعديل، ويضيف أجزاء جديدة دون إنشاء نسخة بديلة من النص المتراكم بالكامل عند كل إضافة. لذلك تقل عمليات النسخ وتخصيص الذاكرة مقارنة بالدمج المتكرر باستخدام `string`.

   لا يعني ذلك أنه لا يخصص ذاكرة؛ فتوسعة المخازن تحتاج إلى تخصيصات، واستدعاء `ToString()` يُنشئ النص النهائي. لكن نمو التخصيصات في هذه التجربة كان أبطأ بكثير من طريقة `string`.

7. **هل StringBuilder أفضل دائمًا من عمليات string العادية؟ وضّح.**

   لا. عند تكوين رسالة قصيرة من بضعة أجزاء ثابتة، قد يكون استخدام الدمج العادي أو إدراج القيم داخل النص باستخدام `string interpolation` أوضح وأقل تكلفة من إنشاء `StringBuilder` ومخازنه.

   تظهر فائدته خصوصًا عند بناء نص طويل من إضافات متكررة داخل حلقة. والنتائج هنا تخص إضافة الحرف نفسه على هذا الجهاز وإصدار .NET المستخدم، ولا تثبت أن `StringBuilder` أسرع في جميع عمليات النصوص. الاختيار يعتمد على طبيعة الاستخدام والقياس عند الحاجة.

## Reading the columns and limits

- Mean is average time per complete benchmark method invocation, not per individual append.
- Error is half of the 99.9% confidence interval; StdDev measures variability among measurements.
- Allocated is managed memory allocated per invocation, including temporary objects; it is not memory retained after collection.
- A displayed ratio of 0.000 is rounding, not zero execution time or zero allocation.
- ShortRun used one launch, three warmups and three measurement iterations. With only three samples, confidence intervals are wide (see Error), so the speedup figures are approximate. They support the broad difference here, not precise universal multipliers. Removing `[ShortRunJob]` from the benchmark class enables BenchmarkDotNet's longer default measurement policy.

## Raw evidence and references

- [Generated Markdown results](Benchmarks/Results/AcademyScheduleAnalyzer.StringBenchmark-report-github.md)
- [Generated CSV results](Benchmarks/Results/AcademyScheduleAnalyzer.StringBenchmark-report.csv)
- [Generated HTML results](Benchmarks/Results/AcademyScheduleAnalyzer.StringBenchmark-report.html)
- [BenchmarkDotNet getting started](https://benchmarkdotnet.org/articles/guides/getting-started.html): Release builds, benchmark methods, and result export.
- [BenchmarkDotNet jobs](https://benchmarkdotnet.org/articles/configs/jobs.html): iteration and warmup configuration.
- [BenchmarkDotNet diagnosers](https://benchmarkdotnet.org/articles/configs/diagnosers.html): MemoryDiagnoser.

