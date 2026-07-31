namespace ProxyChecker.Interfaces;

public interface ICreator
{
  Guid Uid { get; }

  string Name { get; }

  string Description { get; }
}

public interface ICreator<T> : ICreator
{
  T Create();
}
