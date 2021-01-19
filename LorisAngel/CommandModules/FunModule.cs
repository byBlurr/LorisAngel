using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using LorisAngel.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngel.CommandModules
{
    public class FunModule : ModuleBase
    {
        private static readonly string[] YES_REPLIES = { "Yes", "Definitely", "Aye", "All signs point to yes", "You may rely on it", "Do pigs roll in mud?" };
        private static readonly string[] NO_REPLIES = { "No", "Not in a million years", "Keep dreaming", "Absolutely not", "Nay", "My sources say no" };
        private static readonly string[] MAYBE_REPLIES = { "Maybe", "Possibly", "Most probably", "Can not predict right now", "Don't count on it", "Better to not tell you right now", };

        [Command("who")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task WhoAsync([Remainder] string question = null)
        {
            await Context.Message.DeleteAsync();

            if (question == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}who <question>`", false);
                return;
            }

            ulong[] mentions = Context.Message.MentionedUserIds.ToArray<ulong>();

            IGuildUser answerUser;
            if (mentions.Length > 0)
            {
                Random rnd = new Random();
                int answer = rnd.Next(0, mentions.Length);
                answerUser = await Context.Guild.GetUserAsync(mentions[answer]);
            }
            else
            {
                var users = await Context.Guild.GetUsersAsync();
                Random rnd = new Random();
                int answer = rnd.Next(0, users.Count);
                answerUser = users.ToArray()[answer];
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"Who {await Util.GetReadableMentionsAsync(Context.Guild as IGuild, question.ToLower())}",
                Description = $"The answer to that would be {answerUser.Username}.",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("reverse")]
        [Alias("r")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task ReverseAsync([Remainder] string message = null)
        {
            if (message != null)
            {
                await Context.Message.DeleteAsync();
                string newMessage = "";

                foreach (char l in message)
                {
                    newMessage = l + newMessage;
                }

                await Context.Channel.SendMessageAsync(newMessage);
            }
            else
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}reverse <message>`", false);
                return;
            }
        }

        [Command("binary")]
        [Alias("bin")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task BinaryAsync([Remainder] string text = null)
        {
            await Context.Message.DeleteAsync();

            if (text != null)
            {
                var binary = ToBinary(ConvertToByteArray(text, Encoding.ASCII));

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = $"Text to Binary",
                    Description = $"''{text}''\n\n{binary}",
                    Color = Color.DarkPurple
                };
                embed.Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
            else
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}binary <message>`", false);
                return;
            }
        }

        [Command("8ball")]
        [Alias("8b")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task EightBallAsync([Remainder] string question = null)
        {
            await Context.Message.DeleteAsync();


            if (question != null)
            {
                string reply;
                Random rnd = new Random();
                int random = rnd.Next(0, 3);
                switch (random)
                {
                    case 0:
                        int y = rnd.Next(0, YES_REPLIES.Length);
                        reply = YES_REPLIES[y];
                        break;
                    case 1:
                        int n = rnd.Next(0, NO_REPLIES.Length);
                        reply = NO_REPLIES[n];
                        break;
                    default:
                        int m = rnd.Next(0, MAYBE_REPLIES.Length);
                        reply = MAYBE_REPLIES[m];
                        break;
                }

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = question,
                    Description = reply + "!",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." },
                };

                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
            else
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}8ball <question>`", false);
                return;
            }
        }

        [Command("dice")]
        [Alias("die")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task DiceAsync(int amount = 1)
        {
            await Context.Message.DeleteAsync();
            if (amount < 1) amount = 1;
            else if (amount > 100) amount = 100;

            Random rnd = new Random();
            List<int> rolls = new List<int>();
            int total = 0;
            string text = "";

            for (int i = 0; i < amount; i++)
            {
                int roll = rnd.Next(1, 7);
                rolls.Add(roll);
                total += roll;
                text += $"**{roll}**, ";
            }
            text = text.Substring(0, text.Length - 2);

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Dice Roll",
                Description = $"You rolled **{amount}** dice for **{total}**! Dice: [{text}]",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("pickup")]
        [Alias("pickupline", "pickuplines")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task PickupAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}pickup <@user>`", false);
                return;
            }

            Random rnd = new Random();
            List<FunObject> pickups = await FunDatabase.GetOfTypeAsync("pickup");
            int p = rnd.Next(0, pickups.Count);
            string pickup = pickups[p].Text.Replace("USER1", Util.ToUppercaseFirst(Context.User.Mention)).Replace("USER2", Util.ToUppercaseFirst(user.Mention));

            await Context.Channel.SendMessageAsync(pickup);
        }

        [Command("kill")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task KillAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}kill <@user>`", false);
                return;
            }

            Random rnd = new Random();
            List<FunObject> deaths = await FunDatabase.GetOfTypeAsync("deaths");
            int d = rnd.Next(0, deaths.Count);
            string death = deaths[d].Text.Replace("USER1", Util.ToUppercaseFirst(Context.User.Mention)).Replace("USER2", Util.ToUppercaseFirst(user.Mention));

            EmbedBuilder embed = new EmbedBuilder()
            {
                Description = death,
                Footer = new EmbedFooterBuilder() { Text = "❄️  React with a snowflake to mark as too offensive..." },
                Color = Color.DarkPurple
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("roast")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task RoastAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();
            if (user == null) user = Context.User as IUser;

            Random rnd = new Random();
            List<FunObject> roasts = await FunDatabase.GetOfTypeAsync("roast");
            int d = rnd.Next(0, roasts.Count);
            string roast = roasts[d].Text.Replace("USER", Util.ToUppercaseFirst(user.Mention));

            EmbedBuilder embed = new EmbedBuilder()
            {
                Description = roast,
                Footer = new EmbedFooterBuilder() { Text = "❄️  React with a snowflake to mark as too offensive..." },
                Color = Color.DarkPurple
            };

            var msg = await Context.Channel.SendMessageAsync(null, false, embed.Build());
            //await msg.AddReactionAsync(new Discord.Emoji("❄️"));
        }

        [Command("joke")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task JokeAsync()
        {
            await Context.Message.DeleteAsync();

            Random rnd = new Random();
            List<FunObject> jokes = await FunDatabase.GetOfTypeAsync("joke");
            int d = rnd.Next(0, jokes.Count);
            string joke = jokes[d].Text;

            EmbedBuilder embed = new EmbedBuilder()
            {
                Description = joke,
                Footer = new EmbedFooterBuilder() { Text = "❄️  React with a snowflake to mark as too offensive..." },
                Color = Color.DarkPurple
            };

            var msg = await Context.Channel.SendMessageAsync(null, false, embed.Build());
            //await msg.AddReactionAsync(new Discord.Emoji("❄️"));
        }

        [Command("compliment")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task ComplimentsAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();
            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}compliment <@user>`", false);
                return;
            }

            Random rnd = new Random();
            if (Context.User.Id != user.Id)
            {
                List<FunObject> compliments = await FunDatabase.GetOfTypeAsync("compliment");
                int d = rnd.Next(0, compliments.Count);
                string compliment = compliments[d].Text.Replace("USER", Util.ToUppercaseFirst(user.Mention));
                await Context.Channel.SendMessageAsync(compliment);
            }
            else
            {
                List<FunObject> roasts = await FunDatabase.GetOfTypeAsync("roast");
                int d = rnd.Next(0, roasts.Count);
                string roast = roasts[d].Text.Replace("USER", Util.ToUppercaseFirst(user.Mention));
                await Context.Channel.SendMessageAsync(roast);
            }
        }

        [Command("hug")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task HugAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}hug <@user>`", false);
                return;
            }

            List<FunObject> hugs = await FunDatabase.GetOfTypeAsync("hug");
            Random rnd = new Random();
            int g = rnd.Next(0, hugs.Count);
            string GIF = hugs[g].Extra;

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"{Context.User.Username} hugged {user.Username}",
                ImageUrl = GIF,
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.ToString()}" }
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("punish")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task PunishAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}punish <@user>`", false);
                return;
            }

            Random rnd = new Random();
            List<FunObject> punishments = await FunDatabase.GetOfTypeAsync("roast");
            int r = rnd.Next(0, punishments.Count);

            string punishment = punishments[r].Text.Replace("USER1", Util.ToUppercaseFirst(Context.User.Mention)).Replace("USER2", Util.ToUppercaseFirst(user.Mention));

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

                    punishment = punishment.Replace("RUSER", Util.ToUppercaseFirst(ruser.Mention));
                }
                else
                {
                    punishment = punishments[0].Text.Replace("USER1", Util.ToUppercaseFirst(Context.User.Mention)).Replace("USER2", Util.ToUppercaseFirst(user.Mention));
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

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}punishme <@user>`", false);
                return;
            }

            Random rnd = new Random();
            List<FunObject> punishments = await FunDatabase.GetOfTypeAsync("roast");
            int r = rnd.Next(0, punishments.Count);

            string punishment = punishments[r].Text.Replace("USER1", Util.ToUppercaseFirst(user.Mention)).Replace("USER2", Util.ToUppercaseFirst(Context.User.Mention));

            await Context.Channel.SendMessageAsync(punishment);
        }

        [Command("epic")]
        [Alias("rate", "epicrating")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task EpicRatingAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();
            if (user == null) user = Context.User as IUser;

            Random rnd = new Random();
            float rating = (float)(rnd.Next(0, 100) + rnd.NextDouble());

            if (user.IsBot && user.Id != 729696788097007717L) rating = (float)(rnd.Next(0, 20) + rnd.NextDouble());
            else if (user.IsBot && user.Id == 729696788097007717L) rating = (float)(rnd.Next(80, 20) + rnd.NextDouble());

            EmbedBuilder embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder() { IconUrl = user.GetAvatarUrl(), Name = user.Username },
                Description = $"You are {rating}% EPIC!",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }


        [Command("ship")]
        [Alias("relationship", "lovecalc", "lovecalculator")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task ShipAsync(IUser user = null, IUser user1 = null)
        {
            await Context.Message.DeleteAsync();

            if (user == null)
            {
                BotConfig conf = BotConfig.Load();
                var gconf = conf.GetConfig(Context.Guild.Id);
                await Util.SendErrorAsync((Context.Channel as ITextChannel), "Incorrect Command Usage", $"Correct Usage: `{gconf.Prefix}ship <user>` or `{gconf.Prefix}ship <user1> <user2>`", false);
                return;
            }
            if (user1 == null) user1 = (Context.User as IUser);

            string name1 = Util.ToUppercaseFirst(user.Username);
            string name2 = Util.ToUppercaseFirst(user1.Username);
            string title = "";
            string message = "";
            int score = 0;
            string name = "";

            // Check if entry exists in database
            var ship = RelationshipDatabase.DoesExist(user.Id, user1.Id);
            if (ship != null)
            {
                score = ship.Percentage;
                name = ship.Shipname;
            }
            else
            {
                Random rnd = new Random();
                score = rnd.Next(10, 10000);
                name = $"{name1[0].ToString().ToUpper()}{name1[1]}{name2[name2.Length - 3]}{name2[name2.Length - 2]}{name2[name2.Length - 1]}";

                ship = new Relationship(user.Id, user1.Id, name1, name2, name, score);
                RelationshipDatabase.SaveShip(ship);
            }

            float percentage = (float)score / 100f;
            if (percentage > 95f)
            {
                title = $"{name1} 💘 {name2}";
                message = $"I really ship {name}! They're soul mates, a {percentage}% match!";
            }
            else if (percentage > 55f)
            {
                title = $"{name1} ❤️ {name2}";
                message = $"I ship {name}! They get a {percentage}% match!";
            }
            else
            {
                title = $"{name1} 💔 {name2}";
                message = $"Can't say I ship {name}! They get a shitty {percentage}% match!";
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.DarkPurple,
                Title = title,
                Description = message,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." },
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        public static byte[] ConvertToByteArray(string str, Encoding encoding) => encoding.GetBytes(str);
        public static String ToBinary(Byte[] data) => string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
    }
}
