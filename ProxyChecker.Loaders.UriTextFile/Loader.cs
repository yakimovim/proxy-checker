using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;
using System.Runtime.CompilerServices;

namespace ProxyChecker.Loaders.UriTextFile;

internal class Loader : LoaderBase<LoaderSettings>
{
	private readonly IDesktopService _desktopService;

	public Loader(IDesktopService desktopService)
	{
		_desktopService = desktopService;
	}

	public override async IAsyncEnumerable<Proxy> LoadAsync(
	  [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var sourceFilePath = _settings.FilePath;

		if (string.IsNullOrEmpty(sourceFilePath) || !File.Exists(sourceFilePath))
		{
			var topLevel = TopLevel.GetTopLevel(_desktopService.Desktop.MainWindow);

        if (topLevel == null)
        {
				yield break;
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
          Title = Resource.OpenFilePickerTitle,
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
			yield break;
		}

		using var reader = File.OpenText(sourceFilePath);

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

	public override Control GetSettingsControl()
	{
		var viewModel = new LoaderSettingsControlViewModel
		{
			FilePath = _settings.FilePath
		};

		return new LoaderSettingsControl(viewModel);
	}

	protected override LoaderSettings? GetTypedSettingsFromControl(Control? control)
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
}
