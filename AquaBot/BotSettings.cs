using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace AquaBot
{
    [Serializable]
    public class BotSettings
    {
        public string TestingToken { get; set; }
        public string LiveToken { get; set; }
        public ulong[] Drinkers { get; set; }
        public List<string> BannedWords { get; set; }
        public List<DayMessage> DayMessages { get; set; }
        public ulong DayMessageChannel { get; set; }
    }

    [Serializable]
    public class DayMessage
    {
        public string ImageLink { get; set; }
        public string Message { get; set; }
        public DateTime LastRun { get; set; }
        public DayOfWeek Day { get; set; }
        public bool MorningMessage { get; set; }
    }

    public class Settings
    {
        private const string SettingFileName = "settings.json";
        public BotSettings CurrentSettings { get; set; }

        public void LoadSettings()
        {
            if (CurrentSettings == null)
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
                if (CurrentSettings.DayMessages == null)
                {
                    CurrentSettings.DayMessages = new List<DayMessage>();
                }
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