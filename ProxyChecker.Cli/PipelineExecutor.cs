using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ProxyChecker.Cli.Services;
using ProxyChecker.Common.Configuration;
using ProxyChecker.Common.Logging;
using ProxyChecker.Common.Models;
using ProxyChecker.Common.Services;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Checkers;
using ProxyChecker.Interfaces.Exporters;
using ProxyChecker.Interfaces.Loaders;
using Spectre.Console;

namespace ProxyChecker.Cli;

internal static class PipelineExecutor
{
  public static async Task<int> ExecutePipeline(Options options, CancellationToken cancellationToken)
  {
    var servicesProvider = GetServicesProvider();

    var pipelineSettings = await GetPipelineSettings(options, cancellationToken);

    if (pipelineSettings == null)
    {
      return 1;
    }

    var loaderCreator = servicesProvider.GetServices<ILoaderCreator>()
      .SingleOrDefault(l => l.Uid == pipelineSettings.LoaderCreatorUid);

    if (loaderCreator is null)
    {
      AnsiConsole.MarkupLine($"[red]{Resource.LoaderCreatorNotFoundMessage}[/]");
      return 1;
    }

    var loader = loaderCreator.Create();

    loader.SetSettings(pipelineSettings.LoaderSettings);

    var loaderValidationResult = loader.ValidateSettingsForCli();

    if (!loaderValidationResult.IsValid)
    {
      AnsiConsole.MarkupLine($"[red]{Resource.LoaderInvalidSettingsMessage}[/]");
      
      AnsiConsole.WriteLine();

      foreach (var error in loaderValidationResult.Errors)
      {
        AnsiConsole.MarkupLine($"[red]- {error.ErrorMessage}[/]");
      }

      return 1;
    }

    var checkerCreator = servicesProvider.GetServices<ICheckerCreator>()
      .SingleOrDefault(c => c.Uid == pipelineSettings.CheckerCreatorUid);

    if (checkerCreator is null)
    {
      AnsiConsole.MarkupLine($"[red]{Resource.CheckerCreatorNotFoundMessage}[/]");
      return 1;
    }

    var checker = checkerCreator.Create();

    checker.SetSettings(pipelineSettings.CheckerSettings);

    var checkerValidationResult = checker.ValidateSettingsForCli();

    if (!checkerValidationResult.IsValid)
    {
      AnsiConsole.MarkupLine($"[red]{Resource.CheckerInvalidSettingsMessage}[/]");

      AnsiConsole.WriteLine();

      foreach (var error in checkerValidationResult.Errors)
      {
        AnsiConsole.MarkupLine($"[red]- {error.ErrorMessage}[/]");
      }

      return 1;
    }

    var exporterCreator = servicesProvider.GetServices<IExporterCreator>()
      .SingleOrDefault(e => e.Uid == pipelineSettings.ExporterCreatorUid);

    if (exporterCreator is null)
    {
      AnsiConsole.MarkupLine($"[red]{Resource.ExporterCreatorNotFoundMessage}[/]");
      return 1;
    }

    var exporter = exporterCreator.Create();

    exporter.SetSettings(pipelineSettings.ExporterSettings);

    var exporterValidationResult = exporter.ValidateSettingsForCli();

    if (!exporterValidationResult.IsValid)
    {
      AnsiConsole.MarkupLine($"[red]{Resource.ExporterInvalidSettingsMessage}[/]");

      AnsiConsole.WriteLine();

      foreach (var error in exporterValidationResult.Errors)
      {
        AnsiConsole.MarkupLine($"[red]- {error.ErrorMessage}[/]");
      }

      return 1;
    }

    var proxies = await loader.LoadAsync(cancellationToken).ToArrayAsync(cancellationToken);

    if (!(await checker.IsReadyAsync(cancellationToken)))
    {
      AnsiConsole.MarkupLine($"[red]{Resource.CheckerNotReadyMessage}[/]");
      return 1;
    }

    var validProxies = await GetValidProxiesAsync(checker, proxies, cancellationToken);

    await exporter.ExportAsync(validProxies, cancellationToken);

    return 0;
  }

  private static async Task<IEnumerable<Proxy>> GetValidProxiesAsync(
    IChecker checker, 
    IEnumerable<Proxy> proxies,
    CancellationToken cancellationToken)
  {
    var validProxies = new ConcurrentQueue<Proxy>();

    if (checker.SupportsParallelChecking)
    {
      await Parallel.ForEachAsync(
        proxies,
        cancellationToken,
        async (proxy, ct) =>
        {
          if (await checker.CheckAsync(proxy, ct))
          {
            validProxies.Enqueue(proxy);
          }
        }
      );
    }
    else
    {
      foreach (var proxy in proxies)
      {
        if (cancellationToken.IsCancellationRequested)
        {
          break;
        }

        if (await checker.CheckAsync(proxy, cancellationToken))
        {
          validProxies.Enqueue(proxy);
        }
      }
    }

    return validProxies;
  }

  private static async Task<PipelineModel?> GetPipelineSettings(Options options, CancellationToken cancellationToken)
  {
    if (!File.Exists(options.SettingsFilePath))
    {
      AnsiConsole.MarkupLine($"[red]File '{options.SettingsFilePath}' is not found.[/]");
      return null;
    }

    var settingsFileContent = await File.ReadAllTextAsync(options.SettingsFilePath, cancellationToken);

    return JsonConvert.DeserializeObject<PipelineModel>(settingsFileContent);
  }

  private static IServiceProvider GetServicesProvider()
  {
    var configuration = ConfigurationLoader.LoadConfiguration();

    var collection = new ServiceCollection();

    LogConfigurator.ConfigureConsoleLogging(collection, configuration);

    RegisterApplicationServices(collection, configuration);

    new PluginsAssembler().AssemblePlugins(collection);

    var serviceProvider = collection.BuildServiceProvider();

    return serviceProvider;
  }

  private static void RegisterApplicationServices(
    ServiceCollection collection,
    IConfigurationRoot configuration)
  {
    collection.AddSingleton<IDesktopService, DesktopServiceStub>();
    collection.AddSingleton<IWindowFactory, WindowFactoryStub>();
  }
}
