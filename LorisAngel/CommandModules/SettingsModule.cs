using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
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

            string words = "";
            foreach (string word in gconf.CensoredWords)
            {
                words += "," + word;
            }
            embed.AddField(new EmbedFieldBuilder() { Name = "Censored Words", Value = $"{gconf.Prefix}settings addcensor <word>\n{words}" });

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("settings prefix")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task PrefixAsync(string prefix = null)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);

            if (prefix == null)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}settings prefix <prefix>`", false);
                return;
            }

            gconf.Prefix = prefix;
            conf.Save();

            await Context.Channel.SendMessageAsync($"The prefix for the bot in this server has been successfully changed to ''{prefix}''.");
        }

        [Command("settings censor")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task CensorAsync(string censor = null)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);

            if (censor == null)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}settings censor <True/False>`", false);
                return;
            }

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

        [Command("settings addcensor")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task AddCensorAsync(string censor = null)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);

            if (censor == null)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}settings addcensor <word>`", false);
                return;
            }

            string word = censor.ToLower().Trim();
            if (word != string.Empty)
            {
                gconf.AddCensoredWord(word);
                conf.Save();
                await Context.Channel.SendMessageAsync($"The word {word} has been added to the guilds censor.");
            }
            else
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}settings addcensor <word>`", false);
                return;
            }
        }
    }
}
