using System.Runtime.CompilerServices;
using Autonomode.WindowsDesktop.Core.Services;

namespace Autonomode.WindowsDesktop.Core.Logging
{
    /// <summary>
    /// File-based logger implementation with rotation and structured output
    /// Provides team-friendly logging format for both console and file output
    /// </summary>
    public class FileLoggerService : ILoggerService, IDisposable
    {
        private readonly StreamWriter? _fileWriter;
        private readonly bool _enableConsoleOutput;
        private readonly string _logLevel;
        private readonly object _lockObject = new();
        private bool _disposed = false;

        public FileLoggerService()
        {
            try
            {
                var config = ConfigurationService.Instance.Logging;
                _enableConsoleOutput = config.EnableConsoleOutput;
                _logLevel = config.Level;

                // Ensure log directory exists
                var logPath = config.FilePath;
                var logDirectory = Path.GetDirectoryName(logPath);

                if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Create file writer with UTF-8 encoding
                _fileWriter = new StreamWriter(logPath, append: true, encoding: System.Text.Encoding.UTF8)
                {
                    AutoFlush = true
                };

                LogInfo($"🚀 Logger initialized - Level: {_logLevel}, Console: {_enableConsoleOutput}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to initialize logger: {ex.Message}");
                // Continue without file logging if initialization fails
            }
        }

        public void LogInfo(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            WriteLog("INFO", message, methodName, filePath);
        }

        public void LogWarning(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            WriteLog("WARN", message, methodName, filePath);
        }

        public void LogError(string message, Exception? exception = null, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            var fullMessage = exception != null ? $"{message}\nException: {exception}" : message;
            WriteLog("ERROR", fullMessage, methodName, filePath);
        }

        public void LogDebug(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            if (ShouldLog("DEBUG"))
            {
                WriteLog("DEBUG", message, methodName, filePath);
            }
        }

        public void LogTestStep(string stepDescription, string status = "EXECUTING")
        {
            var message = $"🧪 STEP [{status}]: {stepDescription}";
            WriteLog("TEST", message, "TestStep", "");
        }

        public void LogTestResult(string testName, bool passed, string details = "")
        {
            var status = passed ? "✅ PASSED" : "❌ FAILED";
            var message = $"📊 RESULT [{status}]: {testName}";

            if (!string.IsNullOrEmpty(details))
            {
                message += $"\n   Details: {details}";
            }

            WriteLog("RESULT", message, "TestResult", "");
        }

        private void WriteLog(string level, string message, string methodName, string filePath)
        {
            if (!ShouldLog(level)) return;

            lock (_lockObject)
            {
                try
                {
                    var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var context = !string.IsNullOrEmpty(fileName) ? $"{fileName}.{methodName}" : methodName;

                    var logEntry = $"[{timestamp}] [{level,-6}] [{context}] {message}";

                    // Write to file
                    _fileWriter?.WriteLine(logEntry);

                    // Write to console with color coding
                    if (_enableConsoleOutput)
                    {
                        WriteColoredConsole(level, logEntry);
                    }
                }
                catch (Exception ex)
                {
                    // Fallback to console only if file writing fails
                    Console.WriteLine($"[LOG ERROR] {ex.Message}");
                    Console.WriteLine($"[ORIGINAL] {message}");
                }
            }
        }

        private void WriteColoredConsole(string level, string logEntry)
        {
            var originalColor = Console.ForegroundColor;

            try
            {
                Console.ForegroundColor = level switch
                {
                    "ERROR" => ConsoleColor.Red,
                    "WARN" => ConsoleColor.Yellow,
                    "INFO" => ConsoleColor.Green,
                    "DEBUG" => ConsoleColor.Gray,
                    "TEST" => ConsoleColor.Cyan,
                    "RESULT" => ConsoleColor.Magenta,
                    _ => ConsoleColor.White
                };

                Console.WriteLine(logEntry);
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }

        private bool ShouldLog(string level)
        {
            var levelHierarchy = new Dictionary<string, int>
            {
                { "DEBUG", 0 },
                { "INFO", 1 },
                { "WARN", 2 },
                { "ERROR", 3 },
                { "TEST", 1 },    // Test steps at INFO level
                { "RESULT", 1 }   // Test results at INFO level
            };

            var currentLevelValue = levelHierarchy.GetValueOrDefault(_logLevel.ToUpperInvariant(), 1);
            var messageLevelValue = levelHierarchy.GetValueOrDefault(level, 1);

            return messageLevelValue >= currentLevelValue;
        }

        public void Flush()
        {
            lock (_lockObject)
            {
                _fileWriter?.Flush();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                lock (_lockObject)
                {
                    LogInfo("🔧 Logger shutting down");
                    _fileWriter?.Flush();
                    _fileWriter?.Dispose();
                    _disposed = true;
                }
            }
        }
    }
}