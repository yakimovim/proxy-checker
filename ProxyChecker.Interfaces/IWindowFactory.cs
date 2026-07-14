using Avalonia.Controls;

namespace ProxyChecker.Interfaces
{
  public interface IWindowFactory
  {
    TWindow CreateWindow<TWindow>() where TWindow : Window;
  }
}
