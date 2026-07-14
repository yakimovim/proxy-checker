using ProxyChecker.Interfaces.Checkers;

namespace ProxyChecker.Models
{
  internal class CheckerCreationModel
  {
    public string Name { get; init; } = default!;

    public ICheckerCreator CheckerCreator { get; init; } = default!;
  }
}
