using Discord;
using Discord.Interactions;
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

        [InputLabel("Days (0 - 31")]
        [ModalTextInput("days",
            style: TextInputStyle.Short,
            placeholder: "Number of days to delete messages (0-7)",
            minLength: 1,
            maxLength: 2
            )]
        public string? Days { get; set; }
    }

    // Build command
    public class BanModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ban", "Ban anyone from the server [Mod Only]")]
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
            var userIdStr = customId.Split(":").Last();
            if (!ulong.TryParse(userIdStr, out var userId))
            {
                await RespondAsync("Error to process user", ephemeral: true);
                return;
            }

            var guildUser = Context.Guild.GetUser(userId);
            if(guildUser == null)
            {
                await RespondAsync("User not found!", ephemeral: true);
                return;
            }

            int days = 1;
            if (!string.IsNullOrEmpty(modal.Days) && int.TryParse(modal.Days, out var parsedDays))
            {
                days = Math.Clamp(parsedDays, 0, 31);
            }

            await guildUser.BanAsync(days, modal.Reason);
            await RespondAsync($"User {guildUser.Username} has been banned for {days} day(s) of message deletion!", ephemeral: true);
        }
    }

}
