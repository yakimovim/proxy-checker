using Avalonia.Controls;
using ProxyChecker.ViewModels;

namespace ProxyChecker;

internal partial class CreateLoaderWindow : Window
{
  public CreateLoaderWindow(CreateLoaderWindowViewModel model)
  {
    DataContext = model;

    model.Window = this;

    InitializeComponent();
  }
}