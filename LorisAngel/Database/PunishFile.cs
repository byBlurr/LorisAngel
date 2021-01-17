using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace LorisAngel.Database
{
    public class PunishFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "fun");

        [JsonIgnore]
        static readonly string Filename = "punishments.json";

        public List<string> Punishments { get; set; }

        public PunishFile()
        {
            Punishments = new List<string>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                PunishFile file = new PunishFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static PunishFile Load()
        {
            return JsonConvert.DeserializeObject<PunishFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
