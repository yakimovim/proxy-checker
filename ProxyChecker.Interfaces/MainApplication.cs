using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace ProxyChecker.Interfaces
{
	public static class MainApplication
	{
		public static Window MainWindow => Desktop.MainWindow!;

		public static IClassicDesktopStyleApplicationLifetime Desktop { get; internal set; } = default!;
	}
}
