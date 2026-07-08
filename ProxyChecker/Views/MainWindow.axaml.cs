using Avalonia.Controls;
using ProxyChecker.ViewModels;

namespace ProxyChecker.Views
{
  internal partial class MainWindow : Window
  {
    public MainWindow(MainWindowViewModel model)
    {
      DataContext = model;

      model.Window = this;

      InitializeComponent();
    }
  }
}