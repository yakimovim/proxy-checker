using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    var loader = GetCreatedEntity<ILoader, ILoaderCreator>(
      servicesProvider,
      pipelineSettings.LoaderCreatorUid,
      pipelineSettings.LoaderSettings,
      Resource.LoaderCreatorNotFoundMessage,
      Resource.LoaderInvalidSettingsMessage
    );

    if (loader == null)
    {
      return 1;
    }

    var checker = GetCreatedEntity<IChecker, ICheckerCreator>(
      servicesProvider,
      pipelineSettings.CheckerCreatorUid,
      pipelineSettings.CheckerSettings,
      Resource.CheckerCreatorNotFoundMessage,
      Resource.CheckerInvalidSettingsMessage
    );

    if (checker == null)
    {
      return 1;
    }

    var exporter = GetCreatedEntity<IExporter, IExporterCreator>(
      servicesProvider,
      pipelineSettings.ExporterCreatorUid,
      pipelineSettings.ExporterSettings,
      Resource.ExporterCreatorNotFoundMessage,
      Resource.ExporterInvalidSettingsMessage
    );

    if (exporter == null)
    {
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

  private static TEntity? GetCreatedEntity<TEntity, TEntityCreator>(
    IServiceProvider servicesProvider,
    Guid creatorUid,
    JToken? entitySettings,
    string noCreatorMessage,
    string invalidSettingsMessage
    )
    where TEntity : IEntityWithSettings
    where TEntityCreator : ICreator<TEntity>
  {
    var creator = servicesProvider.GetServices<TEntityCreator>()
      .SingleOrDefault(l => l.Uid == creatorUid);

    if (creator is null)
    {
      AnsiConsole.MarkupLine($"[red]{noCreatorMessage}[/]");
      return default;
    }

    var entity = creator.Create();

    entity.SetSettings(entitySettings);

    var validationResult = entity.ValidateSettingsForCli();

    if (!validationResult.IsValid)
    {
      AnsiConsole.MarkupLine($"[red]{invalidSettingsMessage}[/]");

      AnsiConsole.WriteLine();

      foreach (var error in validationResult.Errors)
      {
        AnsiConsole.MarkupLine($"[red]- {error.ErrorMessage}[/]");
      }

      return default;
    }

    return entity;
  }
}
