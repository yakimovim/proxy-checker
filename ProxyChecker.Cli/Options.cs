using CommandLine;

namespace ProxyChecker.Cli;

internal class Options
{
  [Option(shortName: 's', longName: "settings", Required = true, HelpText = "Path to file with settings")]
  public string SettingsFilePath { get; set; } = default!;
}
