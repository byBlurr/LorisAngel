using Blurr;
using Blurr.Sql;
using LorisAngel.Common.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
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
            string backgroundFile = Path.Combine(AppContext.BaseDirectory, "textures/profiles", Data.BackgroundImage());

            int width = 1280;
            int height = 720;
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            string fontName = "Arial";
            Brush darkBrush = new SolidBrush(Data.GetColour().ToDrawColor());

            Bitmap editedBitmap = new Bitmap(width, height);
            Graphics graphicImage = Graphics.FromImage(editedBitmap);
            graphicImage.SmoothingMode = SmoothingMode.AntiAlias;

            Bitmap backTexture = new Bitmap(backgroundFile);
            graphicImage.DrawImage(backTexture, 0, 0, width, height);

            graphicImage.DrawString(User.Name, new Font(fontName, 32, FontStyle.Bold), darkBrush, ProfileLocations.Name.X + (ProfileLocations.Name.Width / 2), ProfileLocations.Name.Y + (ProfileLocations.Name.Height / 2), format);

            editedBitmap.Save(GetPath(), System.Drawing.Imaging.ImageFormat.Png);
            graphicImage.Dispose();
            editedBitmap.Dispose();
            backTexture.Dispose();
        }

        public void GetProfileData()
        {
            var connection = SqlConnection.Instance();
            connection.DatabaseName = "lorisangel";
            string[] columns = { "background", "avatar", "colour" };

            try
            {
                var rows = SqlHelper.SelectDataFromTable<ProfileDatabaseRow>(connection, "profiles", columns, $"id = '{User.Id}'");
                if (rows.Count > 0) Data = new ProfileData(rows[0].background, rows[0].avatar, rows[0].colour);
                else
                {
                    Data = new ProfileData();

                    string[] insColumns = { "id", "background", "avatar", "colour" };
                    object[] values = { User.Id, Data.Background, Data.AssembleAvatarString(), Data.Colour };
                    SqlHelper.InsertIntoTable(connection, "profiles", insColumns, values);
                }
            }
            catch (Exception ex)
            {
                Util.Log(LogType.Error, "MySql", ex.Message);
            }
        }

        public void Dispose()
        {
            if (File.Exists(GetPath())) File.Delete(GetPath());
        }

        public string GetPath() => Path.Combine(AppContext.BaseDirectory, "profiles", $"profile_{User.Id}.png");
    }

    public class ProfileLocations
    {
        public static readonly Rectangle Name = new Rectangle(129, 34, 701, 71);
        public static readonly Rectangle Status = new Rectangle(129, 116, 701, 38);
        public static readonly Rectangle Motto = new Rectangle(129, 165, 701, 38);
        public static readonly Rectangle CreatedOn = new Rectangle(129, 132, 342, 38);
        public static readonly Rectangle LastSeen = new Rectangle(488, 132, 342, 38);
        public static readonly Rectangle JoinedOn = new Rectangle(129, 279, 342, 38);
        public static readonly Rectangle UpdatedOn = new Rectangle(488, 279, 342, 38);
        public static readonly Rectangle LoriGuilds = new Rectangle(129, 326, 342, 38);
        public static readonly Rectangle Id = new Rectangle(488, 654, 342, 38);
    }

    public class ProfileDatabaseRow
    {
        public string background { get; set; }
        public string avatar { get; set; }
        public short colour { get; set; }
    }
}
