using Autonomode.WindowsDesktop.Core.Models;

namespace Autonomode.WindowsDesktop.Core.Repositories
{
    /// <summary>
    /// Repository implementation for dynamic JSON-based test data
    /// Provides caching and efficient access to test data
    /// </summary>
    public class JsonTestDataRepository : ITestDataRepository
    {
        private readonly ITestDataLoadStrategy _loadStrategy;
        private DynamicTestData? _cachedData;
        private TestDataContext? _cachedContext;
        private DateTime _lastLoadTime = DateTime.MinValue;
        private readonly TimeSpan _cacheTimeout = TimeSpan.FromMinutes(5);
        private readonly object _lockObject = new();

        public JsonTestDataRepository(ITestDataLoadStrategy loadStrategy)
        {
            _loadStrategy = loadStrategy ?? throw new ArgumentNullException(nameof(loadStrategy));
        }

        /// <summary>
        /// Loads test data context with caching for performance
        /// Context provides thread-safe access to current test case data
        /// </summary>
        public async Task<TestDataContext> LoadTestDataContextAsync()
        {
            var testData = await LoadTestDataAsync();

            if (_cachedContext == null)
            {
                lock (_lockObject)
                {
                    _cachedContext ??= new TestDataContext(testData);
                }
            }

            return _cachedContext;
        }

        /// <summary>
        /// Loads raw test data with intelligent caching
        /// Avoids repeated file I/O for better performance in test suites
        /// </summary>
        public async Task<DynamicTestData> LoadTestDataAsync()
        {
            lock (_lockObject)
            {
                // Return cached data if still valid
                if (_cachedData != null && DateTime.UtcNow - _lastLoadTime < _cacheTimeout)
                {
                    return _cachedData;
                }
            }

            try
            {
                var testData = await _loadStrategy.LoadAsync();

                lock (_lockObject)
                {
                    _cachedData = testData;
                    _lastLoadTime = DateTime.UtcNow;

                    // Invalidate context cache when data changes
                    _cachedContext = null;
                }

                Console.WriteLine($"✅ Test data loaded successfully. Test cases: {testData.TestCases.Count}, Global variables: {testData.GlobalVariables.Count}");
                return testData;
            }
            catch (Exception ex)
            {
                throw new TestDataException($"Failed to load test data: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validates if a test case exists in the loaded data
        /// Useful for test discovery and validation phases
        /// </summary>
        public async Task<bool> ValidateTestCaseExistsAsync(string testCaseId)
        {
            if (string.IsNullOrWhiteSpace(testCaseId))
            {
                return false;
            }

            var testData = await LoadTestDataAsync();
            return testData.HasTestCase(testCaseId);
        }

        /// <summary>
        /// Gets all available test case identifiers
        /// Useful for test reporting and debugging
        /// </summary>
        public async Task<IEnumerable<string>> GetAvailableTestCasesAsync()
        {
            var testData = await LoadTestDataAsync();
            return testData.GetAvailableTestCases();
        }
    }
}