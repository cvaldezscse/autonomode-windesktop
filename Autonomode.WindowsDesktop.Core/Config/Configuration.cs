using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Autonomode.WindowsDesktop.Core.Config
{
    public static class Configuration
    {
        private static AppConfig _config;

        /// <summary>
        /// Loads the configuration from a YAML file.
        /// </summary>
        /// <param name="filePath">Relative path to the YAML file.</param>
        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Config file was not found in this path: {filePath}");
            else
            {
                var yaml = File.ReadAllText(filePath);
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                _config = deserializer.Deserialize<AppConfig>(yaml);
            }
        }

        /// <summary>
        /// Devuelve la configuración cargada.
        /// </summary>
        public static AppConfig Get()
        {
            if (_config == null)
                throw new InvalidOperationException("Configuration not loaded. Call Load() first.");
            return _config;
        }

        /// <summary>
        /// Devuelve la app path como constante accesible.
        /// </summary>
        public static string AppPath => _config?.App.Path ?? string.Empty;

        public static int WaitTime => _config?.App.WaitTime ?? 0;
    }
}
