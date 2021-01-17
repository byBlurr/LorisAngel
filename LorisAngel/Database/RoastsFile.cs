using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LorisAngel.Database
{
    public class RoastsFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "fun");

        [JsonIgnore]
        static readonly string Filename = "roasts.json";

        public List<string> Roasts { get; set; }

        public RoastsFile()
        {
            Roasts = new List<string>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                RoastsFile file = new RoastsFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static RoastsFile Load()
        {
            return JsonConvert.DeserializeObject<RoastsFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
