using Avalonia.Controls;
using ProxyChecker.Interfaces.Checkers;
using ProxyChecker.ViewModels;

namespace ProxyChecker;

internal partial class CreateCheckerWindow : Window
{
  public CreateCheckerWindow(CreateWindowViewModel<ICheckerCreator> viewModel)
  {
    DataContext = viewModel;

    viewModel.Window = this;

    InitializeComponent();
  }
}