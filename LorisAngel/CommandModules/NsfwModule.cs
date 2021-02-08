using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using LorisAngel.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class NsfwModule : ModuleBase
    {
        [Command("punish")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task PunishAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (!(Context.Channel as ITextChannel).IsNsfw)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "NSFW Error", $"This command can only be used inside channels marked as nsfw...", false);
                return;
            }

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}punish <@user>`", false);
                return;
            }

            Random rnd = new Random();
            List<FunObject> punishments = await FunDatabase.GetOfTypeAsync("punish");
            int r = rnd.Next(0, punishments.Count);

            string punishment = punishments[r].Text.Replace("USER1", StringUtil.ToUppercaseFirst(Context.User.Mention)).Replace("USER2", StringUtil.ToUppercaseFirst(user.Mention));

            if (punishment.Contains("RUSER"))
            {
                var users = await Context.Guild.GetUsersAsync();
                if (users.Count > 3)
                {
                    IGuildUser ruser = null;

                    while (ruser == null)
                    {
                        int u = rnd.Next(0, users.Count);
                        IGuildUser rndUser = users.ToArray()[u];

                        if (Context.User.Id != rndUser.Id && user.Id != rndUser.Id) ruser = rndUser;
                    }

                    punishment = punishment.Replace("RUSER", StringUtil.ToUppercaseFirst(ruser.Mention));
                }
                else
                {
                    punishment = punishments[0].Text.Replace("USER1", StringUtil.ToUppercaseFirst(Context.User.Mention)).Replace("USER2", StringUtil.ToUppercaseFirst(user.Mention));
                }
            }

            await Context.Channel.SendMessageAsync(punishment);
        }

        [Command("punishme")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task PunishMeAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (!(Context.Channel as ITextChannel).IsNsfw)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "NSFW Error", $"This command can only be used inside channels marked as nsfw...", false);
                return;
            }

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}punishme <@user>`", false);
                return;
            }

            Random rnd = new Random();
            List<FunObject> punishments = await FunDatabase.GetOfTypeAsync("punish");
            int r = rnd.Next(0, punishments.Count);

            string punishment = punishments[r].Text.Replace("USER1", StringUtil.ToUppercaseFirst(user.Mention)).Replace("USER2", StringUtil.ToUppercaseFirst(Context.User.Mention));

            await Context.Channel.SendMessageAsync(punishment);
        }

        [Command("tie")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task TieAsync(IUser user = null, [Remainder] string obj = null)
        {
            await Context.Message.DeleteAsync();

            if (!(Context.Channel as ITextChannel).IsNsfw)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "NSFW Error", $"This command can only be used inside channels marked as nsfw...", false);
                return;
            }

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}tie <@user>` or `{gconf.Prefix}tie <@user> <object>`", false);
                return;
            }

            if (obj == null) obj = "the bed";
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} tied {user.Mention} to {obj}!");
        }

        [Command("choke")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task ChokeAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (!(Context.Channel as ITextChannel).IsNsfw)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "NSFW Error", $"This command can only be used inside channels marked as nsfw...", false);
                return;
            }

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}choke <@user>`", false);
                return;
            }

            await Context.Channel.SendMessageAsync($"{Context.User.Mention} choked {user.Mention}!");
        }

        [Command("cuck")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task CuckAsync(IUser user = null, IUser person = null)
        {
            await Context.Message.DeleteAsync();

            if (!(Context.Channel as ITextChannel).IsNsfw)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "NSFW Error", $"This command can only be used inside channels marked as nsfw...", false);
                return;
            }

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}cuck <@user>` or `{gconf.Prefix}cuck <@user> <@secondUser>`", false);
                return;
            }

            string with = "";
            if (person != null) with = $" with {person.Mention}"; 
            await Context.Channel.SendMessageAsync($"{Context.User.Mention} cucked {user.Mention}{with}!");
        }

        [Command("peg")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task PegAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (!(Context.Channel as ITextChannel).IsNsfw)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "NSFW Error", $"This command can only be used inside channels marked as nsfw...", false);
                return;
            }

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}peg <@user>`", false);
                return;
            }

            await Context.Channel.SendMessageAsync($"{Context.User.Mention} pegged {user.Mention}!");
        }
    }
}
