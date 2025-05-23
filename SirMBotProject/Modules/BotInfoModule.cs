using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirMBotProject.Modules
{
    public class BotInfoModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("botinfo", "Return informations about the bot")]
        public async Task BotInfoCommand()
        {
            var botUser = Context.Client.CurrentUser;

            var embed = new EmbedBuilder()
                .WithTitle("🤖 Bot Information")
                .WithThumbnailUrl(botUser.GetAvatarUrl() ?? botUser.GetDefaultAvatarUrl())
                .AddField("Username", botUser.Username, true)
                .AddField("Discriminator", botUser.Discriminator, true)
                .AddField("ID", botUser.Id, true)
                .AddField("Created At", botUser.CreatedAt.UtcDateTime.ToString("u"), true)
                .AddField("Ping", $"{Context.Client.Latency} ms", true)
                .WithColor(Color.Blue)
                .WithCurrentTimestamp()
                .Build();

            await RespondAsync(embed: embed, ephemeral: true);
        }
    }
}
