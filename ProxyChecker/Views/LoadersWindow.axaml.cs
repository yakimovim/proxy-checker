using Avalonia.Controls;
using ProxyChecker.ViewModels;

namespace ProxyChecker;

internal partial class LoadersWindow : Window
{
  public LoadersWindow(LoadersWindowViewModel model)
  {
    DataContext = model;

    model.Window = this;

    InitializeComponent();
  }
}