using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;

namespace ProxyChecker.Exporters.UriTextFile
{
  internal partial class ExporterSettingsControlViewModel
    : ObservableValidator
  {
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    private string? _filePath;

    public Control Control { get; set; } = default!;

    [RelayCommand]
    private async Task SelectFilePathAsync(CancellationToken cancellationToken)
    {
      var topLevel = TopLevel.GetTopLevel(Control);

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
          FilePath = path;
        }
      }
    }
  }
}
