using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProxyChecker.Checkers.Anonymity
{
  internal partial class CheckerSettingsControlViewModel : ObservableValidator
  {
    public Control Control { get; set; } = default!;

    [ObservableProperty]
    private TimeSpan _timeout;

    public int TimeoutInSeconds
    {
      get => (int) Math.Floor(Timeout.TotalSeconds);
      set
      {
        Timeout = TimeSpan.FromSeconds(value);
        OnPropertyChanged(nameof(TimeoutInSeconds));
      }
    }
  }
}
