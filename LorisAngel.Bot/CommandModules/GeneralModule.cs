﻿using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using LorisAngel.Common;
using LorisAngel.Bot.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LorisAngel.Bot.CommandModules
{
    public class GeneralModule : ModuleBase
    {
        [Command("help")]
        [Alias("?")]
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
            await Context.User.SendMessageAsync($"Invite Loris Angel to your server: {Utility.GetBotInviteLink()}");
            await Context.Channel.SendMessageAsync($"Invite Loris Angel to your server: {Utility.GetBotInviteLink()}");
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
                Footer = new EmbedFooterBuilder() { Text = $"{EmojiUtil.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Id}" }
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
                Footer = new EmbedFooterBuilder() { Text = $"{EmojiUtil.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Id}" }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("changelog")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task ChangelogAsync()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync("```markdown\n" + File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "changelog.txt")) + "\n\nFull Changelog: https://raw.githubusercontent.com/byBlurr/LorisAngel/master/changelog.md \n```");
        }

        [Command("webpanel")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task WebpanelAsync()
        {
            await Context.Message.DeleteAsync();

            EmbedBuilder embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder() { Name = "Access Webpanel Here", Url = "https://www.bored.com/" },
                Description = "The webpanel has not yet been implemented. Once implemented you will be able to set server settings (such as toggle moderation commands), set user preferences (such as what data can be shown) and userdata requests (such as request data to be removed)."
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("messageowner")]
        private async Task MessageOwnerAsync([Remainder] string message = null)
        {
            if (message == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}messageowner <message>`", false);
                return;
            }

            var logChannel = LCommandHandler.GetBot().GetGuild(730573219374825523).GetTextChannel(808283416533270549);
            await logChannel.SendMessageAsync($"-- -- -- -- --\n**From:** {Context.User.Username}#{Context.User.Discriminator} ({Context.User.Id})\n**Time:** {DateTime.Now}\n\n{message}\n-- -- -- -- --");
            await Context.Channel.SendMessageAsync("Message sent.");
        }
    }
}
