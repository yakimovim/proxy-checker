using Avalonia.Controls;
using ProxyChecker.ViewModels;

namespace ProxyChecker;

internal partial class CheckerSettingsWindow : Window
{
  public CheckerSettingsWindow(CheckerSettingsWindowViewModel viewModel)
  {
    DataContext = viewModel;

    viewModel.Window = this;

    InitializeComponent();
  }
}