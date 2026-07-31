using System.ComponentModel.DataAnnotations;

namespace ProxyChecker.Common.Storage;

public class Checker : IPipelineEntity
{
	[Key]
	public int Id { get; set; }
	public string Name { get; set; } = default!;
	public Guid CreatorUid { get; set; }
	public string? JsonSettings { get; set; }
}
