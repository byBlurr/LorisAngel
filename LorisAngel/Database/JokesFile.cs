using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace LorisAngel.Database
{
    public class JokesFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "fun");

        [JsonIgnore]
        static readonly string Filename = "jokes.json";

        public List<string> Jokes { get; set; }

        public JokesFile()
        {
            Jokes = new List<string>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                JokesFile file = new JokesFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static JokesFile Load()
        {
            return JsonConvert.DeserializeObject<JokesFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
