using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Exporters;

namespace ProxyChecker.Exporters.UriTextFile;

internal class Exporter : ExporterBase<ExporterSettings>
{
  private readonly IDesktopService _desktopService;

  public Exporter(IDesktopService desktopService)
  {
    _desktopService = desktopService;
  }

  public override Control GetSettingsControl()
  {
    var viewModel = new ExporterSettingsControlViewModel
    {
      FilePath = _settings.FilePath
    };

    return new ExporterSettingsControl(viewModel);
  }

  protected override ExporterSettings? GetTypedSettingsFromControl(Control? control)
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

  public override async Task ExportAsync(IEnumerable<Proxy> proxies, CancellationToken cancellationToken)
  {
    var sourceFilePath = _settings.FilePath;

    if (string.IsNullOrEmpty(sourceFilePath))
    {
      var topLevel = TopLevel.GetTopLevel(_desktopService.Desktop.MainWindow);

      if (topLevel == null)
      {
        return;
      }

      var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
      {
        Title = Resource.SaveFilePickerTitle,
        ShowOverwritePrompt = true,
      });

      if (file is not null)
      {
        var path = file.TryGetLocalPath();

        if (!string.IsNullOrWhiteSpace(path))
        {
          sourceFilePath = path;
        }
      }
    }

    if (string.IsNullOrEmpty(sourceFilePath))
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
