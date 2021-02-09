using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using Discord.Net.Bot.Database.Sql;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace LorisAngel.Database
{
    class ModerationDatabase
    {
        /***
         *  Remove any bans that have timed out
         */
        public static async Task ProcessBansAsync()
        {
            var processBans = Task.Run(async () =>
            {
                while (true)
                {
                    var dbCon = DBConnection.Instance();
                    dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;
                    BotConfig conf = BotConfig.Load();

                    if (dbCon.IsConnect())
                    {
                        var cmd = new MySqlCommand($"SELECT * FROM tempbans", dbCon.Connection);
                        var reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                DateTime bannedTill = reader.GetDateTime(2);
                                if (bannedTill < DateTime.Now)
                                {
                                    ulong guildid = reader.GetUInt64(0);
                                    ulong userid = reader.GetUInt64(1);

                                    var guild = CommandHandler.GetBot().GetGuild(guildid);

                                    if (guild != null)
                                    {
                                        var gconf = conf.GetConfig(guild.Id);
                                        if (gconf.LogActions)
                                        {
                                            var logs = guild.GetTextChannel(gconf.LogChannel);
                                            if (logs != null)
                                            {
                                                EmbedBuilder embed = new EmbedBuilder()
                                                {
                                                    Title = "User Unbanned",
                                                    Description = $"A users temp ban was removed. Check audit log for more info.",
                                                    Color = Color.DarkPurple,
                                                    Footer = new EmbedFooterBuilder() { Text = $"{EmojiUtil.GetRandomEmoji()}  Edit moderation settings on the webpanel." }
                                                };
                                                await logs.SendMessageAsync(null, false, embed.Build());
                                            }
                                        }

                                        await guild.RemoveBanAsync(userid);
                                    }

                                    var removecmd = new MySqlCommand($"REMOVE FROM tempbans WHERE userid = '{userid}'", dbCon.Connection);
                                    try
                                    {
                                        await removecmd.ExecuteNonQueryAsync();
                                        removecmd.Dispose();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine($"Failed to remove temp ban: {e.Message}");
                                        removecmd.Dispose();
                                    }
                                }
                            }
                        }

                        reader.Close();
                        cmd.Dispose();
                        dbCon.Close();
                    }

                    await Task.Delay(60000);
                }
            });
        }

        /***
         *  Add the temp ban to the database
         */
        public static async Task AddTempBanAsync(ulong guildId, ulong userId, DateTime bannedTill)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

            if (dbCon.IsConnect())
            {
                while (LCommandHandler.Saving) await Task.Delay(50);
                LCommandHandler.Saving = true;

                var cmd = new MySqlCommand($"INSERT INTO tempbans (guildid, userid, bannedtill) VALUES (@guildid, @userid, @bannedtill)", dbCon.Connection);
                cmd.Parameters.Add("@guildid", MySqlDbType.UInt64).Value = guildId;
                cmd.Parameters.Add("@userid", MySqlDbType.UInt64).Value = userId;
                cmd.Parameters.Add("@bannedtill", MySqlDbType.DateTime).Value = bannedTill;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    cmd.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to add temp ban: {e.Message}");
                    cmd.Dispose();
                }

                LCommandHandler.Saving = false;

                dbCon.Close();
            }
        }
    }
}
