using Avalonia.Controls;
using ProxyChecker.ViewModels;

namespace ProxyChecker;

internal partial class LoaderSettingsWindow : Window
{
  public LoaderSettingsWindow(NamedEntityWithSettingsViewModel model)
  {
    DataContext = model;

    model.Window = this;

    InitializeComponent();
  }
}