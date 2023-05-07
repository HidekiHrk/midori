using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Midori.Commands;
using System.Collections.Generic;

namespace Midori;

public class Program
{
  private Dictionary<string, Command> commands = new Dictionary<string, Command>();
  private DiscordSocketClient client;
  public static Task Main(string[] args)
  {
    DotEnv.load();
    return new Program().MainAsync();
  }

  public void loadCommands()
  {
    this.commands.Clear();
    Command[] rawCommands = {
      new PingCommand()
    };

    foreach (Command command in rawCommands)
    {
      command.SetClient(this.client);
      foreach (string name in command.names)
      {
        this.commands.Add(name, command);
      }
    }
  }

  public Command GetCommand(string name)
  {
    try
    {
      return this.commands[name];
    }
    catch (KeyNotFoundException)
    {
    }
    return null;
  }

  public async Task MainAsync()
  {
    DiscordSocketConfig config = new DiscordSocketConfig();
    config.GatewayIntents =
      GatewayIntents.AllUnprivileged |
      GatewayIntents.MessageContent |
      GatewayIntents.GuildMembers;

    this.client = new DiscordSocketClient(config);

    string token = DotEnv.get("TOKEN");
    if (token is null)
    {
      Console.WriteLine("Token is null.");
      return;
    }

    this.client.Ready += this.onReady;
    this.client.MessageReceived += this.onMessage;

    this.loadCommands();

    await this.client.LoginAsync(TokenType.Bot, token);
    await this.client.StartAsync();
    await Task.Delay(-1);
  }

  private async Task onMessage(SocketMessage message)
  {
    string[] args = Command.getCommandArgs(message.Content);
    if (args.Length == 0) return;
    string commandName = args[0];

    Command command = this.GetCommand(commandName);
    if (command is null)
    {
      Console.WriteLine("Command '{0}' not found", commandName);
      return;
    }
    await command.execAsync(message, args);
  }

  private async Task onReady()
  {
    SocketSelfUser user = this.client.CurrentUser;
    Console.WriteLine("Bot online!\nNome: {0}#{1}\nID: {2}",
      user.Username,
      user.Discriminator,
      user.Id
    );

    await this.client.SetActivityAsync(
      new Game("Color Bot!", ActivityType.Playing));
  }
}
