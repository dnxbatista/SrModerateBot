using Discord;
using Discord.Interactions;
using SirMBotProject.Services;
using System.Threading.Tasks;

namespace SirMBotProject.Modules
{
    // Build the modal
    public class KickReasonModal : IModal
    {
        public string Title => "Kick Modal";

        [InputLabel("Reason")]
        [ModalTextInput("reason",
        style: TextInputStyle.Paragraph,
        placeholder: "Please provide a detailed reason for kicking this user (3-200 characters)...",
        minLength: 3,
        maxLength: 200)]
        public string? Reason { get; set; }
    }

    // Build command
    public class KickModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ServerLoggingService _serverLoggingService;

        public KickModule(ServerLoggingService serverLoggingService)
        {
            _serverLoggingService = serverLoggingService;
        }

        [SlashCommand("kick", "Kick anyone from the server [Mod Only]")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task KickCommand(
            [Summary(description: "User thats gonna be kicked out")] IUser user
            )
        {
            string customId = $"kick_reason_modal:{user.Id}";
            await Context.Interaction.RespondWithModalAsync<KickReasonModal>(customId);
        }

        // Handle modal
        [ModalInteraction("kick_reason_modal:*")]
        public async Task HandleKickReasonModal(string customId, KickReasonModal modal)
        {
            try
            {
                var userIdStr = customId.Split(":").Last();
                if (!ulong.TryParse(userIdStr, out var userId))
                {
                    await RespondAsync("Error to process user", ephemeral: true);
                    return;
                }

                var guildUser = Context.Guild.GetUser(userId);
                if (guildUser == null)
                {
                    await RespondAsync("User not found!", ephemeral: true);
                    return;
                }

                // Kick user
                await guildUser.KickAsync(modal.Reason);

                // Build log embed
                var embed = new EmbedBuilder()
                    .WithTitle("Kick Log")
                    .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl() ?? Context.Client.CurrentUser.GetDefaultAvatarUrl())
                    .AddField("User kicked out:", guildUser.Username)
                    .AddField("Command executed by:", Context.User.Username)
                    .AddField("Reason:", modal.Reason)
                    .WithColor(Color.Orange)
                    .WithCurrentTimestamp()
                    .Build();

                await _serverLoggingService.LogEventAsync(embed);
                await RespondAsync($"**User {guildUser.Username} has been kicked out!**", ephemeral: true);

            }
            catch (Exception ex)
            {
                var commandUser = Context.User;
                await _serverLoggingService.LogErrorASync("Kick user command failed", commandUser.Username, ex);
                await RespondAsync("**Failed to kick user!\n Check the logs channel to more infos**", ephemeral: true);
            }         
        }
    }

}
