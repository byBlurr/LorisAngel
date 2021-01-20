using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using LorisAngel.Database;
using System;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class ModeratorModule : ModuleBase
    {
        [Command("kick")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        private async Task KickMemberAsync(IUser user = null, string reason = null)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);

            if (user == null)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}kick <user> <reason>`", false);
                return;
            }

            if (reason == null) reason = "Kicked by " + Context.User.Username + "#" + Context.User.Discriminator;
            else reason += " - Kicked by " + Context.User.Username + "#" + Context.User.Discriminator;

            await KickMemberAsync(user, reason);

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = user.Username + " was kicked",
                Description = reason,
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Edit moderation settings on the webpanel." }
            };

            await CreateLogAsync(gconf, embed);
        }

        [Command("ban")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        private async Task BanMemberAsync(IUser user = null, string reason = null)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);

            if (user == null)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}ban <user> <reason>`", false);
                return;
            }

            if (reason == null) reason = "Banned by " + Context.User.Username + "#" + Context.User.Discriminator;
            else reason += " - Banned by " + Context.User.Username + "#" + Context.User.Discriminator;

            await BanMemberAsync(user, reason);

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = user.Username + " was banned",
                Description = reason,
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Edit moderation settings on the webpanel." }
            };

            await CreateLogAsync(gconf, embed);
        }

        [Command("tempban")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        private async Task TempBanMemberAsync(IUser user = null, int time = 60, string reason = null)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);

            if (user == null)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}ban <user> <time> [reason]`", false);
                return;
            }

            if (reason == null) reason = "Banned by " + Context.User.Username + "#" + Context.User.Discriminator + " for " + time + " minutes";
            else reason += " - Banned by " + Context.User.Username + "#" + Context.User.Discriminator + " for " + time + " minutes";

            await BanMemberAsync(user, reason);
            await ModerationDatabase.AddTempBanAsync(Context.Guild.Id, user.Id, DateTime.Now.AddMinutes(time));

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = user.Username + " was temp banned",
                Description = reason,
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Edit moderation settings on the webpanel." }
            };

            await CreateLogAsync(gconf, embed);
        }

        private async Task CreateLogAsync(IndividualConfig gconf, EmbedBuilder embed)
        {
            if (gconf.LogActions && gconf.LogChannel != 0L)
            {
                var logs = await Context.Guild.GetTextChannelAsync(gconf.LogChannel);
                if (logs != null)
                {
                    await logs.SendMessageAsync(null, false, embed.Build());
                }
                else
                {
                    await Util.SendErrorAsync((Context.Channel as ITextChannel), "Log Channel Error", $"The log channel for this guild could not be found. Make sure the logging settings are set correctly on the webpanel.", false);
                }
            }
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
