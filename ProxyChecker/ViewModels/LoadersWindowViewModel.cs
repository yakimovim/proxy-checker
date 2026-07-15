using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.Loaders;
using ProxyChecker.Interfaces.Resources;
using ProxyChecker.Interfaces.ViewModels;
using ProxyChecker.Models;
using ProxyChecker.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyChecker.ViewModels
{
  internal partial class LoadersWindowViewModel : ViewModelBase, IRequireWindow
  {
    private readonly IWindowFactory _windowFactory;
    private readonly IEnumerable<ILoaderCreator> _loaderCreators;
    private readonly AppDbContext _db;

    [ObservableProperty]
    private ObservableCollection<NamedEntityViewModel> _loaders = new();

    public Window Window { get; set; } = default!;

    public LoadersWindowViewModel(
      IWindowFactory windowFactory,
      IEnumerable<ILoaderCreator> loaderCreators,
      AppDbContext db)
    {
      _windowFactory = windowFactory ?? throw new System.ArgumentNullException(nameof(windowFactory));
      _loaderCreators = loaderCreators;
      _db = db;

      var storedLoaders = _db.Loaders.ToArray();

      var settings = _db.Settings.Single();

      foreach (var loader in storedLoaders)
      {
        Loaders.Add(
          new NamedEntityViewModel(loader)
          {
            IsActive = (loader.Id == settings.LoaderId),
          }
        );
      }
    }

    [RelayCommand]
    private async Task Add(CancellationToken cancellationToken)
    {
      var dialog = _windowFactory.CreateWindow<CreateLoaderWindow>();

      var result = await dialog.ShowDialog<CreatorModel<ILoaderCreator>?>(Window);

      if (result is not null)
      {
        var loader = result.Creator.Create();

        Loader dbLoader = new()
        {
          Name = result.Name,
          CreatorUid = result.Creator.Uid,
          JsonSettings = loader.GetSettings()?.ToString(Formatting.None),
        };

        await _db.Loaders.AddAsync(dbLoader);

        await _db.SaveChangesAsync(cancellationToken);

        NamedEntityViewModel loaderViewModel = new(dbLoader);

        Loaders.Add(
          loaderViewModel
        );

        var settings = _db.Settings.Single();

        if (settings.LoaderId is null)
        {
          settings.LoaderId = dbLoader.Id;

          loaderViewModel.IsActive = true;

          await _db.SaveChangesAsync(cancellationToken);
        }
      }
    }

    [RelayCommand]
    private async Task DeleteAsync(NamedEntityViewModel loaderViewModel, CancellationToken cancellationToken)
    {
      Loader? loader = await _db.Loaders.FindAsync(loaderViewModel.Id);

      if (loader is not null)
      {
        _db.Loaders.Remove(loader);

        await _db.SaveChangesAsync(cancellationToken);

        var appSettings = _db.Settings.Single();

        if (appSettings.LoaderId == loaderViewModel.Id)
        {
          appSettings.LoaderId = (await _db.Loaders.FirstOrDefaultAsync())?.Id;

          await _db.SaveChangesAsync(cancellationToken);

          if (appSettings.LoaderId is not null)
          {
            Loaders.Single(l => l.Id == appSettings.LoaderId.Value).IsActive = true;
          }
        }

        Loaders.Remove(loaderViewModel);
      }
    }

    [RelayCommand]
    private async Task ShowSettings(NamedEntityViewModel loaderViewModel, CancellationToken cancellationToken)
    {
      var dbLoader = await _db.Loaders.FindAsync(loaderViewModel.Id, cancellationToken);

      if (dbLoader is null)
      {
        return;
      }

      var loaderCreator = _loaderCreators.SingleOrDefault(c => c.Uid == dbLoader.CreatorUid);

      if (loaderCreator is null)
      {
        return;
      }

      var loader = loaderCreator.Create();

      var settingsToken = string.IsNullOrEmpty(dbLoader.JsonSettings) ? null : JToken.Parse(dbLoader.JsonSettings);

      loader.SetSettings(settingsToken);

      var settingsControl = loader.GetSettingsControl();

      var viewModel = new NamedEntityWithSettingsViewModel
      {
        WindowTitle = Resource.LoaderSettingsWindowTitle,
        SettingsLabel = Resource.LoaderSettingsLabel,
        Name = loaderViewModel.Name,
        SettingsControl = settingsControl,
      };

      var dialog = new SettingsWindow(viewModel);

      if(await dialog.ShowDialog<bool>(Window))
      {
        settingsToken = loader.GetSettingsFromControl(settingsControl);

        dbLoader.Name = viewModel.Name;

        dbLoader.JsonSettings = settingsToken?.ToString(Formatting.None);

        _db.SaveChanges();

        loaderViewModel.Name = viewModel.Name;
      }
    }

    [RelayCommand]
    private async Task MakeActiveAsync(NamedEntityViewModel loaderViewModel, CancellationToken cancellationToken)
    {
      var settings = _db.Settings.Single();

      settings.LoaderId = loaderViewModel.Id;

      await _db.SaveChangesAsync(cancellationToken);

      foreach(var vm in Loaders)
      {
        vm.IsActive = vm.Id == loaderViewModel.Id;
      }
    }
  }
}
