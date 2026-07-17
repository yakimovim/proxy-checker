using Avalonia.Controls;
using ProxyChecker.ViewModels;

namespace ProxyChecker;

internal partial class ExportersWindow : Window
{
  public ExportersWindow(ExportersWindowViewModel model)
  {
    DataContext = model;

    model.Window = this;

    InitializeComponent();
  }
}