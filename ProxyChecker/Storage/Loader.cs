using System;
using System.ComponentModel.DataAnnotations;

namespace ProxyChecker.Storage
{
  internal class Loader
  {
    [Key]
    public int Id { get; set; }
    public int Name { get; set; }
    public Guid CreatorUid { get; set; }
    public string? JsonSettings { get; set; }
  }
}
