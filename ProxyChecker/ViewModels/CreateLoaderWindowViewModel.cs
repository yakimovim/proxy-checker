using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Interfaces.Loaders;
using ProxyChecker.Interfaces.Services;
using ProxyChecker.Models;
using System.Collections.Generic;
using System.Linq;

namespace ProxyChecker.ViewModels
{
  internal partial class CreateLoaderWindowViewModel : ViewModelBase
  {
    private readonly Window _window;

    public CreateLoaderWindowViewModel(
      Window window,
      IDesktopProvider desktopProvider,
      IEnumerable<ILoaderCreator> loaderCreators)
      : base(desktopProvider)
    {
      _window = window ?? throw new System.ArgumentNullException(nameof(window));
      LoaderCreators = loaderCreators ?? throw new System.ArgumentNullException(nameof(loaderCreators));

      SelectedLoaderCreator = loaderCreators.FirstOrDefault();

      _window.DataContext = this;
    }

    public IEnumerable<ILoaderCreator> LoaderCreators { get; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private string _name = string.Empty;

    [ObservableProperty]
    private ILoaderCreator? _selectedLoaderCreator;

    [RelayCommand(CanExecute = nameof(CanCreate))]
    private void Ok()
    {
      _window.Close(
        new LoaderCreationModel 
        {
          Name = Name,
          LoaderCreator = SelectedLoaderCreator!
        }
      );
    }

    private bool CanCreate()
      => !string.IsNullOrWhiteSpace(Name) && SelectedLoaderCreator is not null;

    [RelayCommand]
    private void Cancel()
    {
      _window.Close(null);
    }
  }
}
