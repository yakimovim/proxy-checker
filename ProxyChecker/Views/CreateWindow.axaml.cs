using Avalonia.Controls;
using ProxyChecker.ViewModels;

namespace ProxyChecker;

internal partial class CreateWindow : Window
{
  public CreateWindow(CreateWindowViewModel viewModel)
  {
    DataContext = viewModel;

    viewModel.Window = this;

    InitializeComponent();
  }
}