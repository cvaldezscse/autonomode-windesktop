using System.Text.Json;

namespace Autonomode.WindowsDesktop.Core.Models
{
    /// <summary>
    /// Test data context that maintains current test case and provides easy access
    /// Implements thread-safe context switching for parallel test execution
    /// </summary>
    public class TestDataContext
    {
        private static readonly ThreadLocal<string> _currentTestCaseId = new(() => string.Empty);
        private readonly DynamicTestData _testData;

        public TestDataContext(DynamicTestData testData)
        {
            _testData = testData ?? throw new ArgumentNullException(nameof(testData));
        }

        /// <summary>
        /// Sets the current test case context for this thread
        /// Must be called before accessing test data in each test
        /// </summary>
        /// <param name="testCaseId">Current test case identifier</param>
        public void SetCurrentTestCase(string testCaseId)
        {
            if (string.IsNullOrWhiteSpace(testCaseId))
            {
                throw new ArgumentException("Test case ID cannot be null or empty", nameof(testCaseId));
            }

            if (!_testData.HasTestCase(testCaseId))
            {
                throw new TestDataException($"Test case '{testCaseId}' not found in test data. Available test cases: {string.Join(", ", _testData.GetAvailableTestCases())}");
            }

            _currentTestCaseId.Value = testCaseId;
        }

        /// <summary>
        /// Gets the current test case ID for this thread
        /// </summary>
        public string CurrentTestCaseId => _currentTestCaseId.Value ?? string.Empty;

        /// <summary>
        /// Gets test data value for the current test case
        /// Provides simple access: testData.GetTestData("username")
        /// </summary>
        /// <param name="key">Property key to retrieve</param>
        /// <returns>Property value or throws exception if not found</returns>
        public object GetTestData(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Data key cannot be null or empty", nameof(key));
            }

            var currentTestCase = _currentTestCaseId.Value;
            if (string.IsNullOrEmpty(currentTestCase))
            {
                throw new TestDataException("No current test case set. Call SetCurrentTestCase() first.");
            }

            var testCaseData = _testData.GetTestCaseData(currentTestCase);
            if (testCaseData == null)
            {
                throw new TestDataException($"Test case '{currentTestCase}' not found in test data.");
            }

            if (!testCaseData.TryGetValue(key, out var value))
            {
                var availableKeys = string.Join(", ", testCaseData.Keys);
                throw new TestDataException($"Key '{key}' not found in test case '{currentTestCase}'. Available keys: {availableKeys}");
            }

            return value;
        }

        /// <summary>
        /// Gets strongly typed test data value
        /// Usage: var username = testData.GetTestData&lt;string&gt;("username");
        /// </summary>
        /// <typeparam name="T">Expected data type</typeparam>
        /// <param name="key">Property key to retrieve</param>
        /// <returns>Typed property value</returns>
        public T GetTestData<T>(string key)
        {
            var value = GetTestData(key);

            try
            {
                // Handle JsonElement conversion for complex types
                if (value is JsonElement jsonElement)
                {
                    return JsonSerializer.Deserialize<T>(jsonElement.GetRawText())
                           ?? throw new TestDataException($"Failed to deserialize value for key '{key}' to type {typeof(T).Name}");
                }

                // Direct conversion for simple types
                if (value is T directValue)
                {
                    return directValue;
                }

                // Try conversion for compatible types
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex) when (!(ex is TestDataException))
            {
                throw new TestDataException($"Cannot convert value for key '{key}' from {value?.GetType().Name ?? "null"} to {typeof(T).Name}. Value: {value}", ex);
            }
        }

        /// <summary>
        /// Safely gets test data with default value if not found
        /// Usage: var timeout = testData.GetTestDataOrDefault("timeout", 30);
        /// </summary>
        /// <typeparam name="T">Expected data type</typeparam>
        /// <param name="key">Property key to retrieve</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Property value or default</returns>
        public T GetTestDataOrDefault<T>(string key, T defaultValue)
        {
            try
            {
                return GetTestData<T>(key);
            }
            catch (TestDataException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Checks if test data key exists in current test case
        /// </summary>
        /// <param name="key">Property key to check</param>
        /// <returns>True if key exists</returns>
        public bool HasTestData(string key)
        {
            try
            {
                GetTestData(key);
                return true;
            }
            catch (TestDataException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets global variable value
        /// </summary>
        /// <param name="key">Global variable key</param>
        /// <returns>Global variable value</returns>
        public object GetGlobalVariable(string key)
        {
            if (!_testData.GlobalVariables.TryGetValue(key, out var value))
            {
                var availableKeys = string.Join(", ", _testData.GlobalVariables.Keys);
                throw new TestDataException($"Global variable '{key}' not found. Available variables: {availableKeys}");
            }
            return value;
        }

        /// <summary>
        /// Gets strongly typed global variable
        /// </summary>
        /// <typeparam name="T">Expected data type</typeparam>
        /// <param name="key">Global variable key</param>
        /// <returns>Typed global variable value</returns>
        public T GetGlobalVariable<T>(string key)
        {
            var value = GetGlobalVariable(key);

            try
            {
                if (value is JsonElement jsonElement)
                {
                    return JsonSerializer.Deserialize<T>(jsonElement.GetRawText())
                           ?? throw new TestDataException($"Failed to deserialize global variable '{key}' to type {typeof(T).Name}");
                }

                if (value is T directValue)
                {
                    return directValue;
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex) when (!(ex is TestDataException))
            {
                throw new TestDataException($"Cannot convert global variable '{key}' from {value?.GetType().Name ?? "null"} to {typeof(T).Name}", ex);
            }
        }
    }
}
