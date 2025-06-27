namespace Autonomode.WindowsDesktop.Core.Configuration
{
    public class ReportingConfig
    {
        public string OutputPath { get; set; } = "./Reports";

        public bool GenerateHtmlReport { get; set; } = true;

        public bool AttachScreenshots { get; set; } = true;

        public string ReportTitle { get; set; } = "Autonomode Test Execution Report";
    }
}
