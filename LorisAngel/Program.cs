using Discord.Net.Bot;
using System;

namespace LorisAngel
{
    class Program : Bot
    {
        static void Main(string[] args)
        {
            CommandHandler handler = new LCommandHandler();
            handler.ExecutableName = "LorisAngelBot";
            handler.RestartEveryMs = 21600000; // Every 6 hours
            StartBot(handler);
        }
    }
}
