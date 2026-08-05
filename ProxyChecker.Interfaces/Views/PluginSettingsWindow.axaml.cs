using Avalonia.Controls;
using ProxyChecker.Interfaces.ViewModels;

namespace ProxyChecker.Interfaces;

public partial class PluginSettingsWindow : Window
{
  public PluginSettingsWindow()
  {
    InitializeComponent();
  }
  
  public PluginSettingsWindow(PluginSettingsWindowViewModel viewModel)
    : this()
  {
    DataContext = viewModel;

    viewModel.Window = this;
  }
}