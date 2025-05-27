using BenchmarkDotNet.Running;
using Microsoft.REPR.Performance;

//var reprBenchmark = new REPRBenchmark();
//reprBenchmark.AddREPR();

BenchmarkRunner.Run<REPRBenchmark>();