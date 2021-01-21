using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using LorisAngel.Database;
using System;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class UserModule : ModuleBase
    {
        [Command("profile")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task ProfileAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();
            if (user == null) await ViewProfileAsync(Context, Context.User);
            else await ViewProfileAsync(Context, user);
        }

        [Command("profile")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task ProfileAsync(ulong id)
        {
            await Context.Message.DeleteAsync();

            var user = CommandHandler.GetBot().GetUser(id);

            if (user != null) await ViewProfileAsync(Context, user);
            else
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}profile <@user>`", false);
                return;
            }
        }

        [Command("setmotto")]
        [Alias("motto")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task SetMottoAsync([Remainder] string motto = null)
        {
            await Context.Message.DeleteAsync();

            LoriUser profile = ProfileDatabase.GetUser(Context.User.Id);
            if (profile == null)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Profile Not Found", $"That users profile could not be found?", false);
                return;
            }

            if (motto == null) motto = "";

            ProfileDatabase.SetUserMotto(Context.User.Id, motto);
            await ViewProfileAsync(Context, user);
        }

        private async Task ViewProfileAsync(ICommandContext Context, IUser User)
        {
            if (User.IsBot)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Profile Not Found", $"You can not use this command on bots!", false);
                return;
            }

            LoriUser profile = ProfileDatabase.GetUser(User.Id);
            if (profile == null)
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Profile Not Found", $"That users profile could not be found?", false);
                return;
            }

            string avatar = User.GetAvatarUrl(size: 2048);
            string status = "**" + User.Status.ToString() + " for ";

            Color color;
            switch (User.Status)
            {
                case UserStatus.Offline:
                    color = Color.LightGrey;
                    break;
                case UserStatus.Online:
                    color = Color.Green;
                    break;
                case UserStatus.Idle:
                    color = Color.Orange;
                    break;
                case UserStatus.AFK:
                    color = Color.Orange;
                    break;
                case UserStatus.DoNotDisturb:
                    color = Color.Red;
                    break;
                case UserStatus.Invisible:
                    color = Color.LightGrey;
                    break;
                default:
                    color = Color.LightGrey;
                    break;
            }

            DateTime now = DateTime.Now;
            int seconds = (int)((now - profile.LastSeen).TotalSeconds);
            int minutes = (int)((now - profile.LastSeen).TotalMinutes);
            int hours = (int)((now - profile.LastSeen).TotalHours);
            int days = (int)((now - profile.LastSeen).TotalDays);

            if (days > 0) status += $"{days} Days and {hours - (days * 24)} Hours**";
            else if (hours > 0) status += $"{hours} Hours and {minutes - (hours * 60)} Minutes**";
            else if (minutes > 0) status += $"{minutes} Minutes and {seconds - (minutes * 60)} Seconds**";
            else status += $"{seconds} Seconds**";

            if (User.Activity != null) status += $"\n {User.Activity.ToString()}";

            EmbedBuilder embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder() { IconUrl = avatar, Name = $"{User.Username}#{User.Discriminator}" },
                Description = status,
                Color = color,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  This is a temporary look for profiles..." },
            };

            embed.AddField(new EmbedFieldBuilder() { Name = "Account Created On: ", Value = profile.CreatedOn.ToShortDateString(), IsInline = true });
            embed.AddField(new EmbedFieldBuilder() { Name = "Profile Created On: ", Value = profile.JoinedOn.ToShortDateString(), IsInline = true });
            embed.AddField(new EmbedFieldBuilder() { Name = "Last Updated On: ", Value = profile.LastUpdated.ToShortDateString(), IsInline = true });
            embed.AddField(new EmbedFieldBuilder() { Name = "Unique Identifier: ", Value = profile.Id, IsInline = true });
            embed.AddField(new EmbedFieldBuilder() { Name = "Username: ", Value = User.Username + "#" + User.Discriminator, IsInline = true });
            embed.AddField(new EmbedFieldBuilder() { Name = "Lori's Angel Guilds: ", Value = LCommandHandler.GetUserGuildCount(User.Id), IsInline = true });

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("avatar")]
        [Alias("av")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task AvatarAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (user == null) user = (Context.User as IUser);
            string avatar = user.GetAvatarUrl(size: 2048);

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"{user.Username}#{user.Discriminator}",
                Color = Color.DarkPurple,
                ImageUrl = avatar,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." },
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("avatar")]
        [Alias("av")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task AvatarAsync(ulong userid)
        {
            await Context.Message.DeleteAsync();

            IUser user = (await Context.Guild.GetUserAsync(userid) as IUser);
            if (user == null)
            {
                foreach (var guild in CommandHandler.GetBot().Guilds)
                {
                    if (await (guild as IGuild).GetUserAsync(userid) as IUser != null)
                    {
                        user = await (guild as IGuild).GetUserAsync(userid) as IUser;
                    }
                }
            }

            if (user == null) user = (Context.User as IUser);

            string avatar = user.GetAvatarUrl(size: 2048);
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"{user.Username}#{user.Discriminator}",
                Color = Color.DarkPurple,
                ImageUrl = avatar,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." },
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
