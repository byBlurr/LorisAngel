using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using LorisAngel.Database;
using LorisAngel.Games;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class GamesModule : ModuleBase
    {
        [Command("connect4")]
        [Alias("c4")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(ChannelPermission.AttachFiles)]
        private async Task ConnectAsync(IUser playerTwo = null)
        {
            await Context.Message.DeleteAsync();

            if (playerTwo == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}c4 <@user>`", false);
                return;
            }

            IUser playerOne = Context.User as IUser;

            if (playerTwo.IsBot || playerOne.Id == playerTwo.Id)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Connect4 Error", "You can not play against yourself or a bot.", false);
                return;
            }

            if (GameHandler.DoesGameExist(Context.Guild.Id, GameType.CONNECT))
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Connect4 Error", "There is already a game in this guild.", false);
                return;
            }

            ulong[] players = { playerOne.Id, playerTwo.Id };
            ConnectGame newGame = new ConnectGame(Context.Guild.Id, players);
            GameHandler.AddNewGame(newGame);

            string render = newGame.RenderGame();
            IUser currentPlayer = await Context.Guild.GetUserAsync(newGame.Players[newGame.Turn]);
            var msg = await Context.Channel.SendFileAsync(render, $"**Connect 4**\n" +
                $"Next Up: {currentPlayer.Mention}\n" +
                $"`{CommandHandler.GetPrefix(Context.Guild.Id)}c4 <column>` to take your turn\n`{CommandHandler.GetPrefix(Context.Guild.Id)}c4 end` to end the game");

            newGame.RenderId = msg.Id;
        }


        [Command("connect4")]
        [Alias("c4")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(ChannelPermission.AttachFiles)]
        private async Task ConnectTurnAsunc(int column = -1)
        {
            await Context.Message.DeleteAsync();

            if (column < 1 || column > 7)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}c4 <column>` - Column must be 1 - 7", false);
                return;
            }

            if (!GameHandler.DoesGameExist(Context.Guild.Id, GameType.CONNECT))
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Connect4 Error", "There is not a game in this guild.", false);
                return;
            }

            ConnectGame game = (ConnectGame)GameHandler.GetGame(Context.Guild.Id, GameType.CONNECT);
            if (game == null)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Connect4 Error", "The game could not be found.", false);
                return;
            }

            if (game.Players[0] != Context.User.Id && game.Players[1] != Context.User.Id)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Connect4 Error", "You are not part of this game...");
                return;
            }

            if (game.Players[game.Turn] != Context.User.Id)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Connect4 Error", "It is not your turn...");
                return;
            }

            GameHandler.TakeTurn(Context.Guild.Id, GameType.CONNECT, Context.User.Id, column);
            ulong winner = GameHandler.CheckForWinner(Context.Guild.Id, GameType.CONNECT);
            bool draw = GameHandler.CheckForDraw(Context.Guild.Id, GameType.CONNECT);

            IMessage oldMsg = await Context.Channel.GetMessageAsync(game.RenderId);
            await oldMsg.DeleteAsync();

            if (winner == 0L)
            {
                if (!draw)
                {
                    var nextUp = await Context.Guild.GetUserAsync(game.Players[game.Turn]);
                    string render = game.RenderGame();
                    var msg = await Context.Channel.SendFileAsync(render, $"**Connect 4**\n" +
                        $"Next Up: {nextUp.Mention}\n" +
                        $"`{CommandHandler.GetPrefix(Context.Guild.Id)}c4 <column>` to take your turn\n`{CommandHandler.GetPrefix(Context.Guild.Id)}c4 end` to end the game");

                    game.RenderId = msg.Id;
                }
                else
                {
                    string render = game.RenderGame();
                    await Context.Channel.SendFileAsync(render, $"**Connect 4**\n" +
                        $"DRAW ({(await Context.Guild.GetUserAsync(game.Players[0])).Mention} v {(await Context.Guild.GetUserAsync(game.Players[1])).Mention})");

                    if (ProfileDatabase.GetUser(game.Players[0]) != null) ProfileDatabase.AddCurrency(game.Players[0], 100);
                    if (ProfileDatabase.GetUser(game.Players[1]) != null) ProfileDatabase.AddCurrency(game.Players[1], 100);
                    GameHandler.EndGame(game);

                }
            }
            else
            {
                string render = game.RenderGame();
                await Context.Channel.SendFileAsync(render, $"**Connect 4**\n" +
                    $"Game Won by " + (await Context.Guild.GetUserAsync(winner)).Mention);

                if (ProfileDatabase.GetUser(winner != null) ProfileDatabase.AddCurrency(winner, 250);
                GameHandler.EndGame(game);
            }
        }

        [Command("connect4 end")]
        [Alias("c4 end")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(ChannelPermission.AttachFiles)]
        private async Task ConnectEndAsync()
        {
            await Context.Message.DeleteAsync();

            if (GameHandler.DoesGameExist(Context.Guild.Id, GameType.CONNECT))
            {
                Games.Game game = GameHandler.GetGame(Context.Guild.Id, GameType.CONNECT);

                if (game != null)
                {
                    if (game.Players[0] == Context.User.Id || game.Players[1] == Context.User.Id || (Context.User as IGuildUser).GuildPermissions.Administrator)
                    {
                        IMessage msg = await Context.Channel.GetMessageAsync(game.RenderId);
                        await msg.DeleteAsync();

                        string render = game.RenderGame();
                        await Context.Channel.SendFileAsync(render, $"**Connect 4**\n" +
                            $"Game Ended By " + Context.User.Mention);

                        GameHandler.EndGame(game);
                    }
                }
                else
                {
                    await Util.SendErrorAsync((Context.Channel as ITextChannel), "Connect4 Error", "No game could be found here...", false);
                    return;
                }
            }
            else
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Connect4 Error", "No game could be found here...", false);
                return;
            }
        }

        [Command("tictactoe")]
        [Alias("ttt")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(ChannelPermission.AttachFiles)]
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

            if (playerTwo.IsBot || playerOne.Id == playerTwo.Id)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "TicTacToe Error", "You can not play against yourself or a bot.", false);
                return;
            }

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
        [RequireBotPermission(ChannelPermission.AttachFiles)]
        private async Task TicTacToeTurnAsync(int x = -1, int y = -1)
        {
            await Context.Message.DeleteAsync();

            if (!GameHandler.DoesGameExist(Context.Guild.Id, GameType.TICTACTOE))
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "TicTacToe Error", "No game could be found here...");
                return;
            }

            TicTacToeGame game = (TicTacToeGame) GameHandler.GetGame(Context.Guild.Id, GameType.TICTACTOE);

            if (game == null)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "TicTacToe Error", "No game could be found here...");
                return;
            }

            if (game.Players[0] != Context.User.Id && game.Players[1] != Context.User.Id)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "TicTacToe Error", "You are not part of this game...");
                return;
            }

            if (game.Players[game.Turn] != Context.User.Id)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "TicTacToe Error", "It is not your turn...");
                return;
            }

            if (x <= 0 || y <= 0 || x > 3 || y > 3)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "TicTacToe Error", "You need to choose and x and y between of 1, 2 or 3...");
                return;
            }

            GameHandler.TakeTurn(Context.Guild.Id, GameType.TICTACTOE, Context.User.Id, x, y);
            ulong winner = GameHandler.CheckForWinner(Context.Guild.Id, GameType.TICTACTOE);

            if (winner == 0L)
            {
                var oldMsg = await Context.Channel.GetMessageAsync(game.RenderId);
                await oldMsg.DeleteAsync();

                var nextUp = await Context.Guild.GetUserAsync(game.Players[game.Turn]);
                string render = game.RenderGame();
                var msg = await Context.Channel.SendFileAsync(render, $"**TicTacToe**\n" +
                    $"Next Up: {nextUp.Mention}\n" +
                    $"`{CommandHandler.GetPrefix(Context.Guild.Id)}t <x> <y>` to take your turn\n`{CommandHandler.GetPrefix(Context.Guild.Id)}t end` to end the game");

                game.RenderId = msg.Id;
            }
            else
            {
                IMessage msg = await Context.Channel.GetMessageAsync(game.RenderId);
                await msg.DeleteAsync();

                string render = game.RenderGame();
                await Context.Channel.SendFileAsync(render, $"**TicTacToe**\n" +
                    $"Game Won by " + (await Context.Guild.GetUserAsync(winner)).Mention);

                if (ProfileDatabase.GetUser(winner) != null) ProfileDatabase.AddCurrency(winner, 100);
                GameHandler.EndGame(game);
            }
        }

        [Command("t end")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(ChannelPermission.AttachFiles)]
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
                        IMessage msg = await Context.Channel.GetMessageAsync(game.RenderId);
                        await msg.DeleteAsync();

                        string render = game.RenderGame();
                        await Context.Channel.SendFileAsync(render, $"**TicTacToe**\n" +
                            $"Game Ended By " + Context.User.Mention);

                        GameHandler.EndGame(game);
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
        [RequireBotPermission(ChannelPermission.AttachFiles)]
        private async Task SnakesAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            IndividualConfig gconf = conf.GetConfig(Context.Guild.Id);
            await Util.SendErrorAsync((Context.Channel as ITextChannel), "Unimplemented Game", $"This game has not yet been reimplemented into Lori's Angel v2. Try again in a couple days!\n `{gconf.Prefix}changelog` for more information", false);
        }
    }
}
