using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class GuildModule : ModuleBase
    {
        [Command("oldest")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task OldestAsync()
        {
            await Context.Message.DeleteAsync();
            IUser oldest = null;

            foreach (var user in await Context.Guild.GetUsersAsync())
            {
                if (!user.IsBot)
                {
                    if (oldest == null) oldest = user;
                    else if (oldest.CreatedAt.Date > user.CreatedAt.Date)
                    {
                        oldest = user;
                    }
                }
            }

            if (oldest != null)
            {
                EmbedBuilder embed = new EmbedBuilder() { };
                embed.WithAuthor(new EmbedAuthorBuilder() { Name = oldest.Username + "#" + oldest.Discriminator, IconUrl = oldest.GetAvatarUrl() });
                embed.WithDescription($"The oldest account in the server, first registered {oldest.CreatedAt.Date}!");
                embed.WithColor(Color.DarkPurple);
                embed.Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

        [Command("region")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task RegionAsync()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync("Guild Region: " + Util.ToUppercaseFirst(Context.Guild.VoiceRegionId));
        }
    }
}
