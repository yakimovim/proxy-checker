using Avalonia.Controls;

namespace ProxyChecker.Loaders.UriTextFile;

internal partial class LoaderSettingsWindow : Window
{
  public LoaderSettingsWindow(LoaderSettingsWindowViewModel viewModel)
  {
    DataContext = viewModel;

    viewModel.Window = this;

    InitializeComponent();
  }
}