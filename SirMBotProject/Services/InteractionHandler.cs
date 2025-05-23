using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SirMBotProject.Services
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _interactions;
        private readonly IServiceProvider _services;

        public InteractionHandler(DiscordSocketClient client, InteractionService interactions, IServiceProvider services)
        {
            _client = client;
            _interactions = interactions;
            _services = services;
        }
        public async Task InitializeAsync()
        {
            _client.Ready += ReadyAsync;
            _client.InteractionCreated += HandleInteraction;

            await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);          
        }

        private async Task ReadyAsync()
        {
            //Get guild id
            var guildIdString = Environment.GetEnvironmentVariable("GUILD_ID");
            if (!ulong.TryParse(guildIdString, out var guildId))
            {
                throw new InvalidOperationException("GUILD_ID env variable is null or invalid");
            }

            //First register the commands in the guild
            await _interactions.RegisterCommandsToGuildAsync(guildId);
            //await _interactions.RegisterCommandsGloballyAsync();
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            var context = new SocketInteractionContext(_client, interaction);
            await _interactions.ExecuteCommandAsync(context, _services);
        }
    }
}
