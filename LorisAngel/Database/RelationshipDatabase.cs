using Discord.Net.Bot.Database.Sql;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace LorisAngel.Database
{
    class RelationshipDatabase
    {
        private static bool Saving = false;

        public static Relationship DoesExist(ulong user1, ulong user2)
        {
            Relationship ship = null;

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
                        ship = new Relationship(reader.GetUInt64(0), reader.GetUInt64(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetInt32(5));
                    }
                }

                reader.Close();
                cmd.Dispose();
                dbCon.Close();
            }
            return ship;
        }

        public static void SaveShip(Relationship ship)
        {
            var save = Task.Run(async () =>
            {
                while (Saving == true)
                {
                    await Task.Delay(50);
                }
                Saving = true;

                var dbCon = DBConnection.Instance();
                dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

                if (dbCon.IsConnect())
                {
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

                    Saving = false;
                    dbCon.Close();
                }

            });
        }
    }

    public class Relationship
    {
        public ulong User1 { get; set; }
        public ulong User2 { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Shipname { get; set; }
        public int Percentage { get; set; }

        public Relationship(ulong user1, ulong user2, string name1, string name2, string shipname, int percentage)
        {
            this.User1 = user1;
            this.User2 = user2;
            this.Name1 = name1 ?? throw new ArgumentNullException(nameof(name1));
            this.Name2 = name2 ?? throw new ArgumentNullException(nameof(name2));
            this.Shipname = shipname ?? throw new ArgumentNullException(nameof(shipname));
            this.Percentage = percentage;
        }
    }
}
