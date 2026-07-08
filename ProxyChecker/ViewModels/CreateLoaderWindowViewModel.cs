using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Interfaces.Loaders;
using ProxyChecker.Interfaces.ViewModels;
using ProxyChecker.Models;
using ProxyChecker.Services;
using System.Collections.Generic;
using System.Linq;

namespace ProxyChecker.ViewModels
{
  internal partial class CreateLoaderWindowViewModel : ViewModelBase, IRequireWindow
	{
		public CreateLoaderWindowViewModel()
		{
			SelectedLoaderCreator = LoaderCreators.FirstOrDefault();
		}

		public IEnumerable<ILoaderCreator> LoaderCreators => Plugins.LoaderCreators;

		public Window Window { get; set; } = default!;

    [ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(OkCommand))]
		private string _name = string.Empty;

		[ObservableProperty]
		private ILoaderCreator? _selectedLoaderCreator;

		[RelayCommand(CanExecute = nameof(CanCreate))]
		private void Ok()
		{
			Window.Close(
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
			Window.Close(null);
		}
	}
}
