using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using LorisAngel.Games;
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
        private async Task TicTacToeAsync(IUser playerTwo = null)
        {
            await Context.Message.DeleteAsync();

            if (playerTwo == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}ttt <@user>`", false);
                return;
            }

            IUser playerOne = Context.User as IUser;

            /**if (playerTwo.IsBot || playerOne.Id == playerTwo.Id)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "TicTacToe Error", "You can not play against yourself or a bot.", false);
                return;
            }*/

            if (GameHandler.DoesGameExist(Context.Guild.Id, GameType.TICTACTOE))
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "TicTacToe Error", "There is already a game in this guild.", false);
                return;
            }

            ulong[] players = { playerOne.Id, playerTwo.Id };
            TicTacToeGame newGame = new TicTacToeGame(Context.Guild.Id, players);
            GameHandler.AddNewGame(newGame);

            string render = newGame.RenderGame();
            var msg = await Context.Channel.SendFileAsync(render, $"**TicTacToe**\n" +
                $"Next Up: {playerOne.Mention}\n" +
                $"`{CommandHandler.GetPrefix(Context.Guild.Id)}t <x> <y>` to take your turn\n`{CommandHandler.GetPrefix(Context.Guild.Id)}t end` to end the game");

            newGame.RenderId = msg.Id;
        }

        [Command("t")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task TicTacToeTurnAsync(int x = -1, int y = -1)
        {
            await Context.Message.DeleteAsync();

        }

        [Command("t end")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task TicTacToeEndAsync()
        {
            await Context.Message.DeleteAsync();

            if (GameHandler.DoesGameExist(Context.Guild.Id, GameType.TICTACTOE))
            {
                Games.Game game = GameHandler.GetGame(Context.Guild.Id, GameType.TICTACTOE);

                if (game != null)
                {
                    if (game.Players[0] == Context.User.Id || game.Players[1] == Context.User.Id || (Context.User as IGuildUser).GuildPermissions.Administrator)
                    {
                        GameHandler.EndGame(game);

                        EmbedBuilder embed = new EmbedBuilder()
                        {
                            Title = "TicTacToe",
                            Description = "The game was ended.",
                            Color = Color.DarkPurple
                        };

                        await Context.Channel.SendMessageAsync(null, false, embed.Build());
                    }
                }
                else
                {
                    await Util.SendErrorAsync((Context.Channel as ITextChannel), "TicTacToe Error", "No game could be found here...", false);
                    return;
                }
            }
            else
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "TicTacToe Error", "No game could be found here...", false);
                return;
            }
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
