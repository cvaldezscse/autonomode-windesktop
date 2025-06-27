using System.ComponentModel.DataAnnotations;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Autonomode.WindowsDesktop.Core.Configuration;

namespace Autonomode.WindowsDesktop.Core.Services
{
    /// <summary>
    /// Configuration service implementing Singleton pattern for consistent config access
    /// Validates configuration on load to prevent runtime errors
    /// Leadership tip: Centralized config prevents inconsistencies across team members
    /// </summary>
    public class ConfigurationService
    {
        private static ConfigurationService? _instance;
        private static readonly object _lock = new();
        private AppSettings? _settings;

        private ConfigurationService() { }

        public static ConfigurationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ConfigurationService();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Loads configuration from YAML file with validation
        /// Throws meaningful exceptions for team debugging
        /// </summary>
        /// <param name="configPath">Path to YAML configuration file</param>
        /// <param name="environment">Target environment (dev, staging, prod)</param>
        public void LoadConfiguration(string configPath, string environment)
        {
            try
            {
                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException($"Configuration file not found: {configPath}");
                }

                var yamlContent = File.ReadAllText(configPath);

                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                _settings = deserializer.Deserialize<AppSettings>(yamlContent);

                if (_settings == null)
                {
                    throw new InvalidOperationException("Failed to deserialize configuration");
                }

                // Override environment if specified
                if (!string.IsNullOrEmpty(environment))
                {
                    _settings.Environment = environment;
                }

                ValidateConfiguration(_settings);

                Console.WriteLine($"✅ Configuration loaded successfully for environment: {_settings.Environment}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Configuration loading failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validates configuration using Data Annotations
        /// Prevents silent failures that waste team debugging time
        /// </summary>
        private static void ValidateConfiguration(AppSettings settings)
        {
            var results = new List<ValidationResult>();

            // Validate main settings object
            var context = new ValidationContext(settings);
            if (!Validator.TryValidateObject(settings, context, results, true))
            {
                var errors = string.Join("\n", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"Configuration validation failed:\n{errors}");
            }

            // Validate nested objects manually
            ValidateNestedObject(settings.Application, "Application", results);
            ValidateNestedObject(settings.TestData, "TestData", results);
            ValidateNestedObject(settings.Logging, "Logging", results);
            ValidateNestedObject(settings.Reporting, "Reporting", results);

            // Check if any validation errors occurred in nested objects
            if (results.Count > 0)
            {
                var errors = string.Join("\n", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"Configuration validation failed:\n{errors}");
            }
        }

        /// <summary>
        /// Validates nested configuration objects
        /// </summary>
        private static void ValidateNestedObject(object obj, string propertyName, List<ValidationResult> results)
        {
            if (obj == null) return;

            var context = new ValidationContext(obj) { DisplayName = propertyName };
            var nestedResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(obj, context, nestedResults, true))
            {
                // Add nested results to main results list with property prefix
                foreach (var result in nestedResults)
                {
                    var memberNames = result.MemberNames?.Select(m => $"{propertyName}.{m}") ?? new[] { propertyName };
                    results.Add(new ValidationResult(result.ErrorMessage, memberNames));
                }
            }
        }

        public AppSettings Settings => _settings ??
            throw new InvalidOperationException("Configuration not loaded. Call LoadConfiguration() first.");

        public string Environment => Settings.Environment;

        public ApplicationConfig Application => Settings.Application;

        public TestDataConfig TestData => Settings.TestData;

        public LoggingConfig Logging => Settings.Logging;

        public ReportingConfig Reporting => Settings.Reporting;
    }
}