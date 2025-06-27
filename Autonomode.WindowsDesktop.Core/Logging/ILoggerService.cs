using System.Runtime.CompilerServices;

namespace Autonomode.WindowsDesktop.Core.Logging
{
    /// <summary>
    /// Strategic logging service for test automation framework
    /// Implements structured logging with context for debugging efficiency
    /// Leadership tip: Good logging reduces team debugging time by 60-80%
    /// </summary>
    public interface ILoggerService
    {
        void LogInfo(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "");
        void LogWarning(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "");
        void LogError(string message, Exception? exception = null, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "");
        void LogDebug(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "");
        void LogTestStep(string stepDescription, string status = "EXECUTING");
        void LogTestResult(string testName, bool passed, string details = "");
        void Flush();
    }
}