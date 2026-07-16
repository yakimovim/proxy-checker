using Avalonia.Controls;
using ProxyChecker.Interfaces.ViewModels;

namespace ProxyChecker.Interfaces;

public partial class MessageWindow : Window
{
  public MessageWindow(string message)
  {
    var viewModel = new MessageWindowViewModel
    {
      Message = message
    };

    DataContext = viewModel;

    viewModel.Window = this;

    InitializeComponent();
  }
}