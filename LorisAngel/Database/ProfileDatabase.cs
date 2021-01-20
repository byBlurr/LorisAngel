using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Sql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LorisAngel.Database
{
    public class ProfileDatabase
    {
        private static List<LoriUser> Users;
        private static bool ReadyToUpdate = false;

        // Loop through all users
        public static async Task ProcessUsers()
        {
            var SaveUsers = Task.Run(async () =>
            {
                await Util.Logger(new LogMessage(LogSeverity.Info, "Profiles", "Start of SaveUsers thread."));
                await Task.Delay(5000);

                Users = await GetAllUsersAsync();
                ReadyToUpdate = true;

                await Task.Delay(5000);
                while (true)
                {
                    DateTime startTime = DateTime.Now;
                    await SaveAllUsersAsync(Users);
                    int timetosave = (int)((DateTime.Now - startTime).TotalSeconds);
                    if (timetosave > 5) await Util.Logger(new LogMessage(LogSeverity.Warning, "Profiles", $"Saving users took {timetosave} seconds"));
                    await Task.Delay(60000); // Save users once a minute
                }
            });

            var UpdateUsers = Task.Run(async () =>
            {
                await Util.Logger(new LogMessage(LogSeverity.Info, "Profiles", "Start of UpdateUsers thread."));
                var bot = CommandHandler.GetBot();
                while (!ReadyToUpdate) await Task.Delay(500);
                while (true)
                {
                    if (Users.Count != 0)
                    {
                        DateTime startTime = DateTime.Now;

                        foreach (LoriUser usr in Users)
                        {
                            var discUsr = bot.GetUser(usr.Id);
                            if (discUsr != null && !discUsr.IsBot)
                            {
                                usr.UpdateStatus(discUsr.Status);
                                usr.UpdateName(discUsr.Username);
                            }
                        }

                        int timetoupdate = (int)((DateTime.Now - startTime).TotalSeconds);
                        if (timetoupdate > 5) await Util.Logger(new LogMessage(LogSeverity.Warning, "Profiles", $"Updating users took {timetoupdate} seconds"));
                    }
                    await Task.Delay(500);
                }
            });

            var CheckForNewUsers = Task.Run(async () =>
            {
                await Util.Logger(new LogMessage(LogSeverity.Info, "Profiles", "Start of CheckForNewUsers thread."));
                while (!ReadyToUpdate) await Task.Delay(500);

                int newUsers = 0;
                foreach (var g in CommandHandler.GetBot().Guilds)
                {
                    if (g.Id != 264445053596991498 && g.Id != 110373943822540800 && g.Id != 446425626988249089) // Don't include the bot servers
                    {
                        foreach (var u in CommandHandler.GetBot().GetGuild(g.Id).Users)
                        {
                            if (!DoesUserExistMemory(u.Id) && !u.IsBot)
                            {
                                CreateNewUser((u as IUser));
                                newUsers++;
                            }
                        }
                    }
                }

                if (newUsers > 0) await Util.Logger(new LogMessage(LogSeverity.Info, "Profiles", $"Added {newUsers} new users."));
                else await Util.Logger(new LogMessage(LogSeverity.Info, "Profiles", $"No new users found."));
            });
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

                        LoriUser newUser = new LoriUser(id, name, createdOn, joinedOn, lastSeen, status, "", lastUpdated);
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
                            var cmd = new MySqlCommand($"UPDATE users SET name = @name, lastseen = @lastseen, status = @status, lastupdated = @lastupdated WHERE id = @id", dbCon.Connection);
                            cmd.Parameters.Add("@id", MySqlDbType.UInt64).Value = user.Id;
                            cmd.Parameters.Add("@name", MySqlDbType.String).Value = "";
                            cmd.Parameters.Add("@lastseen", MySqlDbType.DateTime).Value = user.LastSeen;
                            cmd.Parameters.Add("@status", MySqlDbType.String).Value = user.Status;
                            cmd.Parameters.Add("@lastupdated", MySqlDbType.DateTime).Value = user.LastUpdated;

                            try
                            {
                                await cmd.ExecuteNonQueryAsync();
                                user.HasChanged = false;
                                user.IsNew = false;
                                cmd.Dispose();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Failed to save user: {e.Message}");
                                cmd.Dispose();
                            }
                        }
                    }
                }

                dbCon.Close();
            }
        }


        // Check if user exists in database
        private static bool DoesUserExistDatabase(ulong id)
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
        public static bool DoesUserExistMemory(ulong id)
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
                var cmd = new MySqlCommand($"INSERT INTO users (id, name, createdon, joinedon, lastseen, status, badges, lastupdated) VALUES (@id, @name, @createdon, @joinedon, @lastseen, @status, @badges, @lastupdated)", dbCon.Connection);
                cmd.Parameters.Add("@id", MySqlDbType.UInt64).Value = user.Id;
                cmd.Parameters.Add("@name", MySqlDbType.String).Value = "";
                cmd.Parameters.Add("@createdon", MySqlDbType.DateTime).Value = user.CreatedOn;
                cmd.Parameters.Add("@joinedon", MySqlDbType.DateTime).Value = user.JoinedOn;
                cmd.Parameters.Add("@lastseen", MySqlDbType.DateTime).Value = user.LastSeen;
                cmd.Parameters.Add("@status", MySqlDbType.String).Value = user.Status;
                cmd.Parameters.Add("@badges", MySqlDbType.String).Value = "";
                cmd.Parameters.Add("@lastupdated", MySqlDbType.DateTime).Value = user.LastUpdated;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    cmd.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to save user: {e.Message}");
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

        }
    }

    public class LoriUser
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public DateTime JoinedOn { get; private set; }
        public DateTime LastSeen { get; private set; }
        public string Status { get; private set; }
        public string Badges { get; private set; } // WILL BE A LIST OF BADGES ONCE BADGES ADDED
        public bool HasChanged { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsNew { get; set; }

        public LoriUser(ulong id, string name, DateTime createdOn, DateTime joinedOn, DateTime lastSeen, string status, string badges, DateTime lastUpdated)
        {
            Id = id;
            Name = name.Normalize() ?? throw new ArgumentNullException(nameof(name));
            CreatedOn = createdOn;
            JoinedOn = joinedOn;
            LastSeen = lastSeen;
            Status = status ?? throw new ArgumentNullException(nameof(status));
            Badges = badges;
            HasChanged = false;
            LastUpdated = lastUpdated;
            IsNew = false;
        }

        public void SetNew()
        {
            IsNew = true;
            HasChanged = true;
        }

        public void UpdateStatus(UserStatus newStatus)
        {
            if (!newStatus.ToString().Equals(Status))
            {
                Status = newStatus.ToString();
                LastSeen = DateTime.Now;

                HasChanged = true;
                LastUpdated = DateTime.Now;
            }
        }

        public void UpdateName(string newName)
        {
            return; // WE ARENT USING THIS FOR NOW
            if (newName.Normalize() == Name) return;
            Name = newName.Normalize();
            HasChanged = true;
            LastUpdated = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            return obj is LoriUser user &&
                   Id == user.Id;
        }
    }
}
