using System;
using System.IO;
usi

namespace TestBot.Utils
{
    public class FileLoader
    {
        public string SettingsFolder =
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/AlpacaSettings/";

        public Settings ReadSettings()
        {
            using var r = new StreamReader(SettingsFolder + "Settings.json");
            var json = r.ReadToEnd();
            JsonConv
        }
    }
}