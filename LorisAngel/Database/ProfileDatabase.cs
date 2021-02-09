using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Sql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LorisAngel.Database
{
    public class ProfileDatabase
    {
        private static readonly int DAYS_TILL_DELETE = 7; // How many days of no activity until theyre user is wiped...
        private static List<LoriUser> Users;
        private static bool ProfilesReady = false;

        // Loop through all users
        public static async Task ProcessUsers()
        {
            var SaveUsers = Task.Run(async () =>
            {
                await Util.LoggerAsync(new LogMessage(LogSeverity.Info, "Profiles", "Start of SaveUsers thread."));
                await Task.Delay(5000);

                Users = await GetAllUsersAsync();
                ProfilesReady = true;

                await Task.Delay(5000);
                while (true)
                {
                    DateTime startTime = DateTime.Now;
                    await SaveAllUsersAsync(Users);
                    int timetosave = (int)((DateTime.Now - startTime).TotalSeconds);
                    if (timetosave > 5) await Util.LoggerAsync(new LogMessage(LogSeverity.Warning, "Profiles", $"Saving users took {timetosave} seconds"));
                    await Task.Delay(60000); // Save users once a minute
                }
            });

            var CheckForNewUsers = Task.Run(async () =>
            {
                await Util.LoggerAsync(new LogMessage(LogSeverity.Info, "Profiles", "Start of CheckForNewUsers thread."));
                while (!ProfilesReady) await Task.Delay(500);

                int newUsers = 0;
                foreach (var g in CommandHandler.GetBot().Guilds)
                {
                    if (g.Id != 264445053596991498 && g.Id != 110373943822540800 && g.Id != 446425626988249089 && g.Id != 679338357884715035) // Don't include the bot servers
                    {
                        foreach (var u in CommandHandler.GetBot().GetGuild(g.Id).Users)
                        {
                            if (!DoesUserExistInMemory(u.Id) && !u.IsBot)
                            {
                                CreateNewUser((u as IUser));
                                newUsers++;
                            }
                        }
                    }
                }

                if (newUsers > 0) await Util.LoggerAsync(new LogMessage(LogSeverity.Info, "Profiles", $"Added {newUsers} new users."));
                else await Util.LoggerAsync(new LogMessage(LogSeverity.Info, "Profiles", $"No new users found."));
                await Util.LoggerAsync(new LogMessage(LogSeverity.Info, "Profiles", "End of CheckForNewUsers thread."));
            });

            var CheckForLostUsers = Task.Run(async () =>
            {
                await Util.LoggerAsync(new LogMessage(LogSeverity.Info, "Profiles", "Start of CheckForLostUsers thread."));
                while (!ProfilesReady) await Task.Delay(500);

                int remCount = 0;
                foreach (LoriUser user in Users)
                {
                    int daysSinceUpdate = (int)((DateTime.Now - user.LastUpdated).TotalDays);
                    if (daysSinceUpdate > DAYS_TILL_DELETE)
                    {
                        await RemoveUserAsync(user.Id);
                        Users.Remove(user);
                        remCount++;
                    }
                }

                await Util.LoggerAsync(new LogMessage(LogSeverity.Info, "Profiles", $"Removed {remCount} users."));
                await Util.LoggerAsync(new LogMessage(LogSeverity.Info, "Profiles", "End of CheckForLostUsers thread."));
            });
        }

        // Check if the profiles are ready to use...
        public static bool Ready()
        {
            return ProfilesReady;
        }

        // Get a specific user profile
        public static LoriUser GetUser(ulong id)
        {
            foreach (LoriUser usr in Users)
            {
                if (usr.Id == id) return usr;
            }
            return null;
        }


        // Get all users
        private static async Task<List<LoriUser>> GetAllUsersAsync()
        {
            List<LoriUser> users = new List<LoriUser>();

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

            if (dbCon.IsConnect())
            {
                var cmd = new MySqlCommand($"SELECT * FROM users", dbCon.Connection);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ulong id = reader.GetUInt64(0);
                        string name = reader.GetString(1);
                        DateTime createdOn = reader.GetDateTime(2);
                        DateTime joinedOn = reader.GetDateTime(3);
                        DateTime lastSeen = reader.GetDateTime(4);
                        string status = reader.GetString(5);
                        DateTime lastUpdated = reader.GetDateTime(7);
                        string activity = reader.GetString(8);
                        string motto = reader.GetString(9);
                        int currency = reader.GetInt32(10);
                        DateTime claimedAt = reader.GetDateTime(11);

                        if (activity == null) activity = "";
                        if (motto == null) motto = "";
                        if (claimedAt == null) claimedAt = new DateTime();

                        LoriUser newUser = new LoriUser(id, name, createdOn, joinedOn, lastSeen, status, "", lastUpdated, motto, activity, currency, claimedAt);
                        users.Add(newUser);
                    }
                }

                reader.Close();
                cmd.Dispose();
                dbCon.Close();
            }

            return users;
        }

        // Save all users
        private static async Task SaveAllUsersAsync(List<LoriUser> users)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;
            if (dbCon.IsConnect())
            {
                foreach (LoriUser user in users)
                {
                    if (user.HasChanged)
                    {
                        if (user.IsNew)
                        {
                            await AddUserToDatabaseAsync(user);
                            user.HasChanged = false;
                            user.IsNew = false;
                        }
                        else
                        {
                            var cmd = new MySqlCommand($"UPDATE users SET name = @name, lastseen = @lastseen, status = @status, lastupdated = @lastupdated, motto = @motto, activity = @activity, currency = @currency, claimedat = @claimedat WHERE id = @id", dbCon.Connection);
                            cmd.Parameters.Add("@id", MySqlDbType.UInt64).Value = user.Id;
                            cmd.Parameters.Add("@name", MySqlDbType.String).Value = user.Name;
                            cmd.Parameters.Add("@lastseen", MySqlDbType.DateTime).Value = user.LastSeen;
                            cmd.Parameters.Add("@status", MySqlDbType.String).Value = user.Status;
                            cmd.Parameters.Add("@lastupdated", MySqlDbType.DateTime).Value = user.LastUpdated;
                            cmd.Parameters.Add("@activity", MySqlDbType.String).Value = user.Activity;
                            cmd.Parameters.Add("@motto", MySqlDbType.String).Value = user.Motto;
                            cmd.Parameters.Add("@currency", MySqlDbType.Int32).Value = user.Currency;
                            cmd.Parameters.Add("@claimedat", MySqlDbType.DateTime).Value = user.Claimed;

                            try
                            {
                                await cmd.ExecuteNonQueryAsync();
                                user.HasChanged = false;
                                user.IsNew = false;
                                cmd.Dispose();
                            }
                            catch (Exception e)
                            {
                                if (e.Message.StartsWith("Duplicate entry"))
                                {
                                    List<LoriUser> usersToRemove;
                                    List<LoriUser> usersWithId = new List<LoriUser>();

                                    foreach (LoriUser usr in Users)
                                    {
                                        if (usr.Id == user.Id)
                                        {
                                            usersWithId.Add(usr);
                                        }
                                    }

                                    if (usersWithId.Count > 1)
                                    {
                                        usersToRemove = usersWithId.OrderBy(x => x.JoinedOn).ToList();
                                        usersToRemove.RemoveAt(0);

                                        ulong id = user.Id;
                                        string name = user.Name;
                                        string motto = user.Motto;
                                        string status = user.Status;
                                        DateTime joinedOn = user.JoinedOn;

                                        foreach (LoriUser userToRemove in usersToRemove)
                                        {
                                            for (int i = 0; i < Users.Count; i++)
                                            {
                                                LoriUser usr = Users[i];
                                                if (usr.Id == id)
                                                {
                                                    if (usr.Name.Equals(name) && usr.Motto.Equals(motto) && usr.JoinedOn == joinedOn && usr.Status == status)
                                                    {
                                                        Users.RemoveAt(i);
                                                    }
                                                }
                                            }
                                        }

                                        await UpdateUserAsync(id);
                                    }

                                    await Util.LoggerAsync(new LogMessage(LogSeverity.Error, "Profiles", $"Duplicate entry ({user.Id}) - removed duplicate user", null));
                                }
                                else
                                {
                                    await Util.LoggerAsync(new LogMessage(LogSeverity.Error, "Profiles", e.Message, null));

                                    // Lets see what chars are causing the issue...
                                    if (e.Message.Contains("Incorrect string value"))
                                    {
                                        await Util.LoggerAsync(new LogMessage(LogSeverity.Warning, "Profiles", user.Activity, null));
                                    }
                                }

                                cmd.Dispose();
                            }
                        }
                    }
                }

                dbCon.Close();
            }
        }


        // Check if user exists in database
        private static bool DoesUserExistInDatabase(ulong id)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

            if (dbCon.IsConnect())
            {
                var cmd = new MySqlCommand($"SELECT * FROM users WHERE id = '{id}'", dbCon.Connection);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Close();
                    cmd.Dispose();
                    dbCon.Close();
                    return true;
                }
                else
                {
                    reader.Close();
                    cmd.Dispose();
                    dbCon.Close();
                    return false;
                }
            }

            return false;
        }

        // Check if user exists in memory
        public static bool DoesUserExistInMemory(ulong id)
        {
            foreach (LoriUser usr in Users)
            {
                if (usr.Id == id) return true;
            }
            return false;
        }


        // Add user to database
        private static async Task AddUserToDatabaseAsync(LoriUser user)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;
            if (dbCon.IsConnect())
            {
                var cmd = new MySqlCommand($"INSERT INTO users (id, name, createdon, joinedon, lastseen, status, badges, lastupdated, motto, activity, currency, claimedat) VALUES (@id, @name, @createdon, @joinedon, @lastseen, @status, @badges, @lastupdated, @motto, @activity, @currency, @claimedat)", dbCon.Connection);
                cmd.Parameters.Add("@id", MySqlDbType.UInt64).Value = user.Id;
                cmd.Parameters.Add("@name", MySqlDbType.String).Value = user.Name;
                cmd.Parameters.Add("@createdon", MySqlDbType.DateTime).Value = user.CreatedOn;
                cmd.Parameters.Add("@joinedon", MySqlDbType.DateTime).Value = user.JoinedOn;
                cmd.Parameters.Add("@lastseen", MySqlDbType.DateTime).Value = user.LastSeen;
                cmd.Parameters.Add("@status", MySqlDbType.String).Value = user.Status;
                cmd.Parameters.Add("@badges", MySqlDbType.String).Value = "";
                cmd.Parameters.Add("@lastupdated", MySqlDbType.DateTime).Value = user.LastUpdated;
                cmd.Parameters.Add("@activity", MySqlDbType.String).Value = user.Activity;
                cmd.Parameters.Add("@motto", MySqlDbType.String).Value = user.Motto;
                cmd.Parameters.Add("@currency", MySqlDbType.Int32).Value = user.Currency;
                cmd.Parameters.Add("@claimedat", MySqlDbType.DateTime).Value = user.Claimed;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    cmd.Dispose();
                }
                catch (Exception e)
                {
                    await Util.LoggerAsync(new LogMessage(LogSeverity.Error, "Profiles", e.Message, null));

                    // Lets see what chars are causing the issue...
                    if (e.Message.Contains("Incorrect string value"))
                    {
                        await Util.LoggerAsync(new LogMessage(LogSeverity.Warning, "Profiles", user.Activity, null));
                    }

                    cmd.Dispose();
                }

                dbCon.Close();
            }
        }

        // Create a new user and add it to the Users list
        public static void CreateNewUser(IUser user)
        {
            DateTime lastseen = new DateTime();
            if (user.Status != UserStatus.Offline && user.Status != UserStatus.Invisible) lastseen = DateTime.Now;
            LoriUser newUser = new LoriUser(user.Id, "", user.CreatedAt.DateTime, DateTime.Now, lastseen, user.Status.ToString(), "", DateTime.Now);
            newUser.SetNew();
            Users.Add(newUser);
        }

        // Remove user from database
        private static async Task RemoveUserAsync(ulong id)
        {
            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;
            if (dbCon.IsConnect())
            {
                var cmd = new MySqlCommand($"DELETE FROM users WHERE id = '{id}'", dbCon.Connection);

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    await Util.LoggerAsync(new LogMessage(LogSeverity.Info, "Profiles", "Deleted old/lost user."));
                    cmd.Dispose();
                }
                catch (Exception e)
                {
                    await Util.LoggerAsync(new LogMessage(LogSeverity.Error, "Profiles", "Failed to delete user: " + e.Message));
                    cmd.Dispose();
                }

                dbCon.Close();
            }
        }

        public static void AddCurrency(ulong id, float amt)
        {
            foreach (LoriUser user in Users)
            {
                if (user.Id == id)
                {
                    user.AddCurrency(amt);
                }
            }
        }

        public static async Task UpdateUserAsync(ulong id)
        {
            while (!ProfilesReady) await Task.Delay(500);
            var bot = CommandHandler.GetBot();

            var discUsr = bot.GetUser(id);
            var loriUsr = GetUser(id);

            if (loriUsr != null && !discUsr.IsBot)
            {
                if (discUsr.Activity != null) loriUsr.UpdateActivity(discUsr.Activity.ToString());
                else if (discUsr.Status != UserStatus.Offline && discUsr.Status != UserStatus.Invisible) loriUsr.UpdateActivity("");
                loriUsr.UpdateStatus(discUsr.Status);
                loriUsr.UpdateName(discUsr.Username);
            }
        }

        public static void ClaimDaily(ulong id)
        {
            foreach (LoriUser user in Users)
            {
                if (user.Id == id)
                {
                    user.ClaimDaily();
                }
            }
        }

        public static bool HasClaimedDaily(ulong id)
        {
            foreach (LoriUser user in Users)
            {
                if (user.Id == id)
                {
                    return user.HasClaimedDaily();
                }
            }
            return false;
        }

        public static void SetUserOnline(ulong id)
        {
            foreach (LoriUser user in Users)
            {
                if (user.Id == id)
                {
                    user.UpdateStatus(UserStatus.Online);
                }
            }
        }

        public static void SetUserActivity(ulong id, string activity)
        {
            foreach (LoriUser user in Users)
            {
                if (user.Id == id)
                {
                    user.UpdateActivity(activity);
                }
            }
        }
        public static void SetUserMotto(ulong id, string motto)
        {
            foreach (LoriUser user in Users)
            {
                if (user.Id == id)
                {
                    user.UpdateMotto(motto);
                }
            }
        }
    }
}
