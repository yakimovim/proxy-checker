using CommunityToolkit.Mvvm.ComponentModel;

namespace ProxyChecker.Loaders.UriTextFile
{
  internal partial class LoaderSettingsControlViewModel
    : ObservableObject
  {
    [ObservableProperty]
    private string? _filePath;
  }
}
