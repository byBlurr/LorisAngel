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

        private ulong Id { get; set; }
        private LoriUser User { get; set; }
        private ProfileData Data { get; set; }

        public ProfileRenderer(ulong id, LoriUser user)
        {
            Id = id;
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
            graphicImage.DrawString(User.Status, new Font(fontName, 18, FontStyle.Bold), darkBrush, ProfileLocations.Status.X + (ProfileLocations.Status.Width / 2), ProfileLocations.Status.Y + (ProfileLocations.Status.Height / 2), format);
            graphicImage.DrawString(User.Motto, new Font(fontName, 18, FontStyle.Bold), darkBrush, ProfileLocations.Motto.X + (ProfileLocations.Motto.Width / 2), ProfileLocations.Motto.Y + (ProfileLocations.Motto.Height / 2), format);
            graphicImage.DrawString(User.CreatedOn.ToString(), new Font(fontName, 18, FontStyle.Bold), darkBrush, ProfileLocations.CreatedOn.X + (ProfileLocations.CreatedOn.Width / 2), ProfileLocations.CreatedOn.Y + (ProfileLocations.CreatedOn.Height / 2), format);
            graphicImage.DrawString(User.LastSeen.ToString(), new Font(fontName, 18, FontStyle.Bold), darkBrush, ProfileLocations.LastSeen.X + (ProfileLocations.LastSeen.Width / 2), ProfileLocations.LastSeen.Y + (ProfileLocations.LastSeen.Height / 2), format);
            graphicImage.DrawString(User.JoinedOn.ToString(), new Font(fontName, 18, FontStyle.Bold), darkBrush, ProfileLocations.JoinedOn.X + (ProfileLocations.JoinedOn.Width / 2), ProfileLocations.JoinedOn.Y + (ProfileLocations.JoinedOn.Height / 2), format);
            graphicImage.DrawString(User.LastUpdated.ToString(), new Font(fontName, 18, FontStyle.Bold), darkBrush, ProfileLocations.UpdatedOn.X + (ProfileLocations.UpdatedOn.Width / 2), ProfileLocations.UpdatedOn.Y + (ProfileLocations.UpdatedOn.Height / 2), format);
            graphicImage.DrawString(Id.ToString(), new Font(fontName, 18, FontStyle.Bold), darkBrush, ProfileLocations.Id.X + (ProfileLocations.Id.Width / 2), ProfileLocations.Id.Y + (ProfileLocations.Id.Height / 2), format);

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

        public string GetPath() => Path.Combine(AppContext.BaseDirectory, "profiles", $"profile_{Id}.png");
    }

    public class ProfileLocations
    {
        public static readonly Rectangle Name = new Rectangle(129, 34, 701, 71);
        public static readonly Rectangle Status = new Rectangle(129, 116, 701, 38);
        public static readonly Rectangle Motto = new Rectangle(129, 165, 701, 38);
        public static readonly Rectangle CreatedOn = new Rectangle(129, 232, 342, 38);
        public static readonly Rectangle LastSeen = new Rectangle(488, 232, 342, 38);
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
