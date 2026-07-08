using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.ViewModels;
using ProxyChecker.Models;
using ProxyChecker.Storage;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyChecker.ViewModels
{
  internal partial class LoadersWindowViewModel : ViewModelBase, IRequireWindow
  {
    private readonly IWindowFactory _windowFactory;
    private readonly AppDbContext _db;

    [ObservableProperty]
    private ObservableCollection<LoaderViewModel> _loaders = new();

    public Window Window { get; set; } = default!;

    public LoadersWindowViewModel(
      IWindowFactory windowFactory,
      AppDbContext db)
    {
      _windowFactory = windowFactory ?? throw new System.ArgumentNullException(nameof(windowFactory));
      _db = db;

      var storedLoaders = _db.Loaders.ToArray();

      foreach (var loader in storedLoaders)
      {
        Loaders.Add(new LoaderViewModel(loader));
      }
    }

    [RelayCommand]
    private async Task Add(CancellationToken cancellationToken)
    {
      var dialog = _windowFactory.CreateWindow<CreateLoaderWindow>();

      var result = await dialog.ShowDialog<LoaderCreationModel?>(Window);

      if (result is not null)
      {
        var loader = result.LoaderCreator.Create();

        Loader dbLoader = new()
        {
          Name = result.Name,
          CreatorUid = result.LoaderCreator.Uid,
          JsonSettings = loader.GetSettings()?.ToString(Formatting.None),
        };

        await _db.Loaders.AddAsync(dbLoader);

        await _db.SaveChangesAsync(cancellationToken);

        Loaders.Add(
          new LoaderViewModel(dbLoader)
        );

        var settings = _db.Settings.Single();

        if (settings.LoaderId is null)
        {
          settings.LoaderId = dbLoader.Id;

          await _db.SaveChangesAsync(cancellationToken);
        }
      }
    }

    [RelayCommand]
    private async Task DeleteAsync(LoaderViewModel loaderViewModel, CancellationToken cancellationToken)
    {
      Loader? loader = await _db.Loaders.FindAsync(loaderViewModel.Id);

      if (loader is not null)
      {
        _db.Loaders.Remove(loader);

        await _db.SaveChangesAsync(cancellationToken);

        var settings = _db.Settings.Single();

        if (settings.LoaderId == loaderViewModel.Id)
        {
          settings.LoaderId = (await _db.Loaders.FirstOrDefaultAsync())?.Id;

          await _db.SaveChangesAsync(cancellationToken);
        }

        Loaders.Remove(loaderViewModel);
      }
    }

    [RelayCommand]
    private void ShowSettings(LoaderViewModel loaderViewModel)
    {
      ;
    }

    [RelayCommand]
    private async Task MakeActiveAsync(LoaderViewModel loaderViewModel, CancellationToken cancellationToken)
    {
      var settings = _db.Settings.Single();

      settings.LoaderId = loaderViewModel.Id;

      await _db.SaveChangesAsync(cancellationToken);
    }
  }
}
