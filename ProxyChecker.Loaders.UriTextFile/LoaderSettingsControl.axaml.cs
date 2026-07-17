using Avalonia.Controls;

namespace ProxyChecker.Loaders.UriTextFile;

internal partial class LoaderSettingsControl : UserControl
{
  public LoaderSettingsControl(LoaderSettingsControlViewModel viewModel)
  {
    DataContext = viewModel;

    viewModel.Control = this;

    InitializeComponent();
  }
}