using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SirMBotProject.Services
{
    public class ServerLoggingService
    {
        private readonly DiscordSocketClient _client;
        private readonly ulong _logChannelId;

        public ServerLoggingService(DiscordSocketClient client, ulong logChannelId)
        {
            _client = client;
            _logChannelId = logChannelId;
        }

        public async Task LogEventAsync(Embed embed)
        {
            if (_client.GetChannel(_logChannelId) is IMessageChannel channel)
                await channel.SendMessageAsync(embed: embed);
        }

        public async Task LogErrorASync(string message, string username, Exception? ex = null)
        {
            IMessageChannel? channel = _client.GetChannel(_logChannelId) as IMessageChannel;
            var embed = new EmbedBuilder()
                .WithTitle("Error Log")
                .WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl() ?? _client.CurrentUser.GetDefaultAvatarUrl())
                .AddField("Error Title:", message)
                .AddField("User responsible for executing the command:", username)
                .AddField("Error:", ex)
                .WithColor(Color.Red)
                .WithCurrentTimestamp()
                .Build();

            if (channel != null)
                await channel.SendMessageAsync(embed: embed);
        }
    }
}
