using System.ComponentModel.DataAnnotations;

namespace Autonomode.WindowsDesktop.Core.Configuration
{
    /// <summary>
    /// Central configuration model for test environments and application settings
    /// Implements validation to catch configuration errors early in CI/CD pipeline
    /// </summary>
    public class AppSettings
    {
        [Required]
        public string Environment { get; set; } = string.Empty;

        [Required]
        public ApplicationConfig Application { get; set; } = new();

        [Required]
        public TestDataConfig TestData { get; set; } = new();

        public LoggingConfig Logging { get; set; } = new();

        public ReportingConfig Reporting { get; set; } = new();
    }
}
