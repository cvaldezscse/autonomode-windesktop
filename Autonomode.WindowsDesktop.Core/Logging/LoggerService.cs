using System.Runtime.CompilerServices;

namespace Autonomode.WindowsDesktop.Core.Logging
{

    /// <summary>
    /// Static facade for easy access to logging throughout the framework
    /// Provides team-friendly API without dependency injection complexity
    /// </summary>
    public static class Logger
    {
        private static ILoggerService? _instance;
        private static readonly object _lock = new();

        public static ILoggerService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new FileLoggerService();
                    }
                }
                return _instance;
            }
        }

        // Convenience methods for team productivity
        public static void Info(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
            => Instance.LogInfo(message, methodName, filePath);

        public static void Warning(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
            => Instance.LogWarning(message, methodName, filePath);

        public static void Error(string message, Exception? exception = null, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
            => Instance.LogError(message, exception, methodName, filePath);

        public static void Debug(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
            => Instance.LogDebug(message, methodName, filePath);

        public static void TestStep(string stepDescription, string status = "EXECUTING")
            => Instance.LogTestStep(stepDescription, status);

        public static void TestResult(string testName, bool passed, string details = "")
            => Instance.LogTestResult(testName, passed, details);
    }
}