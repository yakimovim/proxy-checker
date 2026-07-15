using Avalonia.Controls;
using ProxyChecker.ViewModels;

namespace ProxyChecker;

internal partial class SettingsWindow : Window
{
  public SettingsWindow(NamedEntityWithSettingsViewModel model)
  {
    DataContext = model;

    model.Window = this;
    InitializeComponent();
  }
}