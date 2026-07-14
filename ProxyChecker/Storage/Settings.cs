using System.ComponentModel.DataAnnotations;

namespace ProxyChecker.Storage
{
  public class Settings
  {
    [Key]
    public int Id { get; set; }

    public int? LoaderId { get; set; }

    public int? CheckerId { get; set; }

    public int? ExporterId { get; set; }
  }
}
