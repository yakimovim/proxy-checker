using Avalonia;
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
using ProxyChecker.Services;
using ProxyChecker.ViewModels;
using ProxyChecker.Views;
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
			var configuration = ConfigurationLoader.LoadConfiguration();

			var collection = new ServiceCollection();

			RegisterApplicationServices(collection, configuration);

			new PluginsAssembler().AssemblePlugins(collection);

			var serviceProvider = collection.BuildServiceProvider();

			StoragePreparer.PrepareStorage(serviceProvider);

			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				serviceProvider.GetRequiredService<DesktopService>().Desktop = desktop;

				desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
			}

			base.OnFrameworkInitializationCompleted();
		}

		private static void RegisterApplicationServices(ServiceCollection collection, IConfigurationRoot configuration)
		{
			LogConfigurator.ConfigureLogging(collection, configuration);

			StorageConfigurator.ConfigureStorage(collection);

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