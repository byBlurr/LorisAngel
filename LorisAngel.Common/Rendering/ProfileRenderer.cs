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
                Data = SqlHelper.SelectDataFromTable<ProfileData>(connection, "profiles", columns, $"id = '{User.Id}'")[0];
            }
            catch (Exception ex)
            {
                Util.Log(LogType.Error, "MySQL", ex.Message);
            }
        }
    }
}
