using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomode.WindowsDesktop.Core.Utils
{
    public static class PathUtils
    {
        /// <summary>
        /// Finds the absolute path to the configuration.yaml file,
        /// traversing up directories if needed.
        /// </summary>
        /// <returns>The absolute path to the configuration.yaml file.</returns>
        public static string GetConfigFilePath()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var directoryInfo = new DirectoryInfo(currentDirectory);

            while (directoryInfo != null)
            {
                // Look for the Resources directory
                var resourcesPath = Path.Combine(directoryInfo.FullName, "Autonomode.WindowsDesktop.Resources");
                var configFile = Path.Combine(resourcesPath, "configuration.yaml");

                if (File.Exists(configFile))
                    return configFile;
                directoryInfo = directoryInfo.Parent;
            }

            throw new FileNotFoundException("configuration.yaml file not found in any parent directory.");
        }
    }
}
