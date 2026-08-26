using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Common.Configuration;
using ProxyChecker.Common.Logging;
using ProxyChecker.Common.Services;
using ProxyChecker.Common.Storage;
using ProxyChecker.Factories;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Resources;
using ProxyChecker.Services;
using ProxyChecker.ViewModels;
using ProxyChecker.Views;

namespace ProxyChecker;

internal partial class LoadingWindow : Window
{
  private static readonly string StartupLogPath = Path.Combine(
    PathsProvider.GetLogsFolder(),
    "proxy-checker-startup.log"
  );

  private readonly IClassicDesktopStyleApplicationLifetime? _desktop;

  public LoadingWindow()
  {
    InitializeComponent();
  }

  public LoadingWindow(IClassicDesktopStyleApplicationLifetime desktop)
    : this()
  {
    _desktop = desktop;
  }

  protected override void OnOpened(EventArgs e)
  {
    base.OnOpened(e);

    Task.Run(async () =>
    {
      await InitializeApplicationAsync();
    });
  }

  private async Task InitializeApplicationAsync()
  {
    try
    {
      var configuration = ConfigurationLoader.LoadConfiguration();

      var collection = new ServiceCollection();

      RegisterApplicationServices(collection, configuration);

      new PluginsAssembler().AssemblePlugins(collection);

      var serviceProvider = collection.BuildServiceProvider();

      StoragePreparer.PrepareStorage(serviceProvider);

      if (_desktop is not null)
      {
        serviceProvider.GetRequiredService<DesktopService>().Desktop = _desktop;

        Dispatcher.Invoke(() =>
        {
          var mainWindow = serviceProvider.GetRequiredService<MainWindow>();

          mainWindow.Show();

          _desktop.MainWindow = mainWindow;

          Close();
        });
      }
      else
      {
        await Dispatcher.Invoke(async () =>
        {
          var dialog = new MessageWindow(Resource.NoDesktopErrorMessage);

          await dialog.ShowDialog(this);

          Close();

          Environment.Exit(1);
        });
      }
    }
    catch (Exception ex)
    {
      LogStartup(ex.ToString());

      await Dispatcher.Invoke(async () =>
      {
        var dialog = new MessageWindow(Resource.ApplicationInitializationErrorMessage);

        await dialog.ShowDialog(this);

        Close();

        Environment.Exit(1);
      });
    }

  }

  private static void RegisterApplicationServices(ServiceCollection collection, IConfigurationRoot configuration)
  {
    LogConfigurator.ConfigureFileLogging(collection, configuration);

    StorageConfigurator.ConfigureStorage(collection);

    collection.AddTransient<CurrentEntityProvider>();

    collection.AddTransient<IWindowFactory, WindowFactory>();

    collection.AddTransient<MainWindow>();
    collection.AddTransient<MainWindowViewModel>();

    collection.AddTransient<LoadersWindow>();
    collection.AddTransient<LoadersWindowViewModel>();

    collection.AddTransient(typeof(CreateWindowViewModel<>));

    collection.AddTransient<CheckersWindow>();
    collection.AddTransient<CheckersWindowViewModel>();

    collection.AddTransient<ExportersWindow>();
    collection.AddTransient<ExportersWindowViewModel>();

    collection.AddSingleton<DesktopService>();
    collection.AddSingleton<IDesktopService>(s => s.GetRequiredService<DesktopService>());
  }

  private void LogStartup(string message)
  {
    File.AppendAllText(StartupLogPath, Environment.NewLine + message);
  }
}