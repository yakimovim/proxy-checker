using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Factories;
using ProxyChecker.Interfaces;
using ProxyChecker.Services;
using ProxyChecker.Storage;
using ProxyChecker.ViewModels;
using ProxyChecker.Views;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace ProxyChecker
{
  public partial class App : Application
  {
    public override void Initialize()
    {
      AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
      var configuration = ReadConfiguration();

      var collection = new ServiceCollection();

      RegisterApplicationServices(collection, configuration);

      new PluginsAssembler().AssemblePlugins(collection);

      var serviceProvider = collection.BuildServiceProvider();

      PrepareDatabase(serviceProvider);

      if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
      {
        serviceProvider.GetRequiredService<DesktopService>().Desktop = desktop;

        desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
      }

      base.OnFrameworkInitializationCompleted();
    }

    private IConfigurationRoot ReadConfiguration()
    {
      return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
        .Build();
    }

    private void PrepareDatabase(ServiceProvider serviceProvider)
    {
      var db = serviceProvider.GetRequiredService<AppDbContext>();
      db.Database.EnsureCreated();

      if (!db.Settings.Any())
      {
        db.Settings.Add(new Settings());

        db.SaveChanges();
      }
    }

    private void RegisterApplicationServices(ServiceCollection collection, IConfigurationRoot configuration)
    {
      collection.AddLogging(loggingBuilder =>
      {
        loggingBuilder.AddSerilog(
          new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.File("app.log")
            .CreateLogger()
        );
      });

      collection.AddDbContext<AppDbContext>(options =>
      {
        options.UseSqlite("Data Source=app.db");
      });

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
  }
}