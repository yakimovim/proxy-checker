using System;
using System.ComponentModel.DataAnnotations;

namespace ProxyChecker.Storage
{
  internal class Checker : INamedEntity
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid CreatorUid { get; set; }
    public string? JsonSettings { get; set; }
  }
}
