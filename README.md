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