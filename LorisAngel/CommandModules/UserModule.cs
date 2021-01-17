using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class UserModule : ModuleBase
    {
        [Command("avatar")]
        [Alias("av")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task AvatarAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (user == null) user = Context.User as IUser;
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

            IUser user = await Context.Guild.GetUserAsync(userid) as IUser;
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
