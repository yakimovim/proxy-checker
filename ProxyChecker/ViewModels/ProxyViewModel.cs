using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProxyChecker.Interfaces;
using ProxyChecker.Interfaces.ViewModels;
using ProxyChecker.Services;
using TextCopy;

namespace ProxyChecker.ViewModels
{
  internal partial class ProxyViewModel : ViewModelBase
  {
    [ObservableProperty]
    private string _scheme = string.Empty;

    [ObservableProperty]
    private string _host = string.Empty;

    [ObservableProperty]
    private int _port;

    [ObservableProperty]
    private string? _user;

    [ObservableProperty]
    private string? _password;

    public ProxyViewModel()
    { }

    public ProxyViewModel(Proxy proxy) 
    {
      _scheme = proxy?.Scheme ?? string.Empty;
      _host = proxy?.Host ?? string.Empty;
      _port = proxy?.Port ?? 0;
      _user = proxy?.User;
      _password = proxy?.Password;
    }

    public Proxy ToProxy()
      => new Proxy(Scheme, Host, Port, User, Password);

    [RelayCommand]
    private async Task CopyToClipboardAsync(CancellationToken cancellationToken)
    {
      var text = ToProxy().GetUri().ToString();

      await ClipboardService.SetTextAsync(text, cancellationToken);
    }
  }
}
