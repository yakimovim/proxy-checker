using Avalonia.Controls;
using ProxyChecker.ViewModels;

namespace ProxyChecker;

internal partial class LoaderSettingsWindow : Window
{
  public LoaderSettingsWindow(LoaderSettingsWindowViewModel model)
  {
    DataContext = model;

    model.Window = this;

    InitializeComponent();
  }
}