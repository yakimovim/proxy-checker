using Avalonia.Controls;
using ProxyChecker.Interfaces.Loaders;
using ProxyChecker.ViewModels;

namespace ProxyChecker;

internal partial class CreateLoaderWindow : Window
{
  public CreateLoaderWindow(CreateWindowViewModel<ILoaderCreator> model)
  {
    DataContext = model;

    model.Window = this;

    InitializeComponent();
  }
}