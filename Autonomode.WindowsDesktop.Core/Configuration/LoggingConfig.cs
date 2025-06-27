namespace Autonomode.WindowsDesktop.Core.Configuration
{
    public class LoggingConfig
    {
        public string Level { get; set; } = "Information";

        public string FilePath { get; set; } = "./Logs/test-execution.log";

        public bool EnableConsoleOutput { get; set; } = true;

        public int MaxFileSizeMB { get; set; } = 50;
    }
}
