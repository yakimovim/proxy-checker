namespace ProxyChecker.Interfaces.Loaders
{
  public interface ILoaderCreator : ICreator
  {
    ILoader Create();
  }
}
