using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Exporters;
using ProxyChecker.Interfaces.Loaders;
using System.Runtime.CompilerServices;

namespace ProxyChecker.Exporters.UriTextFile
{
	internal class Exporter : IExporter
	{
		private readonly IDesktopService _desktopService;
		private ExporterSettings _settings = new();

		public Exporter(IDesktopService desktopService)
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
			_settings = settings?.ToObject<ExporterSettings>()!;
		}

		public Control GetSettingsControl()
		{
			var viewModel = new ExporterSettingsControlViewModel
			{
				FilePath = _settings.FilePath
			};

			return new ExporterSettingsControl(viewModel);
		}

		private ExporterSettings? GetTypedSettingsFromControl(Control? control)
		{
			if (control is not ExporterSettingsControl settingsControl)
			{
				return null;
			}

			if (settingsControl.DataContext is not ExporterSettingsControlViewModel viewModel)
			{
				return null;
			}

			var settings = new ExporterSettings
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

    public async Task ExportAsync(IEnumerable<Proxy> proxies, CancellationToken cancellationToken)
    {
      var sourceFilePath = _settings.FilePath;

      if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
      {
        var topLevel = TopLevel.GetTopLevel(_desktopService.Desktop.MainWindow);

        if (topLevel == null)
        {
					return;
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
          Title = "Choose URI File",
          AllowMultiple = false,
        });

        if (files.Count > 0)
        {
          var path = files[0].TryGetLocalPath();

          if (!string.IsNullOrWhiteSpace(path))
          {
            sourceFilePath = path;
          }
        }
      }

      if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
      {
				return;
      }

      using var stream = File.OpenWrite(sourceFilePath);

			using var writer = new StreamWriter(stream);

			foreach (var proxy in proxies)
			{
				await writer.WriteLineAsync(proxy.GetUri().ToString());
			}
    }
  }
}
