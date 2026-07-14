using Microsoft.Extensions.DependencyInjection;
using ProxyChecker.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace ProxyChecker.Services
{
  internal class PluginsAssembler
  {
    private readonly string _pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

    [ImportMany(typeof(IServicesRegistrator))]
    public IEnumerable<IServicesRegistrator>? ServicesRegistrators { get; set; }

    public void AssemblePlugins(IServiceCollection services)
    {
      CreatePluginsDirectoryIfNecessary();

      GetRegistrators();

      RegisterServices(services);
    }

    private void CreatePluginsDirectoryIfNecessary()
    {
      if (!Directory.Exists(_pluginPath))
      {
        Directory.CreateDirectory(_pluginPath);
      }
    }

    private void GetRegistrators()
    {
      var catalog = new AggregateCatalog();

      AddDirectory(_pluginPath, catalog);

      var container = new CompositionContainer(catalog);

      container.ComposeParts(this);
    }

    private void AddDirectory(string pluginPath, AggregateCatalog catalog)
    {
      if (Directory.Exists(pluginPath))
      {
        catalog.Catalogs.Add(new DirectoryCatalog(pluginPath));

        foreach(var subDir in Directory.EnumerateDirectories(pluginPath))
        {
          AddDirectory(subDir, catalog);
        }
      }
    }

    private void RegisterServices(IServiceCollection services)
    {
      if (ServicesRegistrators != null)
      {
        foreach (var registrator in ServicesRegistrators)
        {
          registrator.RegisterServices(services);
        }
      }
    }
  }
}
