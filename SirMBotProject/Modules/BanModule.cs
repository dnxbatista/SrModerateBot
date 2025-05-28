using Discord;
using Discord.Interactions;
using SirMBotProject.Services;
using System.Threading.Tasks;

namespace SirMBotProject.Modules
{
    // Build the modal
    public class BanReasonModal : IModal
    {
        public string Title => "Ban Modal";

        [InputLabel("Reason")]
        [ModalTextInput("reason",
        style: TextInputStyle.Paragraph,
        placeholder: "Please provide a detailed reason for banning this user (3-200 characters)...",
        minLength: 3,
        maxLength: 200)]
        public string? Reason { get; set; }

        [InputLabel("Days (0 - 7)")]
        [ModalTextInput("days",
            style: TextInputStyle.Short,
            placeholder: "Number of days to delete messages (0-7)",
            minLength: 1,
            maxLength: 1
            )]
        public string? Days { get; set; }
    }

    // Build command
    public class BanModule : InteractionModuleBase<SocketInteractionContext>
    {
        public readonly ServerLoggingService _serverLoggingService;

        public BanModule(ServerLoggingService serverLoggingService)
        {
            _serverLoggingService = serverLoggingService;
        }

        [SlashCommand("ban", "Ban anyone from the server [Mod Only]")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanCommand(
            [Summary(description: "User thats gonna be banned")] IUser user
            )
        {
            string customId = $"ban_reason_modal:{user.Id}";
            await Context.Interaction.RespondWithModalAsync<BanReasonModal>(customId);
        }

        // Handle modal
        [ModalInteraction("ban_reason_modal:*")]
        public async Task HandleBanReasonModal(string customId, BanReasonModal modal)
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

                int days = 1;
                if (!string.IsNullOrEmpty(modal.Days) && int.TryParse(modal.Days, out var parsedDays))
                {
                    days = Math.Clamp(parsedDays, 0, 7);
                }

                await guildUser.BanAsync(days, modal.Reason);

                // Build log embed
                var embed = new EmbedBuilder()
                    .WithTitle("Ban Log")
                    .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl() ?? Context.Client.CurrentUser.GetDefaultAvatarUrl())
                    .AddField("User Banned:", guildUser.Username)
                    .AddField("Command executed by:", Context.User.Username)
                    .AddField("Reason:", modal.Reason)
                    .AddField("Days to delete messages:", days)
                    .WithColor(Color.Orange)
                    .WithCurrentTimestamp()
                    .Build();

                await _serverLoggingService.LogEventAsync(embed);
                await RespondAsync($"**User {guildUser.Username} has been banned for {days} day(s) of message deletion!**", ephemeral: true);
            }
            catch (Exception ex)
            {
                var commandUser = Context.User;
                await _serverLoggingService.LogErrorASync("Ban user command failed", commandUser.Username, ex);
                await RespondAsync("**Failed to ban user!\n Check the logs channel to more infos**", ephemeral: true);
            }
        }
    }

}
