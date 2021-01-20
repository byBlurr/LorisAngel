using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using System.Collections.Generic;

namespace LorisAngel.Utility
{
    class Help
    {
        public static EmbedBuilder GetCommandHelp(ulong guildId, string c = null, int page = 1)
        {
            BotConfig conf = BotConfig.Load();
            IndividualConfig gconf = conf.GetConfig(guildId);
            List<BotCommand> commands = conf.Commands;
            int pages;

            CommandCategory category = CommandCategory.Main;
            if (c != null)
            {
                c = c.ToLower().Trim();

                switch (c)
                {
                    case "fun":
                        category = CommandCategory.Fun;
                        break;
                    case "user":
                        category = CommandCategory.User;
                        break;
                    case "server":
                        category = CommandCategory.Server;
                        break;
                    case "botrelated":
                        category = CommandCategory.BotRelated;
                        break;
                    case "moderation":
                        category = CommandCategory.Moderation;
                        break;
                    case "games":
                        category = CommandCategory.Games;
                        break;

                    default:
                        category = CommandCategory.Main;
                        break;
                }
            }


            List<CommandCategory> cats = new List<CommandCategory>();
            List<BotCommand> commandsToShow = new List<BotCommand>();

            bool displayCommandInfo = false;
            BotCommand commandToDisplay = null;

            foreach (BotCommand command in commands)
            {
                if (c != null)
                {
                    if (command.Handle.ToLower() == c.ToLower().Trim())
                    {
                        commandToDisplay = command;
                        displayCommandInfo = true;
                        break;
                    }
                }

                if (category == CommandCategory.Main)
                {
                    if (!cats.Contains(command.Category)) cats.Add(command.Category);
                }
                if (command.Category == category)
                {
                    if (!commandsToShow.Contains(command)) commandsToShow.Add(command);
                }
            }

            EmbedBuilder embed;
            if (!displayCommandInfo)
            {
                if (category == CommandCategory.Main) pages = (cats.Count / 10);
                else pages = (commandsToShow.Count / 8);

                if (page > pages) page = pages;

                embed = new EmbedBuilder()
                {
                    Title = "Help: " + Util.ToUppercaseFirst(category.ToString()),
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Page {page} of {pages}" }
                };

                if (category == CommandCategory.Main)
                {
                    embed.Description = $"For further help use `{gconf.Prefix}help <commandCategory>`, for example `{gconf.Prefix}help fun`";

                    foreach (var cat in cats)
                    {
                        embed.AddField(new EmbedFieldBuilder() { Name = Util.ToUppercaseFirst(cat.ToString()), Value = "category description", IsInline = true });
                    }
                }
                else
                {
                    embed.Description = $"For further help use `{gconf.Prefix}help <commandName>`, for example `{gconf.Prefix}help roast`";

                    foreach (var command in commandsToShow)
                    {
                        embed.AddField(new EmbedFieldBuilder() { Name = Util.ToUppercaseFirst(command.Handle), Value = command.Description, IsInline = true });
                    }
                }
            }
            else
            {
                embed = new EmbedBuilder()
                {
                    Title = "Help: " + Util.ToUppercaseFirst(commandToDisplay.Handle),
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  <> - Required argument, [] - Optional argument" }
                };

                string desc = $"{commandToDisplay.Description}\n\n**Usage:**";

                foreach (var usage in commandToDisplay.Usage)
                {
                    desc += $"\n**{usage.ToString()}** (eg. {usage.ToExample()})";
                }

                embed.Description = desc;
            }

            return embed;
        }
    }
}
