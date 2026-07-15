using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ProxyChecker.Checkers.OkResponse
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

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTargetUriCommand))]
    private string? _targetUriCandidate;

    [ObservableProperty]
    private ObservableCollection<Uri> _targetUris = new();

    [RelayCommand]
    private void DeleteTargetUri(Uri uri)
    {
      TargetUris.Remove(uri);
    }

    [RelayCommand(CanExecute = nameof(CanAddTargetUri))]
    private void AddTargetUri()
    {
      TargetUris.Add(new Uri(TargetUriCandidate!));

      TargetUriCandidate = string.Empty;
    }

    private bool CanAddTargetUri()
    {
      return Uri.TryCreate(TargetUriCandidate, UriKind.Absolute, out var _);
    }
  }
}
