using Avalonia.Controls;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;
using System.Runtime.CompilerServices;

namespace ProxyChecker.Loaders.UriTextFile
{
	internal class Loader : ILoader
	{
		private readonly IDesktopService _desktopService;
		private LoaderSettings _settings = new();

		public Loader(IDesktopService desktopService)
		{
			_desktopService = desktopService;
		}

		public string Name { get; set; } = string.Empty;

		public JToken GetSettings()
		{
			return JToken.FromObject(_settings);
		}

		public void SetSettings(JToken? settings)
		{
			_settings = settings?.ToObject<LoaderSettings>()!;
		}

		public async IAsyncEnumerable<Proxy> LoadAsync(
		  [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			var settings = _settings;

			if (string.IsNullOrEmpty(settings.FilePath) || !File.Exists(settings.FilePath))
			{
				var viewModel = new LoaderSettingsWindowViewModel
				{
					SettingsControl = GetSettingsControl(),
				};

				var dialog = new LoaderSettingsWindow(viewModel);

				if (!(await dialog.ShowDialog<bool>(_desktopService.Desktop.MainWindow!)))
				{
					yield break;
				}

				settings = GetTypedSettingsFromControl(viewModel.SettingsControl);

				if (settings == null)
				{
					yield break;
				}
			}

			if (string.IsNullOrEmpty(settings.FilePath) || !File.Exists(settings.FilePath))
			{
				yield break;
			}

			using var reader = File.OpenText(settings.FilePath);

			while (true)
			{
				var line = await reader.ReadLineAsync(cancellationToken);

				if (line is null)
				{
					break;
				}

				if (Uri.TryCreate(line, UriKind.Absolute, out var uri))
				{
					yield return new Proxy(
					  uri.Scheme,
					  uri.Host,
					  uri.Port
					);
				}
			}
		}

		public Control GetSettingsControl()
		{
			var viewModel = new LoaderSettingsControlViewModel
			{
				FilePath = _settings.FilePath
			};

			return new LoaderSettingsControl(viewModel);
		}

		private LoaderSettings? GetTypedSettingsFromControl(Control? control)
		{
			if (control is not LoaderSettingsControl loaderSettingsControl)
			{
				return null;
			}

			if (loaderSettingsControl.DataContext is not LoaderSettingsControlViewModel viewModel)
			{
				return null;
			}

			var settings = new LoaderSettings
			{
				FilePath = viewModel.FilePath
			};

			return settings;
		}

		public JToken? GetSettingsFromControl(Control? control)
		{
			var settings = GetTypedSettingsFromControl(control);

			return settings is null ? null : JToken.FromObject(settings);
		}
	}
}
