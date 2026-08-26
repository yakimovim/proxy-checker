namespace ProxyChecker.Common.Services;

public static class PathsProvider
{
  public static string GetLogsFolder()
    => GetFolder(Environment.SpecialFolder.LocalApplicationData);

  public static string GetStorageFolder()
    => GetFolder(Environment.SpecialFolder.ApplicationData);

  private static string GetFolder(Environment.SpecialFolder folder)
  {
    var path = Path.Combine(
      Environment.GetFolderPath(folder),
      "ProxyChecker"
    );

    if (!Directory.Exists(path))
    {
      Directory.CreateDirectory(path);
    }

    return path;
  }
}
