using BenchmarkDotNet.Running;
using REPR.Performance;

//var reprBenchmark = new REPRBenchmark();
//reprBenchmark.AddREPR();

BenchmarkRunner.Run<REPRBenchmark>();