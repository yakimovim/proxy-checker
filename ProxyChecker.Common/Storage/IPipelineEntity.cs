namespace ProxyChecker.Common.Storage;

public interface IPipelineEntity : INamedEntity
{
  Guid CreatorUid { get; set; }
  string? JsonSettings { get; set; }
}
