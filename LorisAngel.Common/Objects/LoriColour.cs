namespace LorisAngel.Common.Objects
{
    public class LoriColour
    {

        public static LoriColour LoriPurple = new LoriColour(121, 55, 134, "#793786");
        public static LoriColour LoriRed = new LoriColour(231, 74, 72, "#e74a48");
        public static LoriColour LoriGreen = new LoriColour(231, 74, 72, "#00b485");
        public static LoriColour Blurple = new LoriColour(114, 137, 218, "#7289DA");
        public static LoriColour FullWhite = new LoriColour(255, 255, 255, "#FFFFFF");
        public static LoriColour Greyple = new LoriColour(153, 170, 181, "#99AAB5");
        public static LoriColour DarkButNotBlack = new LoriColour(44, 47, 51, "#2C2F33");
        public static LoriColour NotQuiteBlack = new LoriColour(35, 39, 42, "#23272A");
        public static LoriColour ActualBlack = new LoriColour(0, 0, 0, "#000000");

        public int Red { get; private set; }
        public int Green { get; private set; }
        public int Blue { get; private set; }
        public string Hex { get; private set; }

        public LoriColour(int r, int g, int b, string hex)
        {
            Red = r;
            Green = g;
            Blue = b;
            Hex = hex;
        }

        // TODO: We probably want a way to convert this into a Discord Color?
        // public Discord.Color ConvertToDiscord() => new Discord.Color(r, g, b);
    }
}
