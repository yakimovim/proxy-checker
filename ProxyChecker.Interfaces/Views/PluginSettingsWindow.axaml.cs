using Avalonia.Controls;
using ProxyChecker.Interfaces.ViewModels;

namespace ProxyChecker.Interfaces;

public partial class PluginSettingsWindow : Window
{
  public PluginSettingsWindow(PluginSettingsWindowViewModel viewModel)
  {
    DataContext = viewModel;

    viewModel.Window = this;

    InitializeComponent();
  }
}