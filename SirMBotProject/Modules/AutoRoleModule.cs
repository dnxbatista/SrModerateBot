using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;

namespace SirMBotProject.Modules
{
    public class AutoRoleModule
    {
        private readonly DiscordSocketClient _client;
        private readonly ulong _roleId = 1374868207088832677;

        public AutoRoleModule(DiscordSocketClient client)
        {
            _client = client;
            _client.UserJoined += OnUserJoinedAsync;
        }

        private async Task OnUserJoinedAsync(SocketGuildUser user)
        {
            try
            {
                Console.WriteLine($"New User In: {user.Username}");
                var role = user.Guild.GetRole(_roleId);
                if (role != null)
                {
                    await user.AddRoleAsync(role);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AutoEvent Error:\n{ex}");
            }
        }
    }
}