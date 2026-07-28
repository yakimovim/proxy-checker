using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Cli.Services;
using ProxyChecker.Common.Configuration;
using ProxyChecker.Common.Logging;
using ProxyChecker.Common.Services;
using ProxyChecker.Interfaces;

namespace ProxyChecker.Cli;

internal static class PipelineExecutor
{
  public static async Task<int> ExecutePipeline(Options options)
  {
    var servicesProvider = GetServicesProvider();

    return 0;
  }

  private static object GetServicesProvider()
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
