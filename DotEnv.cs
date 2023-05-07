using System;
using System.IO;

namespace Midori;

internal class DotEnv
{
  public static void load(string path = "./.env")
  {
    if (!File.Exists(path)) return;
    foreach (string line in File.ReadAllLines(path))
    {
      if (line.Contains("="))
      {
        string[] values = line.Split("=", 2, StringSplitOptions.RemoveEmptyEntries);
        string key = values[0];
        string value = values[1];
        Environment.SetEnvironmentVariable(key, value);
      }
    }
  }
}
