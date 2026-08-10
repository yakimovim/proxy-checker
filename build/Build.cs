using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;

class Build : NukeBuild
{
  /// Support plugins are available for:
  ///   - JetBrains ReSharper        https://nuke.build/resharper
  ///   - JetBrains Rider            https://nuke.build/rider
  ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
  ///   - Microsoft VSCode           https://nuke.build/vscode
  ///   

  private readonly AbsolutePath OutputDirectory = RootDirectory / "output";

  public static int Main() => Execute<Build>(x => x.Compose);

  [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
  //readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
  readonly Configuration Configuration;

  [Parameter]
  readonly string Solution;

  [Parameter]
  readonly DotNetVerbosity DotNetVerbosity = DotNetVerbosity.quiet;

  [Parameter]
  readonly string Runtime;

  private void Cleanup()
  {
    (RootDirectory).GlobDirectories("ProxyChecker*/**/bin", "ProxyChecker*/**/obj").ForEach(d =>
    {
      d.DeleteDirectory();
    });
  }

  Target Clean => _ => _
      .Description("Clean output directory")
      .Executes(() =>
      {
        Cleanup();

        (OutputDirectory).CreateOrCleanDirectory();
      });

  Target Restore => _ => _
      .Description("Restore dependencies")
      .DependsOn(Clean)
      .Executes(() =>
      {
        DotNetTasks.DotNetRestore(
          new DotNetRestoreSettings()
            .SetProjectFile(RootDirectory / Solution)
            .SetVerbosity(DotNetVerbosity)
        );
      });

  Target Compile => _ => _
      .Description("Compile project")
      .DependsOn(Restore)
      .Executes(() =>
      {
        DotNetTasks.DotNetBuild(
          new DotNetBuildSettings()
            .SetConfiguration(Configuration)
            .SetNoRestore(true)
            .SetProjectFile(RootDirectory / Solution)
            .SetVerbosity(DotNetVerbosity)
        );
      });

  Target Publish => _ => _
    .Description("Publish project")
    .DependsOn(Compile)
    .Executes(() => {
      DotNetTasks.DotNetPublish(
        new DotNetPublishSettings()
          .SetConfiguration(Configuration)
          .SetProject(RootDirectory / Solution)
          .SetSelfContained(true)
          .SetRuntime(Runtime)
          .SetVerbosity(DotNetVerbosity)
      );
    });

  Target Compose => _ => _
    .Description("Compose resulting project folder")
    .DependsOn(Publish)
    .Executes(() => {
      (RootDirectory).GlobFiles("ProxyChecker/**/publish/*.*").ForEach(f => {
        f.Copy(OutputDirectory / f.Name, ExistsPolicy.FileOverwrite);
      });
      (RootDirectory).GlobFiles("ProxyChecker.Cli/**/publish/*.*").ForEach(f => {
        f.Copy(OutputDirectory / f.Name, ExistsPolicy.FileOverwrite);
      });

      // Loaders
      (RootDirectory).GlobFiles("ProxyChecker.Loaders.UriTextFile/**/publish/ProxyChecker.Loaders.UriTextFile.*").ForEach(f => {
        f.Copy(OutputDirectory / "Plugins/Loaders/UriTextFile" / f.Name, ExistsPolicy.FileOverwrite);
      });
      (RootDirectory).GlobFiles("ProxyChecker.Loaders.FlashProxyApi/**/publish/ProxyChecker.Loaders.FlashProxyApi.*").ForEach(f => {
        f.Copy(OutputDirectory / "Plugins/Loaders/FlashProxyApi" / f.Name, ExistsPolicy.FileOverwrite);
      });
      (RootDirectory).GlobFiles("ProxyChecker.Loaders.GeonodeApi/**/publish/ProxyChecker.Loaders.GeonodeApi.*").ForEach(f => {
        f.Copy(OutputDirectory / "Plugins/Loaders/GeonodeApi" / f.Name, ExistsPolicy.FileOverwrite);
      });
      (RootDirectory).GlobFiles("ProxyChecker.Loaders.GithubIpLocate/**/publish/ProxyChecker.Loaders.GithubIpLocate.*").ForEach(f => {
        f.Copy(OutputDirectory / "Plugins/Loaders/GithubIpLocate" / f.Name, ExistsPolicy.FileOverwrite);
      });

      // Checkers
      (RootDirectory).GlobFiles("ProxyChecker.Checkers.Anonymity/**/publish/ProxyChecker.Checkers.Anonymity.*").ForEach(f => {
        f.Copy(OutputDirectory / "Plugins/Checkers/Anonymity" / f.Name, ExistsPolicy.FileOverwrite);
      });
      (RootDirectory).GlobFiles("ProxyChecker.Checkers.OkResponse/**/publish/ProxyChecker.Checkers.OkResponse.*").ForEach(f => {
        f.Copy(OutputDirectory / "Plugins/Checkers/OkResponse" / f.Name, ExistsPolicy.FileOverwrite);
      });

      // Exporters
      (RootDirectory).GlobFiles("ProxyChecker.Exporters.UriTextFile/**/publish/ProxyChecker.Exporters.UriTextFile.*").ForEach(f => {
        f.Copy(OutputDirectory / "Plugins/Exporters/UriTextFile" / f.Name, ExistsPolicy.FileOverwrite);
      });

      // Cleanup
      Cleanup();
    });

}
