using System;
using TestBot.Utils;

namespace TestBot
{
    public static class Program
    {
        public static void Main()
        {
            var loader = new FileLoader();
            var settings = loader.ReadSettings();

            settings.Log();
            Console.ReadKey();
        }
    }
}