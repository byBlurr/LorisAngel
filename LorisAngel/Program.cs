using Discord.Net.Bot;

namespace LorisAngel
{
    class Program : Bot
    {
        static void Main(string[] args)
        {
            CommandHandler handler = new LCommandHandler();
            handler.ExecutableName = "LorisAngel";
            handler.RestartEveryMs = 21600000; // Every 6 hours
            StartBot(handler);
        }
    }
}
