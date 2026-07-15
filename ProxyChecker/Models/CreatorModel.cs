using ProxyChecker.Interfaces;

namespace ProxyChecker.Models
{
  internal class CreatorModel<TCreator> where TCreator : ICreator
  {
    public string Name { get; init; } = default!;

    public TCreator Creator { get; init; } = default!;
  }
}
