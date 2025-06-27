using Autonomode.WindowsDesktop.Core.Repositories;
using Autonomode.WindowsDesktop.Core.Models;

namespace Autonomode.WindowsDesktop.Core.Factories
{
    /// <summary>
    /// Service locator for easy access to test data context
    /// Provides thread-safe access to current test case data
    /// Leadership tip: Centralized access patterns reduce team confusion
    /// </summary>
    public static class TestDataService
    {
        private static ITestDataRepository? _repository;
        private static TestDataContext? _context;
        private static readonly object _lock = new();

        /// <summary>
        /// Gets the test data repository instance
        /// Thread-safe singleton implementation
        /// </summary>
        public static ITestDataRepository Repository
        {
            get
            {
                if (_repository == null)
                {
                    lock (_lock)
                    {
                        _repository ??= TestDataFactory.CreateRepository();
                    }
                }
                return _repository;
            }
        }

        /// <summary>
        /// Gets the test data context for easy data access
        /// Provides the simple API: testData.GetTestData("username")
        /// </summary>
        public static async Task<TestDataContext> GetContextAsync()
        {
            if (_context == null)
            {
                lock (_lock)
                {
                    if (_context == null)
                    {
                        var repository = Repository;
                        _context = repository.LoadTestDataContextAsync().GetAwaiter().GetResult();
                    }
                }
            }
            return _context;
        }

        /// <summary>
        /// Initializes test data context for the current test case
        /// Must be called at the beginning of each test
        /// Usage: await TestDataService.InitializeTestCase("TEST-001");
        /// </summary>
        /// <param name="testCaseId">Test case identifier</param>
        /// <returns>Initialized test data context</returns>
        public static async Task<TestDataContext> InitializeTestCaseAsync(string testCaseId)
        {
            var context = await GetContextAsync();
            context.SetCurrentTestCase(testCaseId);

            Console.WriteLine($"🧪 Test data context initialized for test case: {testCaseId}");
            return context;
        }

        /// <summary>
        /// Quick access method for getting test data
        /// Usage: var username = await TestDataService.GetTestDataAsync&lt;string&gt;("TEST-001", "username");
        /// </summary>
        /// <typeparam name="T">Expected data type</typeparam>
        /// <param name="testCaseId">Test case identifier</param>
        /// <param name="key">Data key</param>
        /// <returns>Typed test data value</returns>
        public static async Task<T> GetTestDataAsync<T>(string testCaseId, string key)
        {
            var context = await InitializeTestCaseAsync(testCaseId);
            return context.GetTestData<T>(key);
        }

        /// <summary>
        /// Quick access method for getting test data with default value
        /// Usage: var timeout = await TestDataService.GetTestDataOrDefaultAsync("TEST-001", "timeout", 30);
        /// </summary>
        /// <typeparam name="T">Expected data type</typeparam>
        /// <param name="testCaseId">Test case identifier</param>
        /// <param name="key">Data key</param>
        /// <param name="defaultValue">Default value if key not found</param>
        /// <returns>Test data value or default</returns>
        public static async Task<T> GetTestDataOrDefaultAsync<T>(string testCaseId, string key, T defaultValue)
        {
            try
            {
                return await GetTestDataAsync<T>(testCaseId, key);
            }
            catch (TestDataException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Validates that a test case exists in test data
        /// Useful for test discovery and setup validation
        /// </summary>
        /// <param name="testCaseId">Test case identifier</param>
        /// <returns>True if test case exists</returns>
        public static async Task<bool> ValidateTestCaseAsync(string testCaseId)
        {
            return await Repository.ValidateTestCaseExistsAsync(testCaseId);
        }

        /// <summary>
        /// Gets all available test case IDs
        /// Useful for dynamic test generation and reporting
        /// </summary>
        /// <returns>Collection of available test case identifiers</returns>
        public static async Task<IEnumerable<string>> GetAvailableTestCasesAsync()
        {
            return await Repository.GetAvailableTestCasesAsync();
        }

        /// <summary>
        /// Resets the service state - useful for testing and environment switches
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                _repository = null;
                _context = null;
            }
        }

        /// <summary>
        /// Sets a custom repository instance - useful for testing
        /// </summary>
        /// <param name="repository">Custom repository instance</param>
        public static void SetRepository(ITestDataRepository repository)
        {
            lock (_lock)
            {
                _repository = repository;
                _context = null; // Reset context when repository changes
            }
        }
    }
}