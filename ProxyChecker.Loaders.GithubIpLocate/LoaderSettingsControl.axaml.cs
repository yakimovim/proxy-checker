using Avalonia.Controls;

namespace ProxyChecker.Loaders.GithubIpLocate;

internal partial class LoaderSettingsControl : UserControl
{
  public LoaderSettingsControl(LoaderSettingsControlViewModel viewModel)
  {
    DataContext = viewModel;

    InitializeComponent();
  }
}