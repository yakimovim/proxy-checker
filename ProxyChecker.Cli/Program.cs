using CommandLine;

namespace ProxyChecker.Cli;

internal class Program
{
  static async Task<int> Main(string[] args)
  {
    var parser = Parser.Default;

    var parsingResult = parser.ParseArguments<Options>(args);

    var exitCode = await parsingResult.MapResult(
      PipelineExecutor.ExecutePipeline, 
      errors => Task.FromResult(1)
    );

    return exitCode;
  }
}
