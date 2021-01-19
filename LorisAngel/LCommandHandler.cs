using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using Discord.WebSocket;
using LorisAngel.Database;
using LorisAngel.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LorisAngel
{
    public class LCommandHandler : CommandHandler
    {
        public const string DATABASE_NAME = "lorisangel";

        public override void RegisterCommands(List<BotCommand> commands)
        {
            commands.Clear();

            // General commands
            //commands.Add(new BotCommand("help", "help", "Get help using Lori's Angel.", CommandCategory.BotRelated));
            commands.Add(new BotCommand("invite", "invite", "Receive the invite link to add LorisAngel to your server.", CommandCategory.BotRelated));
            commands.Add(new BotCommand("users", "users", "Check how many guilds the bot is in and how many total users.", CommandCategory.BotRelated));
            commands.Add(new BotCommand("uptime", "uptime", "Check how long the bot has been live since last restart.", CommandCategory.BotRelated));
            commands.Add(new BotCommand("settings", "settings", "Adjust the bots settings for this guild.", CommandCategory.BotRelated));
            commands.Add(new BotCommand("changelog", "changelog", "View the bots changelog and see what is coming soon.", CommandCategory.BotRelated));
            
            // User commands (All to be written from scratch)
            commands.Add(new BotCommand("profile", "profile <@user>", "View the users profile.", CommandCategory.User));
            commands.Add(new BotCommand("av", "av <@user>", "View the users profile picture.", CommandCategory.User));
            //commands.Add(new BotCommand("requestdata", "requestdata", "Request a copy of your userdata.", CommandCategory.User));

            // Guild commands
            commands.Add(new BotCommand("oldest", "oldest", "Check who has the oldest Discord account in the server.", CommandCategory.Server));
            commands.Add(new BotCommand("region", "region", "Check the region of the server you are currently in.", CommandCategory.Server));

            // Fun commands
            commands.Add(new BotCommand("pickup", "pickup <@user>", "Use a pickup line on the user.", CommandCategory.Fun));
            commands.Add(new BotCommand("kill", "kill <@user>", "Kill the user.", CommandCategory.Fun));
            commands.Add(new BotCommand("roast", "roast <@user>", "Roast the user.", CommandCategory.Fun));
            commands.Add(new BotCommand("joke", "joke", "Have the bot tell a joke.", CommandCategory.Fun));
            commands.Add(new BotCommand("compliment", "compliment <@user>", "Compliment the user.", CommandCategory.Fun));
            commands.Add(new BotCommand("hug", "hug <@user>", "Hug the user.", CommandCategory.Fun));
            commands.Add(new BotCommand("punish", "punish <@user>", "Punish the user.", CommandCategory.Fun));
            commands.Add(new BotCommand("punishme", "punishme <@user>", "Have the user punish you.", CommandCategory.Fun));
            commands.Add(new BotCommand("epic", "epic <@user>", "See the users epic rating.", CommandCategory.Fun));
            commands.Add(new BotCommand("who", "who <question>", "Ask a question such as 'Who is the tallest @Blurr or @Knight?` for the bot to answer.", CommandCategory.Fun));
            commands.Add(new BotCommand("reverse", "reverse <message>", "The bot will mirror the message.", CommandCategory.Fun));
            commands.Add(new BotCommand("binary", "binary <message>", "The bot will convert the message into binary.", CommandCategory.Fun));
            commands.Add(new BotCommand("8ball", "8ball <question>", "Ask the bot a yes/no question.", CommandCategory.Fun));
            commands.Add(new BotCommand("dice", "dice <amount>", "Roll a 6 sided dice or multiple.", CommandCategory.Fun));
            commands.Add(new BotCommand("ship", "ship <@user> or ship <@user1> <@user2>", "Check your compatibility together.", CommandCategory.Fun));

            // Moderation commands
            // Not to be added until a webpanel is up
        }

        public override void SetupHandlers(DiscordSocketClient bot)
        {
            bot.Ready += ReadyAsync;
            bot.JoinedGuild += JoinedGuildAsync;
            bot.LeftGuild += LeftGuildAsync;
            bot.MessageReceived += CensorMessageAsync;
        }

        private async Task ReadyAsync()
        {
            PickupsFile.Exists();
            DeathsFile.Exists();
            RoastsFile.Exists();
            JokesFile.Exists();
            ComplimentsFile.Exists();
            PunishFile.Exists();

            await bot.SetStatusAsync(UserStatus.Online);

            var status = Task.Run(async () => {
                int i = 0;
                while (true)
                {
                    switch (i)
                    {
                        case 0:
                            await bot.SetGameAsync($"use -invite {Util.GetRandomHeartEmoji()}", type: ActivityType.Playing);
                            i++;
                            break;
                        case 1:
                            await bot.SetGameAsync($"Lori's Angel v2! { Util.GetRandomHeartEmoji()}", type: ActivityType.Playing);
                            i++;
                            break;
                        case 2:
                            await bot.SetGameAsync($"use -changelog", type: ActivityType.Playing);
                            i++;
                            break;
                        default:
                            {
                                Random rnd = new Random();
                                BotConfig conf = BotConfig.Load();
                                int j = rnd.Next(0, conf.Commands.Count);

                                await bot.SetGameAsync($"try -{conf.Commands[j].Handle.ToLower()} {Util.GetRandomHeartEmoji()}", type: ActivityType.Playing);
                                i = 0;
                                break;
                            }
                    }
                    await Task.Delay(15000);
                }
            });

            await ProfileDatabase.ProcessUsers();
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

            var sg = bot.GetGuild(730573219374825523);
            await sg.GetTextChannel(739308321655226469).SendMessageAsync("[" + bot.Guilds.Count + "] Joined guild " + guild.Name);
        }

        private async Task LeftGuildAsync(SocketGuild guild)
        {
            var sg = bot.GetGuild(730573219374825523);
            await sg.GetTextChannel(739308321655226469).SendMessageAsync("[" + bot.Guilds.Count + "] Left guild " + guild.Name);
        }

        private async Task CensorMessageAsync(SocketMessage message)
        {
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