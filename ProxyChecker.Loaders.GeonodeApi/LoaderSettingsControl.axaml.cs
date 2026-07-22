using Avalonia.Controls;

namespace ProxyChecker.Loaders.GeonodeApi;

internal partial class LoaderSettingsControl : UserControl
{
  public LoaderSettingsControl(LoaderSettingsControlViewModel viewModel)
  {
    DataContext = viewModel;

    InitializeComponent();
  }
}