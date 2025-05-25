using Discord;
using Discord.Interactions;
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
        [SlashCommand("kick", "Kick anyone from the server [Mod Only]")]
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

            await guildUser.KickAsync(modal.Reason);
            await RespondAsync($"User {guildUser.Username} has been kicked out!", ephemeral: true);
        }
    }

}
