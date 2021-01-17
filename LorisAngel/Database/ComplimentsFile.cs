using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace LorisAngel.Database
{
    public class ComplimentsFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "fun");

        [JsonIgnore]
        static readonly string Filename = "compliments.json";

        public List<string> Compliments { get; set; }
        public List<string> HugGifs { get; set; }

        public ComplimentsFile()
        {
            Compliments = new List<string>();
            HugGifs = new List<string>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                ComplimentsFile file = new ComplimentsFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static ComplimentsFile Load()
        {
            return JsonConvert.DeserializeObject<ComplimentsFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
