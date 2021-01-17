using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace LorisAngel.Database
{
    public class PickupsFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "fun");

        [JsonIgnore]
        static readonly string Filename = "pickups.json";

        public List<string> Pickups { get; set; }

        public PickupsFile()
        {
            Pickups = new List<string>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                PickupsFile file = new PickupsFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static PickupsFile Load()
        {
            return JsonConvert.DeserializeObject<PickupsFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
