using Discord.Net.Bot.Database.Sql;
using LorisAngel.Common.Objects;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace LorisAngel.Bot.Database
{
    class RelationshipDatabase
    {
        public static LoriShip DoesExist(ulong user1, ulong user2)
        {
            LoriShip ship = null;

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

            if (dbCon.IsConnect())
            {
                var cmd = new MySqlCommand($"SELECT * FROM relationships WHERE (id1 = '{user1}' AND id2 = '{user2}') OR (id2 = '{user1}' AND id1 = '{user2}')", dbCon.Connection);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ship = new LoriShip(reader.GetUInt64(0), reader.GetUInt64(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetInt32(5));
                    }
                }

                reader.Close();
                cmd.Dispose();
                dbCon.Close();
            }
            return ship;
        }

        public static void SaveShip(LoriShip ship)
        {
            var save = Task.Run(async () =>
            {
                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

                if (dbCon.IsConnect())
                {
                    while (LCommandHandler.Saving) await Task.Delay(50);
                    LCommandHandler.Saving = true;

                    var cmd = new MySqlCommand("INSERT INTO relationships (id1, id2, name1, name2, shipname, percentage) VALUES (@id1, @id2, @name1, @name2, @shipname, @percentage)", dbCon.Connection);
                    cmd.Parameters.Add("@id1", MySqlDbType.UInt64).Value = ship.User1;
                    cmd.Parameters.Add("@id2", MySqlDbType.UInt64).Value = ship.User2;
                    cmd.Parameters.Add("@name1", MySqlDbType.String).Value = ship.Name1;
                    cmd.Parameters.Add("@name2", MySqlDbType.String).Value = ship.Name2;
                    cmd.Parameters.Add("@shipname", MySqlDbType.String).Value = ship.Shipname;
                    cmd.Parameters.Add("@percentage", MySqlDbType.Int32).Value = ship.Percentage;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        cmd.Dispose();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to add relationship to database: {e.Message}");
                        cmd.Dispose();
                    }

                    LCommandHandler.Saving = false;
                    dbCon.Close();
                }

            });
        }
    }
}
