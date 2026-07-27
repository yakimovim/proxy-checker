using Newtonsoft.Json.Linq;
using System;

namespace ProxyChecker.Models;

internal class PipelineModel
{
  public Guid LoaderCreatorUid { get; set; }

  public JToken? LoaderSettings { get; set; }

  public Guid CheckerCreatorUid { get; set; 
  }
  public JToken? CheckerSettings { get; set; }

  public Guid ExporterCreatorUid { get; set; }

  public JToken? ExporterSettings { get; set; }
}
