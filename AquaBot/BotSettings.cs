using Newtonsoft.Json;
using System;
using System.IO;

namespace AquaBot
{
    [Serializable]
    public class BotSettings
    {
        public string TestingToken { get; set; }
        public string LiveToken { get; set; }
        public ulong[] Drinkers { get; set; }
    }

    public class Settings
    {
        private const string SettingFileName = "settings.json";
        public BotSettings CurrentSettings { get; set; }

        public void LoadSettings()
        {
            if (File.Exists(SettingFileName))
            {
                using (StreamReader re = File.OpenText(SettingFileName))
                {
                    JsonSerializer se = new JsonSerializer();
                    JsonTextReader reader = new JsonTextReader(re);
                    CurrentSettings = se.Deserialize<BotSettings>(reader);
                }
            }
            else
            {
                CurrentSettings = new BotSettings();
            }
        }

        public void SaveSettings()
        {
            using (StreamWriter file = File.CreateText(SettingFileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, CurrentSettings);
            }
        }
    }
}