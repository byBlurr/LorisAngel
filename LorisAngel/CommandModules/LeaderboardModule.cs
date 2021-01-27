﻿using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using LorisAngel.Database;
using LorisAngel.Leaderboards;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class LeaderboardModule : ModuleBase
    {
        [Command("leaderboard")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task LeaderboardAsync([Remainder] string lb = "")
        {
            await Context.Message.DeleteAsync();

            if (lb.Equals(""))
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}leaderboard <game name>`", false);
                return;
            }

            string leaderboard = "";
            if (lb.ToLower().Equals("connect 4") || lb.ToLower().Equals("connect4") || lb.ToLower().Equals("c4")) leaderboard = "Connect 4";
            else if (lb.ToLower().Equals("tic tac toe") || lb.ToLower().Equals("tictactoe") || lb.ToLower().Equals("ttt")) leaderboard = "Tic Tac Toe";
            else
            {
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Leaderboard Error", $"Leaderboard with name '{lb}' could not be found.");
                return;
            }

            Leaderboard fullLb = await LeaderboardDatabase.GetLeaderboardAsync(leaderboard, 10);
            var top10 = fullLb.GetTop(10);
            string board = "";

            for (int i = 0; i < top10.Count; i++)
            {
                board += $"\n[{i+1,3}] {top10[i].Name, 15} - {top10[i].Score}";
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"{leaderboard} - Top 10",
                Color = Color.DarkPurple,
                Description = board,
                Footer = new EmbedFooterBuilder() { Text = fullLb.GetPositionAsString(Context.User.Id) }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
