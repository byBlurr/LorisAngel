using Discord.Net.Bot.Database.Sql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngel.Database
{
    public class FunDatabase
    {
        public static async Task<List<FunObject>> GetOfTypeAsync(string type)
        {
            type = type.ToLower();
            List<FunObject> Objects = new List<FunObject>();

            var dbCon = DBConnection.Instance();
            dbCon.DatabaseName = LCommandHandler.DATABASE_NAME;

            if (dbCon.IsConnect())
            {
                var cmd = new MySqlCommand($"SELECT * FROM funmessages WHERE (type = '{type}')", dbCon.Connection);
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string text = reader.GetString(1);
                        string extra = reader.GetString(2);;
                        Objects.Add(new FunObject(text, extra));
                    }
                }

                reader.Close();
                cmd.Dispose();
                dbCon.Close();
            }

            return Objects;
        }
    }

    public class FunObject
    {
        public string Text { get; private set; }
        public string Extra { get; private set; }

        public FunObject(string text, string extra = null)
        {
            Text = text;
            Extra = extra ?? "";
        }
    }
}
