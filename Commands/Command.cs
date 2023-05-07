using System;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Midori.Commands;

public abstract class Command
{
  public static readonly string DEFAULT_PREFIX = ".";

  public abstract string[] names { get; }

  public DiscordSocketClient client;

  public void SetClient(DiscordSocketClient client)
  {
    this.client = client;
  }

  public bool validate(string[] args)
  {
    if (args.Length == 0) return false;
    string commandName = args[0].ToLower();
    return Array.Exists(this.names, (name) => name == commandName);
  }

  public abstract Task execAsync(SocketMessage message, string[] args);

  public static string[] getCommandArgs(string messageContent)
  {
    string prefix = DotEnv.get("PREFIX", DEFAULT_PREFIX);
    if (!messageContent.ToLower().StartsWith(prefix)) return null;
    string cleanContent = messageContent.Remove(0, prefix.Length);
    return cleanContent.Split(" ");
  }
}
