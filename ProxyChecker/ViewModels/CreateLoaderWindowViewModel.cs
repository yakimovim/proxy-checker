using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Interfaces.Loaders;
using ProxyChecker.Models;
using ProxyChecker.Services;
using System.Collections.Generic;
using System.Linq;

namespace ProxyChecker.ViewModels
{
	internal partial class CreateLoaderWindowViewModel : ViewModelBase
	{
		private readonly Window _window;

		public CreateLoaderWindowViewModel(
		  Window window)
		{
			_window = window ?? throw new System.ArgumentNullException(nameof(window));

			SelectedLoaderCreator = LoaderCreators.FirstOrDefault();

			_window.DataContext = this;
		}

		public IEnumerable<ILoaderCreator> LoaderCreators => Plugins.LoaderCreators;

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
