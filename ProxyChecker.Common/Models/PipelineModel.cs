using Newtonsoft.Json.Linq;

namespace ProxyChecker.Common.Models;

public class PipelineModel
{
  public Guid LoaderCreatorUid { get; set; }

  public JToken? LoaderSettings { get; set; }

  public Guid CheckerCreatorUid { get; set; 
  }
  public JToken? CheckerSettings { get; set; }

  public Guid ExporterCreatorUid { get; set; }

  public JToken? ExporterSettings { get; set; }
}
