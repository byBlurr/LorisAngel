using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.CommandModules;
using Discord.Net.Bot.Database.Configs;
using Discord.WebSocket;
using LorisAngel.Database;
using LorisAngel.Utility;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LorisAngel
{
    public class LCommandHandler : CommandHandler
    {
        public const string DATABASE_NAME = "lorisangel";

        public override void RegisterCommands(List<BotCommand> commands)
        {
            commands.Clear();

            List<CommandArgument> emptyArguments = new List<CommandArgument>();

            // General commands
            List<CommandArgument> arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.TEXT, true, "botrelated") };
            CommandUsage[] helpUsage = { new CommandUsage("help", arguments) };
            commands.Add(new BotCommand("help", helpUsage, "Get help using Lori's Angel.", CommandCategory.BotRelated));
            CommandUsage[] inviteUsage = { new CommandUsage("invite", emptyArguments) };
            commands.Add(new BotCommand("invite", inviteUsage, "Receive the invite link to add LorisAngel to your server.", CommandCategory.BotRelated));
            CommandUsage[] usersUsage = { new CommandUsage("users", emptyArguments) };
            commands.Add(new BotCommand("users", usersUsage, "Check how many guilds the bot is in and how many total users.", CommandCategory.BotRelated));
            CommandUsage[] uptimeUsage = { new CommandUsage("uptime", emptyArguments) };
            commands.Add(new BotCommand("uptime", uptimeUsage, "Check how long the bot has been live since last restart.", CommandCategory.BotRelated));
            CommandUsage[] settingsUsage = { new CommandUsage("settings", emptyArguments) };
            commands.Add(new BotCommand("settings", settingsUsage, "Adjust the bots settings for this guild.", CommandCategory.BotRelated));
            CommandUsage[] changelogUsage = { new CommandUsage("changelog", emptyArguments) };
            commands.Add(new BotCommand("changelog", changelogUsage, "View the bots changelog and see what is coming soon.", CommandCategory.BotRelated));
            CommandUsage[] webpanelUsage = { new CommandUsage("webpanel", emptyArguments) };
            commands.Add(new BotCommand("webpanel", webpanelUsage, "Help with accessing the webpanel for setting server/user preferences.", CommandCategory.BotRelated));


            // User commands (All to be written from scratch)
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, true) };
            CommandUsage[] profileUsage = { new CommandUsage("profile", arguments) };
            commands.Add(new BotCommand("profile", profileUsage, "View the users profile.", CommandCategory.User));
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.TEXT, true, "Life is like a box of chocolates.") };
            CommandUsage[] mottoUsage = { new CommandUsage("setmotto", arguments), new CommandUsage("motto", arguments) };
            commands.Add(new BotCommand("motto", mottoUsage, "Set a new motto on your profile", CommandCategory.User));
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, true) };
            CommandUsage[] avUsage = { new CommandUsage("av", arguments) };
            commands.Add(new BotCommand("av", avUsage, "View the users profile picture.", CommandCategory.User));

            // Guild commands
            CommandUsage[] oldestUsage = { new CommandUsage("oldest", emptyArguments) };
            commands.Add(new BotCommand("oldest", oldestUsage, "Check who has the oldest Discord account in the server.", CommandCategory.Server));
            CommandUsage[] regionUsage = { new CommandUsage("region", emptyArguments) };
            commands.Add(new BotCommand("region", regionUsage, "Check the region of the server you are currently in.", CommandCategory.Server));
            CommandUsage[] statsUsage = { new CommandUsage("stats", emptyArguments) };
            commands.Add(new BotCommand("stats", statsUsage, "View statistics of this server.", CommandCategory.Server));

            // Games commands
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, false) };
            CommandUsage[] connectUsage = { new CommandUsage("connect4", arguments), new CommandUsage("c4", arguments) };
            commands.Add(new BotCommand("connect4", connectUsage, "Play a game of Connect 4.", CommandCategory.Games));
            CommandUsage[] tttUsage = { new CommandUsage("tictactoe", arguments), new CommandUsage("ttt", arguments) };
            commands.Add(new BotCommand("tictactoe", tttUsage, "Play a game of Tic Tac Toe.", CommandCategory.Games));
            //arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, false), new CommandArgument(CommandArgumentType.USER, true, "@Knight"), new CommandArgument(CommandArgumentType.USER, true, "@Lydia") };
            //CommandUsage[] snakesUsage = { new CommandUsage("snakes", arguments), new CommandUsage("snakesandladders", arguments) };
            //commands.Add(new BotCommand("snakesandladders", snakesUsage, "Play a game of Snakes and Ladders (Minimum of two players).", CommandCategory.Games));

            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.TEXT, false, "ttt") };
            CommandUsage[] lbUsage = { new CommandUsage("leaderboard", arguments), new CommandUsage("lb", arguments) };
            commands.Add(new BotCommand("leaderboard", lbUsage, "View who has the most bragging rights.", CommandCategory.Games));

            // Fun commands
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, false) };
            CommandUsage[] pickupUsage = { new CommandUsage("pickup", arguments) };
            commands.Add(new BotCommand("pickup", pickupUsage, "Use a pickup line on the user.", CommandCategory.Fun));
            CommandUsage[] killUsage = { new CommandUsage("kill", arguments) };
            commands.Add(new BotCommand("kill", killUsage, "Kill the user.", CommandCategory.Fun));
            CommandUsage[] roastUsage = { new CommandUsage("roast", arguments) };
            commands.Add(new BotCommand("roast", roastUsage, "Roast the user.", CommandCategory.Fun));
            CommandUsage[] jokeUsage = { new CommandUsage("joke", emptyArguments) };
            commands.Add(new BotCommand("joke", jokeUsage, "Have the bot tell a joke.", CommandCategory.Fun));
            CommandUsage[] complimentUsage = { new CommandUsage("compliment", arguments) };
            commands.Add(new BotCommand("compliment", complimentUsage, "Compliment the user.", CommandCategory.Fun));
            CommandUsage[] hugUsage = { new CommandUsage("hug", arguments) };
            commands.Add(new BotCommand("hug", hugUsage, "Hug the user.", CommandCategory.Fun));
            CommandUsage[] epicUsage = { new CommandUsage("epic", arguments) };
            commands.Add(new BotCommand("epic", epicUsage, "See the users epic rating.", CommandCategory.Fun));
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.TEXT, false, "Who is the tallest @Blurr or @Knight?") };
            CommandUsage[] whoUsage = { new CommandUsage("who", arguments) };
            commands.Add(new BotCommand("who", whoUsage, "Ask a question for the bot to answer.", CommandCategory.Fun));
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.TEXT, false) };
            CommandUsage[] reverseUsage = { new CommandUsage("reverse", arguments) };
            commands.Add(new BotCommand("reverse", reverseUsage, "The bot will mirror the message.", CommandCategory.Fun));
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.TEXT, false) };
            CommandUsage[] binaryUsage = { new CommandUsage("binary", arguments) };
            commands.Add(new BotCommand("binary", binaryUsage, "The bot will convert the message into binary.", CommandCategory.Fun));
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.TEXT, false, "Is Blurr the best?") };
            CommandUsage[] ballUsage = { new CommandUsage("8ball", arguments) };
            commands.Add(new BotCommand("8ball", ballUsage, "Ask the bot a yes/no/maybe question.", CommandCategory.Fun));
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.NUMBER, true) };
            CommandUsage[] diceUsage = { new CommandUsage("dice", arguments) };
            commands.Add(new BotCommand("dice", diceUsage, "Roll a 6 sided dice or multiple.", CommandCategory.Fun));
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, false), new CommandArgument(CommandArgumentType.USER, true) };
            CommandUsage[] shipUsage = { new CommandUsage("ship", arguments) };
            commands.Add(new BotCommand("ship", shipUsage, "Check your compatibility together.", CommandCategory.Fun));

            // Moderation commands
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, false), new CommandArgument(CommandArgumentType.TEXT, true, "Being mean!") };
            CommandUsage[] kickUsage = { new CommandUsage("kick", arguments) };
            commands.Add(new BotCommand("kick", kickUsage, "Kick a member from the server.", CommandCategory.Moderation));
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, false), new CommandArgument(CommandArgumentType.TEXT, true, "Being mean!") };
            CommandUsage[] banUsage = { new CommandUsage("ban", arguments) };
            commands.Add(new BotCommand("ban", banUsage, "Ban a member from the server.", CommandCategory.Moderation));
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, false), new CommandArgument(CommandArgumentType.NUMBER, false), new CommandArgument(CommandArgumentType.TEXT, true, "Being mean!") };
            CommandUsage[] tempbanUsage = { new CommandUsage("tempban", arguments) };
            commands.Add(new BotCommand("tempban", tempbanUsage, "Temp ban a member from the server.", CommandCategory.Moderation));
            //arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, false), new CommandArgument(CommandArgumentType.TEXT, true, "Being mean!") };
            //CommandUsage[] muteUsage = { new CommandUsage("mute", arguments) };
            //commands.Add(new BotCommand("mute", muteUsage, "Mute a member from the server.", CommandCategory.Moderation));

            // Currency
            arguments = new List<CommandArgument>();
            CommandUsage[] bankUsage = { new CommandUsage("bank", arguments) };
            commands.Add(new BotCommand("bank", bankUsage, "Check your balance.", CommandCategory.Currency));
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, false), new CommandArgument(CommandArgumentType.NUMBER, false, "12.99") };
            CommandUsage[] transferUsage = { new CommandUsage("bank transfer", arguments) };
            commands.Add(new BotCommand("bank transfer", transferUsage, "Transfer some money to another user.", CommandCategory.Currency));

            // NSFW
            arguments = new List<CommandArgument> { new CommandArgument(CommandArgumentType.USER, false) };
            CommandUsage[] punishUsage = { new CommandUsage("punish", arguments) };
            commands.Add(new BotCommand("punish", punishUsage, "Punish the user.", CommandCategory.NSFW));
            CommandUsage[] punishmeUsage = { new CommandUsage("punishme", arguments) };
            commands.Add(new BotCommand("punishme", punishmeUsage, "Have the user punish you.", CommandCategory.NSFW));
        }

        public override void SetupHandlers(DiscordSocketClient bot)
        {
            bot.Ready += ReadyAsync;
            bot.GuildMemberUpdated += UpdateUserAsync;
            bot.JoinedGuild += JoinedGuildAsync;
            bot.UserJoined += UserJoinedAsync;
            bot.LeftGuild += LeftGuildAsync;
            bot.MessageReceived += CensorMessageAsync;
        }

        private async Task ReadyAsync()
        {
            await bot.SetStatusAsync(UserStatus.Online);

            // Clear up any old game renders...
            var clearGames = Task.Run(async () =>
            {
                var files = Directory.GetFiles(Path.Combine("textures", "games"));
                foreach (string p in files)
                {
                    File.Delete(p);
                }
                
            });

            // Set the custom status once a minute
            var status = Task.Run(async () => {
                int i = 0;
                while (true)
                {
                    switch (i)
                    {
                        case 0:
                            await bot.SetGameAsync($"use -invite {EmojiUtil.GetRandomHeartEmoji()}", type: ActivityType.Playing);
                            i++;
                            break;
                        case 1:
                            await bot.SetGameAsync($"Lori's Angel v2! { EmojiUtil.GetRandomHeartEmoji()}", type: ActivityType.Playing);
                            i++;
                            break;
                        case 2:
                            await bot.SetGameAsync($"use -changelog", type: ActivityType.Playing);
                            i++;
                            break;
                        default:
                            {
                                //Random rnd = new Random();
                                //BotConfig conf = BotConfig.Load();
                                // int j = rnd.Next(0, conf.Commands.Count);

                                //await bot.SetGameAsync($"try -{conf.Commands[j].Handle.ToLower()} {Util.GetRandomHeartEmoji()}", type: ActivityType.Playing);
                                i = 0;
                                await bot.SetGameAsync($"use -help", type: ActivityType.Playing);
                                break;
                            }
                    }
                    await Task.Delay(60000);
                }
            });

            await ProfileDatabase.ProcessUsers();
            //await ModerationDatabase.ProcessBansAsync();
        }
        private async Task UpdateUserAsync(SocketGuildUser oldUser, SocketGuildUser updatedUser)
        {
            _ = Task.Run(async () =>
            {
                string old = "";
                string updated = "";
                if (oldUser.Activity != null) old = oldUser.Activity.ToString();
                if (updatedUser.Activity != null) updated = updatedUser.Activity.ToString();

                // if username, status or activity has changed...
                if (oldUser.Status != updatedUser.Status || !old.Equals(updated) || !oldUser.Username.Equals(updatedUser.Username)) 
                    await ProfileDatabase.UpdateUserAsync(oldUser.Id);
            });
        }

        private async Task JoinedGuildAsync(SocketGuild guild)
        {
            if (guild.Owner.Id != 376841246955667459)
            {
                bool left = false;
                foreach (var g in bot.Guilds)
                {
                    if (g.Owner.Id == 376841246955667459 && left == false)
                    {
                        await g.LeaveAsync();
                        left = true;
                    }
                }
            }

            while (!ProfileDatabase.Ready()) await Task.Delay(50);

            var sg = bot.GetGuild(730573219374825523);
            await sg.GetTextChannel(739308321655226469).SendMessageAsync("[" + bot.Guilds.Count + "] Joined guild " + guild.Name);

            foreach (var user in guild.Users)
            {
                if (!ProfileDatabase.DoesUserExistInMemory(user.Id) && !user.IsBot)
                {
                    ProfileDatabase.CreateNewUser((user as IUser));
                }
            }
        }

        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            while (!ProfileDatabase.Ready()) await Task.Delay(100);

            if (!ProfileDatabase.DoesUserExistInMemory(user.Id) && !user.IsBot)
            {
                ProfileDatabase.CreateNewUser((user as IUser));
            }
        }

        private async Task LeftGuildAsync(SocketGuild guild)
        {
            var sg = bot.GetGuild(730573219374825523);
            await sg.GetTextChannel(739308321655226469).SendMessageAsync("[" + bot.Guilds.Count + "] Left guild " + guild.Name);
        }

        private async Task CensorMessageAsync(SocketMessage message)
        {
            if (message == null) return;
            if (message.Author.IsBot) return;

            if (ProfileDatabase.Ready())
            {
                // Check if user offline
                if (!message.Author.IsBot && (message.Author.Status == UserStatus.Offline || message.Author.Status == UserStatus.Invisible))
                {
                    // Mark them as online for a loop, reset their last seen... THEY APPEARING!
                    ProfileDatabase.SetUserOnline(message.Author.Id);
                }
            }

            await Moderation.CheckMessageAsync(message);
        }

        public static int GetUserGuildCount(ulong id)
        {
            int count = 0;
            foreach (var guild in bot.Guilds)
            {
                if (guild.GetUser(id) != null) count++;
            }
            return count;
        }
    }
}