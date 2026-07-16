using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ProxyChecker.Interfaces.ViewModels
{
  internal partial class MessageWindowViewModel : ViewModelBase, IRequireWindow
  {
    [ObservableProperty]
    private string _message = string.Empty;

    public Window Window { get; set; } = default!;

    [RelayCommand]
    private void Ok()
    {
      Window.Close();
    }
  }
}
