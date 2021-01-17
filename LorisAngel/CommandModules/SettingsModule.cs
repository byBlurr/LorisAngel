using Discord;
using Discord.Commands;
using Discord.Net.Bot.Database.Configs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class SettingsModule : ModuleBase
    {
        [Command("settings")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task SettingsAsync()
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Modify Guild Settings",
                Description = "Use the command `" + gconf.Prefix + " <setting> <newSetting>` to adjust settings. (Example: `" + gconf.Prefix + "settings prefix :` to set the command prefix to ':'`",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = "These settings will later be moved to a webpanel." }
            };

            embed.AddField(new EmbedFieldBuilder() { Name = "Prefix", Value = gconf.Prefix, IsInline = true});
            embed.AddField(new EmbedFieldBuilder() { Name = "Censor", Value = gconf.Censor, IsInline = true});
        }

        [Command("settings prefix")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task PrefixAsync(string prefix)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);
            gconf.Prefix = prefix;
            conf.Save();

            await Context.Channel.SendMessageAsync($"The prefix for the bot in this server has been successfully changed to ''{prefix}''.");
        }

        [Command("settings censor")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task CensorAsync(string censor)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);

            char c = censor.ToLower()[0];
            if (c == 't')
            {
                gconf.Censor = true;
                conf.Save();
                await Context.Channel.SendMessageAsync($"The message censor feature has been enabled in this guild.");
            }
            else if (c == 'f')
            {
                gconf.Censor = false;
                conf.Save();
                await Context.Channel.SendMessageAsync($"The message censor feature has been disabled in this guild.");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Incorrect use of command: Please use the command as follows `" + gconf.Prefix + "settings censor true` or `" + gconf.Prefix + "settings censor false`");
            }
        }
    }
}
