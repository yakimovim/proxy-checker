using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
  /// Support plugins are available for:
  ///   - JetBrains ReSharper        https://nuke.build/resharper
  ///   - JetBrains Rider            https://nuke.build/rider
  ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
  ///   - Microsoft VSCode           https://nuke.build/vscode
  ///   

  private readonly AbsolutePath OutputDirectory = RootDirectory / "output";

  public static int Main() => Execute<Build>(x => x.Publish);

  [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
  readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

  [Parameter]
  readonly string Solution;

  [Parameter]
  readonly DotNetVerbosity DotNetVerbosity = DotNetVerbosity.quiet;

  Target Clean => _ => _
      .Description("Clean output directory")
      .Executes(() =>
      {
        (RootDirectory).GlobDirectories("ProxyChecker*/**/bin", "ProxyChecker*/**/obj").ForEach(d =>
        {
          d.DeleteDirectory();
        });

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
          .SetVerbosity(DotNetVerbosity)
      );
    });
}
