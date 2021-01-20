using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class GamesModule : ModuleBase
    {
        [Command("connect4")]
        [Alias("c4")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task ConnectAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            IndividualConfig gconf = conf.GetConfig(Context.Guild.Id);
            await Util.SendErrorAsync((Context.Channel as ITextChannel), "Unimplemented Game", $"This game has not yet been reimplemented into Lori's Angel v2. Try again in a couple days!\n `{gconf.Prefix}changelog` for more information", false);
        }

        [Command("tictactoe")]
        [Alias("ttt")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task TicTacToeAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            IndividualConfig gconf = conf.GetConfig(Context.Guild.Id);
            await Util.SendErrorAsync((Context.Channel as ITextChannel), "Unimplemented Game", $"This game has not yet been reimplemented into Lori's Angel v2. Try again in a couple days!\n `{gconf.Prefix}changelog` for more information", false);
        }

        [Command("snake")]
        [Alias("snakes", "snakesandladders")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task SnakesAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            IndividualConfig gconf = conf.GetConfig(Context.Guild.Id);
            await Util.SendErrorAsync((Context.Channel as ITextChannel), "Unimplemented Game", $"This game has not yet been reimplemented into Lori's Angel v2. Try again in a couple days!\n `{gconf.Prefix}changelog` for more information", false);
        }
    }
}
