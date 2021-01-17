using Discord;
using Discord.Net.Bot.Database.Configs;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace LorisAngel.Utility
{
    class Moderation
    {
        public static async Task CheckMessageAsync(SocketMessage message)
        {
            IndividualConfig config = BotConfig.Load().GetConfig((message.Channel as IGuildChannel).GuildId);
            if (!config.Censor) return;

            if (message.Content.Length > 0)
            {
                string messageText = message.Content.ToLower();

                foreach (string word in config.CensoredWords)
                {
                    if (messageText.Contains(word))
                    {
                        await message.Author.SendMessageAsync("Your message was deleted as it contained censored words.");

                        if (config.LogActions)
                        {
                            var logChannel = await (message.Channel as IGuildChannel).Guild.GetTextChannelAsync(config.LogChannel);
                            await logChannel.SendMessageAsync($"**Message deleted for profanity.**\nMessage from {message.Author.Mention} was deleted for containing the word '{word}'.");
                        }

                        await message.DeleteAsync();
                        return;
                    }
                }
            }
        }
    }
}
