using Avalonia.Controls;

namespace ProxyChecker.Loaders.FlashProxyApi;

internal partial class LoaderSettingsControl : UserControl
{
  public LoaderSettingsControl(LoaderSettingsControlViewModel viewModel)
  {
    DataContext = viewModel;

    InitializeComponent();
  }
}