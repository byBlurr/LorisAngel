using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using Discord.WebSocket;
using LorisAngel.Utility;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LorisAngel
{
    class LCommandHandler : CommandHandler
    {
        public override void RegisterCommands(List<BotCommand> commands)
        {
            commands.Clear();

            // General commands
            //commands.Add(new BotCommand("help", "help", "Get help using Lori's Angel.", CommandCategory.BotRelated));
            commands.Add(new BotCommand("invite", "invite", "Receive the invite link to add LorisAngel to your server.", CommandCategory.BotRelated));
            commands.Add(new BotCommand("users", "users", "Check how many guilds the bot is in and how many total users.", CommandCategory.BotRelated));
            commands.Add(new BotCommand("uptime", "uptime", "Check how long the bot has been live since last restart.", CommandCategory.BotRelated));
            //commands.Add(new BotCommand("settings", "settings", "Adjust the bots settings for this guild.", CommandCategory.BotRelated));
            
            // User commands (All to be written from scratch)
            //commands.Add(new BotCommand("profile", "profile <@user>", "View the users profile.", CommandCategory.User));
            commands.Add(new BotCommand("av", "av <@user>", "View the users profile picture.", CommandCategory.User));

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

            // Moderation commands

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
            await bot.SetStatusAsync(UserStatus.Online);
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
    }
}