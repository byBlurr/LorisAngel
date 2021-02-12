using Blurr;
using Blurr.Sql;
using LorisAngel.Common.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace LorisAngel.Common.Rendering
{
    public class ProfileRenderer
    {

        private LoriUser User { get; set; }
        private ProfileData Data { get; set; }

        public ProfileRenderer(LoriUser user)
        {
            User = user;
            GetProfileData();
            Console.WriteLine(Data.ToString());
        }

        public void Render()
        {

        }

        public void GetProfileData()
        {
            var connection = SqlConnection.Instance();
            connection.DatabaseName = "lorisangel";
            string[] columns = { "background", "avatar" };

            try
            {
                var rows = SqlHelper.SelectDataFromTable<ProfileData>(connection, "profiles", columns, $"id = '{User.Id}'");
                if (rows.Count > 0) Data = rows[0];
                else
                {
                    Data = new ProfileData();

                    string[] insColumns = { "id", "background", "avatar" };
                    object[] values = { User.Id, Data.Background, Data.AssembleAvatarString() };
                    SqlHelper.InsertIntoTable(connection, "profiles", insColumns, values);
                }
            }
            catch (Exception ex)
            {
                Util.Log(LogType.Error, "MySql", ex.Message);
            }
        }
    }
}
