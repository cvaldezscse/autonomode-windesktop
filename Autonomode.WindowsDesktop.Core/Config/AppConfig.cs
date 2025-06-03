using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomode.WindowsDesktop.Core.Config
{
    public class AppConfig
    {
        public AppSettings App { get; set; }
        public TestSettings Test { get; set; }
    }

    public class AppSettings
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int WaitTime { get; set; }
    }

    public class TestSettings
    {
        public string Environment { get; set; }
        public string ReportPath { get; set; }
    }
}
