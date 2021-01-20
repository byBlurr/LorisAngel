using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Command("biggestguild")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task BiggestGuildAsync()
        {
            await Context.Message.DeleteAsync();

            var guilds = CommandHandler.GetBot().Guilds.ToList().OrderByDescending(x => x.Users.Count);
            string list = "";
            int count = 0;
            foreach (var g in guilds)
            {
                count++;
                if (count <= 10)
                {
                    list += $"\n[{count}] {g.Name} ({g.Id}) - {g.Users.Count} users";
                }
                else
                {
                    break;
                }
            }

            await Context.Channel.SendMessageAsync("Biggest Guilds:" + list);
        }

        [Command("stats")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task StatsAsync(ulong id = 0L)
        {
            SocketGuild guild = (Context.Guild as SocketGuild);
            if (id != 0L && Context.User.Id == 211938243535568896) guild = (CommandHandler.GetBot().GetGuild(id) as SocketGuild);

            await Context.Message.DeleteAsync();

            if (guild != null)
            {
                int userCount = 0;
                int botCount = 0;
                int rolesCount = 0;
                int textCount = 0;
                int voiceCount = 0;

                foreach (var u in guild.Users)
                {
                    if (u.IsBot) botCount++;
                    else userCount++;
                }
                
                rolesCount = guild.Roles.Count;
                textCount = guild.TextChannels.Count;
                voiceCount = guild.VoiceChannels.Count;

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = guild.Name + " (" + guild.Id + ") stats",
                    Description = "Owned by " + guild.Owner.Username + "#" + guild.Owner.Discriminator,
                    Color = Color.DarkPurple
                };
                embed.AddField(new EmbedFieldBuilder() { Name = "Users", Value = userCount, IsInline = true });
                embed.AddField(new EmbedFieldBuilder() { Name = "Bots", Value = botCount, IsInline = true });
                embed.AddField(new EmbedFieldBuilder() { Name = "Roles", Value = rolesCount, IsInline = true });
                embed.AddField(new EmbedFieldBuilder() { Name = "Text Channels", Value = textCount, IsInline = true });
                embed.AddField(new EmbedFieldBuilder() { Name = "Voice Channels", Value = voiceCount, IsInline = true });
                embed.AddField(new EmbedFieldBuilder() { Name = "Created On", Value = guild.CreatedAt.DateTime, IsInline = true });
                embed.AddField(new EmbedFieldBuilder() { Name = "Nitro Level", Value = guild.PremiumTier.ToString(), IsInline = true });
                embed.AddField(new EmbedFieldBuilder() { Name = "Nitro Boosters", Value = guild.PremiumSubscriptionCount, IsInline = true });

                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
            else
            {
                await Util.SendErrorAsync(Context.Channel as ITextChannel, "Error", "An unknown error has occurred", false);
                return;
            }
        }
    }
}
