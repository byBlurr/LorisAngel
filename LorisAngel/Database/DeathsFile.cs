using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace LorisAngel.Database
{
    public class DeathsFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "fun");

        [JsonIgnore]
        static readonly string Filename = "deaths.json";

        public List<string> Deaths { get; set; }

        public DeathsFile()
        {
            Deaths = new List<string>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                DeathsFile file = new DeathsFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static DeathsFile Load()
        {
            return JsonConvert.DeserializeObject<DeathsFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
