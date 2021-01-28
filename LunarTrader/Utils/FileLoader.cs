using System;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using Pastel;

namespace TestTradingBot.Utils
{
    public class FileLoader
    {
        private readonly string _settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/AlpacaSettings/";

        public Settings ReadSettings()
        {
            Console.WriteLine("Loading Settings".Pastel((Color.FromArgb(46, 151, 72))));
            string json;
            using (var r = new StreamReader(_settingsPath + "Settings.json"))
            {
                json = r.ReadToEnd();
            }

            var settings = JsonConvert.DeserializeObject<Settings>(json);
            return settings;
        }
    }
}