using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class CurrencyModule : ModuleBase
    {
        [Command("bank")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task BankAsync()
        {
            await Context.Message.DeleteAsync();


        }

        [Command("bank transfer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task BankTransferAsync(IUser user = null, int amount = 0)
        {
            await Context.Message.DeleteAsync();


        }
    }
}
