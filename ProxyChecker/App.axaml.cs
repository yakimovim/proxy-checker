using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Services;
using ProxyChecker.Storage;
using ProxyChecker.ViewModels;
using ProxyChecker.Views;

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
      var collection = new ServiceCollection();

      RegisterApplicationServices(collection);

      new PluginsAssembler().AssemblePlugins(collection);

      var serviceProvider = collection.BuildServiceProvider();

      if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
      {
        desktop.MainWindow = new MainWindow
        {
          DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
        };
      }

      base.OnFrameworkInitializationCompleted();
    }

    private void RegisterApplicationServices(ServiceCollection collection)
    {
      collection.AddDbContext<AppDbContext>(options => {
        options.UseSqlite("Data Source=app.db");
      });

      collection.AddTransient<MainWindowViewModel>();

      collection.AddTransient<ProxyCheckerService>();
    }
  }
}