using Autonomode.WindowsDesktop.Core.Config;
using Autonomode.WindowsDesktop.Core.Constants;
using Autonomode.WindowsDesktop.Core.Utils;
using System;

namespace Autonomode.WindowsDesktop.ConsoleDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== DEMO CONFIGURACIÓN ===");
            try
            {
                var configFilePath = PathUtils.GetConfigFilePath();
                Configuration.Load(configFilePath);

                Console.WriteLine("App Path: " + Configuration.AppPath);
                Console.WriteLine("Wait Time: " + Configuration.WaitTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading configuration: " + ex.Message);
            }
            Console.WriteLine($"App Path: {Configuration.AppPath}");
            Console.WriteLine($"Wait Time: {Configuration.WaitTime}");
            Console.WriteLine($"Constante: {TestConstants.DefaultTestName}");
            Console.ReadLine();
        }
    }
}