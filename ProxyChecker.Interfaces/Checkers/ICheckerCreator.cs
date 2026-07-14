namespace ProxyChecker.Interfaces.Checkers
{
  public interface ICheckerCreator : ICreator
  {
    IChecker Create();
  }
}
