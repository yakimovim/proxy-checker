using Avalonia.Controls;

namespace ProxyChecker.Checkers.OkResponse;

internal partial class CheckerSettingsControl : UserControl
{
  public CheckerSettingsControl(CheckerSettingsControlViewModel viewModel)
  {
    DataContext = viewModel;

    viewModel.Control = this;

    InitializeComponent();
  }
}