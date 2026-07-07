using ProxyChecker.Interfaces.Loaders;
using ProxyChecker.Interfaces.Services;
using ProxyChecker.ViewModels;
using System;
using System.Collections.Generic;

namespace ProxyChecker.Factories
{
  internal class LoadersWindowFactory
  {
    private readonly IDesktopProvider _desktopProvider;
    private readonly IEnumerable<ILoaderCreator> _loaderCreators;

    public LoadersWindowFactory(
      IDesktopProvider desktopProvider,
      IEnumerable<ILoaderCreator> loaderCreators
    )
    {
      _desktopProvider = desktopProvider ?? throw new ArgumentNullException(nameof(desktopProvider));
      _loaderCreators = loaderCreators ?? throw new ArgumentNullException(nameof(loaderCreators));
    }

    public LoadersWindow CreateLoadersWindow()
    {
      var window = new LoadersWindow();

      LoadersWindowViewModel viewModel = new(
        _desktopProvider, _loaderCreators
      );

      window.DataContext = viewModel;

      viewModel.Window = window;

      return window;
    }
  }
}
