using System.Net;
using System.Text.Json;

namespace IPLogger
{
    /// <summary>
    /// Обработчик настроек
    /// </summary>
    public static class ConfigHandler
    {
        private const string _configPath = ".\\Configs\\config.txt";

        private static Dictionary<ConfigName, string> _configs;

        public static void ReadConfig()
        {
            if (!File.Exists(_configPath)) {
                WriteDefaults();
                return;
            }

            string jsonString = File.ReadAllText(_configPath);

            try {
                _configs = JsonSerializer.Deserialize<Dictionary<ConfigName, string>>(jsonString);
            }
            catch (Exception) {

                WriteDefaults();
            }
        }

        public static string GetConfigValue(ConfigName name)
        {
            if (_configs.ContainsKey(name)) {
                return _configs[name];
            }

            return null;
        }

        public static void WriteDefaults()
        {
            _configs = new Dictionary<ConfigName, string>()
            {
                { ConfigName.fileLog, ".\\Resources\\log.txt" },
                { ConfigName.fileOutput, ".\\Resources\\output.txt" },
                { ConfigName.timeStart, DateTime.MinValue.ToString() },
                { ConfigName.timeEnd, DateTime.MaxValue.ToString() },
                { ConfigName.addressStart, IPAddress.Any.ToString() },
                { ConfigName.addressMask, IPAddress.None.ToString() }
            };

            JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(_configs, options);

            File.WriteAllText(_configPath, jsonString);
        }
    }

    public enum ConfigName
    {
        fileLog,
        fileOutput,
        timeStart,
        timeEnd,
        addressStart,
        addressMask
    }
}
