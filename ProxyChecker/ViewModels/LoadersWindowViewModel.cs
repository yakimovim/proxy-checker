using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.ViewModels;
using ProxyChecker.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ProxyChecker.ViewModels
{
	internal partial class LoadersWindowViewModel : ViewModelBase, IRequireWindow
	{
    private readonly IWindowFactory _windowFactory;
    
		[ObservableProperty]
		private ObservableCollection<LoaderViewModel> _loaders = new();

    public Window Window { get; set; } = default!;

    public LoadersWindowViewModel(IWindowFactory windowFactory)
    {
      _windowFactory = windowFactory ?? throw new System.ArgumentNullException(nameof(windowFactory));
    }

    [RelayCommand]
		private async Task Add()
		{
			var dialog = _windowFactory.CreateWindow<CreateLoaderWindow>();

			var result = await dialog.ShowDialog<LoaderCreationModel?>(Window);

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
