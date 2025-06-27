using System.ComponentModel.DataAnnotations;

namespace Autonomode.WindowsDesktop.Core.Configuration
{
    public class ApplicationConfig
    {
        [Required]
        public string TargetApplicationPath { get; set; } = string.Empty;

        public int DefaultTimeoutSeconds { get; set; } = 30;

        public int ElementSearchTimeoutSeconds { get; set; } = 10;

        public bool EnableScreenshots { get; set; } = true;

        public string ScreenshotPath { get; set; } = "./Screenshots";
    }
}
