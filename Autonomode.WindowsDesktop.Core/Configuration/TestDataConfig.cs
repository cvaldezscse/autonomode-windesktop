using System.ComponentModel.DataAnnotations;

namespace Autonomode.WindowsDesktop.Core.Configuration
{
    public class TestDataConfig
    {
        [Required]
        public string BasePath { get; set; } = string.Empty;

        [Required]
        public string UsersDataFile { get; set; } = string.Empty;

        public string TestCasesDataFile { get; set; } = string.Empty;

        public string DatabaseConnectionString { get; set; } = string.Empty;
    }
}
