using Avalonia.Controls;
using ProxyChecker.Interfaces;

namespace ProxyChecker.Cli.Services;

internal class WindowFactoryStub : IWindowFactory
{
  public Window CreateCreateWindow<TCreator>() where TCreator : ICreator
  {
    throw new NotSupportedException(Resource.NoWindowFactory);
  }

  public TWindow CreateWindow<TWindow>() where TWindow : Window
  {
    throw new NotSupportedException(Resource.NoWindowFactory);
  }
}
