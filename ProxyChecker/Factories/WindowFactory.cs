using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Interfaces;
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

    public TWindow CreateWindow<TWindow>()
			where TWindow : Window
		{
      return _serviceProvider.GetRequiredService<TWindow>();
		}
	}
}
