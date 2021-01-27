using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class LeaderboardModule : ModuleBase
    {
        [Command("leaderboard")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task LeaderboardAsync([Remainder] string lb = "")
        {
            await Context.Message.DeleteAsync();
        }
    }
}
