# Proxy Checker

Proxy Checker is a desktop application to check availability of proxy servers. There are many sources of different proxy servers. But it does not mean that all of them will work at the place where you are. Proxy Checker allows you to download lists of proxy servers from different sources, check there availability and export those proxy servers that passed the test.

## Installation

There are several way to install Proxy Checker.

### From Git release

Go to Releases page of [GitVerse](https://gitverse.ru/yakimovim/proxy-checker/releases) or [GitHub](https://github.com/yakimovim/proxy-checker/releases). Select the release corresponding to your operation system. Download the archive and unpack it to any folder. The application is ready to use.

### From Chocolatey

## UI Usage

Start `ProxyChecker` application.

First of all, you should create instances of loaders, checkers and exporters. It can be done through main menu, `File` and then `Loaders...`, `Checkers...` and `Exporters...` menu items. In each case you'll see a windows with a list of corresponding entities. With `Add` button you can create a new loader, checker or exporter. In the `Name` field you can set meaningful name for you entity. With a combobox below you can choose specific type of entity. With `Settings` button you can open settings window for each entity. `Delete` button allows you to delete it. `Make active` button allows you to select one of them as active (current).

Make sure you have created at least one loader, checker and exporter.

When you have created loaders, you'll be able to use `Load` button in the main window. Dropdown near it allows to select active loader.

After some proxies are loaded, you can use `Check` button to apply selected checker to thses proxies.

Checked proxies can be exported using `Export` button.

### Creation of settings file

Proxy Checker includes `ProxyChecker.Cli` application which can be used in terminal. But is order to use it you first should create settings file. This file contains descriptions of loader, checker and exporter. The simplest way to create such a file is through UI. Open `ProxyChecker` UI application. Setup loader, checker and exporter. Then in main menu, `File` select `Export settings...` menu item. Select target file. Configuration of current loader, checker and exporter will be written into this file.

## CLI Usage

Application `ProxyChecker.Cli` can be used for automation of getting, checking and exporting lists of proxies.

```ps
ProxyChecker.Cli.exe --settings "c:\Temp\settings.json"
```

File `settings.json` contains configurations of loader, checker and exported. The simplest way to create this file is through UI, as it is described above.

## Contribution

If you want to inform about an issue or propose an improvement, please, use

* [Issues](https://github.com/yakimovim/proxy-checker/issues) or [Pull Requests](https://github.com/yakimovim/proxy-checker/pulls) on [GitHub](https://github.com/yakimovim/proxy-checker) or
* [Issues](https://gitverse.ru/yakimovim/proxy-checker/tasktracker) or [Requests](https://gitverse.ru/yakimovim/proxy-checker/pulls) on [GitVerse](https://gitverse.ru/yakimovim/proxy-checker).

## Extension

Proxy Checker is based on [MEF technology](https://learn.microsoft.com/en-us/dotnet/standard/mef/). All loaders, checkers and exporters are just plugins. Nothing prevents you from creation your own entities. Here is how you can do it.

Create a new project of type `Class Library`. Use the same .NET version as [ProxyChecker](https://github.com/yakimovim/proxy-checker/blob/master/ProxyChecker/ProxyChecker.csproj) project. Add reference to the `ProxyChecker.Interfaces` assembly which you can take in the folder where you've installed Proxy Checker. If you need to add some NuGet packages, take a look at the versions of already used packages in the `Directory.Packages.props` file. Please, use the same versions to avoid compatibility issues.

If you want to create a loader, you should implement `ILoader` interface from `ProxyChecker.Interfaces` assembly. Use `IChecker` for checker and `IExporter` for exporter. It is highly recommended to take a look at how similar entities are implemented in Proxy Checker project.

Create a creator for your entity (`ILoaderCreator`, `ICheckerCreator` or `IExporterCreator`).

Create a `ServicesRegistrator` class which should register your entities into a dependency container:

```cs
[Export(typeof(IServicesRegistrator))]
public class ServicesRegistrator : IServicesRegistrator
{
  public void RegisterServices(IServiceCollection services)
  {
    services.AddTransient<ILoaderCreator, LoaderCreator>();
  }
}
```

When your implementation is finished, use self-contained .NET publishing:

```ps
dotnet publish -o publish --sc -r win-x64
```

Option `--sc` makes publishing to create self-contained result.

You don't need to distribute all NuGet assemblies which are already part of Proxy Checker and can be found in the folder of Proxy Checker installation.

In this main folder go into `Plugins` directory. If you create a loader, go to `Loaders` folder, for checker - to `Checkers` and `Exporters` for exporter. Here create a new directory for your plugin. The name of the directory is not important. Place result of your publishing into this new directory.