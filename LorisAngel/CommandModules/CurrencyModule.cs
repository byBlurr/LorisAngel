using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using LorisAngel.Database;
using System;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class CurrencyModule : ModuleBase
    {
        [Command("daily")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task DailyAsync()
        {
            await Context.Message.DeleteAsync();

            LoriUser profile = ProfileDatabase.GetUser(Context.User.Id);
            if (profile == null)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Transfer Error", $"We could not find your bank account.");
                return;
            }

            var tgg = LCommandHandler.GetTopGGClient();
            bool hasVoted = await tgg.HasVoted(Context.User.Id);

            if (hasVoted)
            {
                // check if already claimed
                
            }
            else
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Color.DarkPurple,
                    Author = new EmbedAuthorBuilder() { Name = "Click here to vote!", Url = "https://top.gg/bot/729696788097007717/vote" },
                    Description = $"Vote on TopGG to claim your daily!",
                    Footer = new EmbedFooterBuilder() { Text = "If you can't click above, head to this url https://top.gg/bot/729696788097007717/vote" }
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

        [Command("bank")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task BankAsync()
        {
            await Context.Message.DeleteAsync();

            LoriUser profile = ProfileDatabase.GetUser(Context.User.Id);
            if (profile == null)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Transfer Error", $"We could not find your bank account.");
                return;
            }

            float amount = profile.GetCurrency();
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.DarkPurple,
                Title = "Transfer successful",
                Description = $"Bank balance: ${amount}"
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("bank transfer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task BankTransferAsync(IUser user = null, float amount = 0)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);

            if (user == null)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}bank transfer <@user> <amount>`", false);
                return;
            }

            if (amount <= 0)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Transfer Error", $"The amount must be greater than 0.");
                return;
            }

            LoriUser profile = ProfileDatabase.GetUser(Context.User.Id);
            if (profile == null)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Transfer Error", $"We could not find your bank account.");
                return;
            }

            LoriUser profile2 = ProfileDatabase.GetUser(user.Id);
            if (profile2 == null)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Transfer Error", $"We could not find {user.Username}'s bank account.");
                return;
            }

            if (profile.GetCurrency() >= amount)
            {
                await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Transfer Error", "You can not afford this transfer.");
                return;
            }

            ProfileDatabase.AddCurrency(Context.User.Id, -amount);
            ProfileDatabase.AddCurrency(user.Id, amount);

            float newAmt = profile.GetCurrency();

            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.DarkPurple,
                Title = "Transfer successful",
                Description = $"Successfully transferred ${amount} to {user.Username}.\nNew balance: ${newAmt}"
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
