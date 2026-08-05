using Avalonia.Controls;
using ProxyChecker.Interfaces.ViewModels;

namespace ProxyChecker.Interfaces;

public partial class MessageWindow : Window
{
  public MessageWindow()
  {
    InitializeComponent();
  }

  public MessageWindow(string message)
    : this()
  {
    var viewModel = new MessageWindowViewModel
    {
      Message = message
    };

    DataContext = viewModel;

    viewModel.Window = this;
  }
}