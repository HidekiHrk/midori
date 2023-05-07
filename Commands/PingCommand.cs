using System.Threading.Tasks;
using Discord.WebSocket;

namespace Midori.Commands;

public class PingCommand : Command
{

  public override string[] names => new string[] { "ping" };

  public override async Task execAsync(SocketMessage message, string[] args)
  {
    await message.Channel.SendMessageAsync("pong!");
  }
}
