namespace ProxyChecker.Common.Storage;

public interface INamedEntity
{
	int Id { get; set; }
	string Name { get; set; }
}
