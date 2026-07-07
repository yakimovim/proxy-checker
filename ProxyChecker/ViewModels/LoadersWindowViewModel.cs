using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Interfaces.Loaders;
using ProxyChecker.Interfaces.Services;
using ProxyChecker.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ProxyChecker.ViewModels
{
  internal partial class LoadersWindowViewModel : ViewModelBase
  {
    private readonly IEnumerable<ILoaderCreator> _loaderCreators;

    public LoadersWindowViewModel(
      IDesktopProvider desktopProvider,
      IEnumerable<ILoaderCreator> loaderCreators
      ) 
      : base(desktopProvider)
    {
      _loaderCreators = loaderCreators;
    }

    public Window Window { get; set; } = default!;

    [ObservableProperty]
    private ObservableCollection<LoaderViewModel> _loaders = new();

    [RelayCommand]
    private async Task Add()
    {
      var dialog = new CreateLoaderWindow();

      var viewModel = new CreateLoaderWindowViewModel(dialog, _desktopProvider, _loaderCreators);

      var result = await dialog.ShowDialog<LoaderCreationModel?>(Window);

      if (result is not null)
      {
        Loaders.Add(
          new LoaderViewModel(_desktopProvider)
          {
            Name = result.Name,
          }
        );
      }
    }
  }
}
