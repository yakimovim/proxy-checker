using ProxyChecker.ViewModels;

namespace ProxyChecker.Factories
{
	internal class LoadersWindowFactory
	{
		public LoadersWindow CreateLoadersWindow()
		{
			var window = new LoadersWindow();

			LoadersWindowViewModel viewModel = new(window);

			return window;
		}
	}
}
