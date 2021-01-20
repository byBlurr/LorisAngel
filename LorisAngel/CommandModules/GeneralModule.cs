using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using LorisAngel.Utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class GeneralModule : ModuleBase
    {
        [Command("help")]
        [Alias("?")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task HelpAsync(string c = null, int page = 1)
        {
            EmbedBuilder embed = Help.GetCommandHelp(Context.Guild.Id, c, page);
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("invite")]
        [Alias("inv")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task InviteAsync()
        {
            await Context.Message.DeleteAsync();
            await Context.User.SendMessageAsync($"Invite Loris Angel to your server: {Util.GetInviteLink(729696788097007717)}");
            await Context.Channel.SendMessageAsync($"Invite Loris Angel to your server: {Util.GetInviteLink(729696788097007717)}");
        }

        [Command("users")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task UsersAsync()
        {
            await Context.Message.DeleteAsync();

            var bot = LCommandHandler.GetBot();
            int guilds = bot.Guilds.Count;
            int users = (await Context.Guild.GetUsersAsync()).Count;
            int total = 0;
            foreach (var guild in bot.Guilds) total += guild.Users.Count;

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Lori's Angels Statistics",
                Color = Color.DarkPurple,
                Description = $"Out of the {guilds} guilds I am watching {total} total users, {users} of which are from this guild!",
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Id}" }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("uptime")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task UptimeAsync()
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            DateTime time = DateTime.UtcNow;
            int minutes = (int)((time - conf.LastStartup).TotalMinutes);
            int uptime = 0;
            string m = "minutes";

            if (minutes >= 60)
            {
                uptime = minutes / 60;
                m = "hours";
            }
            else uptime = minutes;

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Lori's Angels Statistics",
                Color = Color.DarkPurple,
                Description = $"Lori's Angel has been online since {time}, thats an uptime of {uptime} {m}!",
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Id}" }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("changelog")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task ChangelogAsync()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync("```markdown\n" + File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "changelog.txt")) + "\n```");
        }
    }
}
