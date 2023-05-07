using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Midori;

public class Program
{
  private DiscordSocketClient client;
  public static Task Main(string[] args)
  {
    DotEnv.load();
    return new Program().MainAsync();
  }

  public async Task MainAsync()
  {
    DiscordSocketConfig config = new DiscordSocketConfig();
    config.GatewayIntents =
      GatewayIntents.AllUnprivileged |
      GatewayIntents.MessageContent |
      GatewayIntents.GuildMembers;

    this.client = new DiscordSocketClient(config);

    string token = Environment.GetEnvironmentVariable("TOKEN");
    if (token is null)
    {
      Console.WriteLine("Token is null.");
      return;
    }

    this.client.Ready += this.onReady;
    this.client.MessageReceived += this.onMessage;

    await this.client.LoginAsync(TokenType.Bot, token);
    await this.client.StartAsync();
    await Task.Delay(-1);
  }

  private async Task onMessage(SocketMessage message)
  {
    if (message.Content.StartsWith(".say"))
    {
      string[] args = message.Content.Split(" ");
      string argsContent = "";
      for (int i = 1; i < args.Length; i++)
      {
        argsContent += args[i] + (i != args.Length - 1 ? " " : "");
      }
      await message.Channel.SendMessageAsync(argsContent);
    }
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
