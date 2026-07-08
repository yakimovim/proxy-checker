using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Factories;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;
using ProxyChecker.Services;
using ProxyChecker.Storage;
using ProxyChecker.ViewModels;
using ProxyChecker.Views;
using System;
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
			var collection = new ServiceCollection();

			RegisterApplicationServices(collection);

			new PluginsAssembler().AssemblePlugins(collection);

			var serviceProvider = collection.BuildServiceProvider();

			PreparePlugins(serviceProvider);

			PrepareDatabase(serviceProvider);

			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				serviceProvider.GetRequiredService<DesktopService>().Desktop = desktop;

				desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
			}

			base.OnFrameworkInitializationCompleted();
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

    private void RegisterApplicationServices(ServiceCollection collection)
		{
			collection.AddDbContext<AppDbContext>(options =>
			{
				options.UseSqlite("Data Source=app.db");
			});

			collection.AddTransient<IWindowFactory, WindowFactory>();

			collection.AddTransient<MainWindow>();
			collection.AddTransient<MainWindowViewModel>();

			collection.AddTransient<LoadersWindow>();
			collection.AddTransient<LoadersWindowViewModel>();

      collection.AddTransient<CreateLoaderWindow>();
      collection.AddTransient<CreateLoaderWindowViewModel>();

			collection.AddSingleton<DesktopService>();
			collection.AddSingleton<IDesktopService>(s => s.GetRequiredService<DesktopService>());

      collection.AddTransient<ProxyCheckerService>();
		}

		private void PreparePlugins(ServiceProvider serviceProvider)
		{
			Plugins.LoaderCreators = serviceProvider.GetServices<ILoaderCreator>() ?? Array.Empty<ILoaderCreator>();
		}
	}
}