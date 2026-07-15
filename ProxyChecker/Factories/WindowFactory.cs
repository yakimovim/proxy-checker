using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Interfaces;
using ProxyChecker.ViewModels;
using System;

namespace ProxyChecker.Factories
{
	internal class WindowFactory : IWindowFactory
  {
    private readonly IServiceProvider _serviceProvider;

    public WindowFactory(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public Window CreateCreateWindow<TCreator>() where TCreator : ICreator
    {
      return new CreateWindow(
        _serviceProvider.GetRequiredService<CreateWindowViewModel<TCreator>>()
      );
    }

    public TWindow CreateWindow<TWindow>()
			where TWindow : Window
		{
      return _serviceProvider.GetRequiredService<TWindow>();
		}
	}
}
