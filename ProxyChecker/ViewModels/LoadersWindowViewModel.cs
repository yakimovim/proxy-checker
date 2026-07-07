using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ProxyChecker.ViewModels
{
	internal partial class LoadersWindowViewModel : ViewModelBase
	{
		private readonly Window _window; 

		[ObservableProperty]
		private ObservableCollection<LoaderViewModel> _loaders = new();

		public LoadersWindowViewModel(Window window)
		{
			_window = window ?? throw new System.ArgumentNullException(nameof(window));

			_window.DataContext = this;
		}

		[RelayCommand]
		private async Task Add()
		{
			var dialog = new CreateLoaderWindow();

			var viewModel = new CreateLoaderWindowViewModel(dialog);

			var result = await dialog.ShowDialog<LoaderCreationModel?>(_window);

			if (result is not null)
			{
				Loaders.Add(
				  new LoaderViewModel
				  {
					  Name = result.Name,
				  }
				);
			}
		}
	}
}
