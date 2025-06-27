namespace Autonomode.WindowsDesktop.Core.Models
{
    /// <summary>
    /// Dynamic test data model that allows flexible properties per test case
    /// Each test case ID maps to a dynamic object with custom properties
    /// Leadership tip: Flexible data models reduce maintenance overhead for teams
    /// </summary>
    public class DynamicTestData
    {
        /// <summary>
        /// Dictionary containing test case data where key is TEST-001, TEST-002, etc.
        /// Value is a dynamic object with custom properties per test
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> TestCases { get; set; } = new();

        /// <summary>
        /// Global variables available across all test cases
        /// Useful for environment-specific constants
        /// </summary>
        public Dictionary<string, object> GlobalVariables { get; set; } = new();

        /// <summary>
        /// Gets test data for a specific test case ID
        /// </summary>
        /// <param name="testCaseId">Test case identifier (e.g., "TEST-001")</param>
        /// <returns>Test case data dictionary or null if not found</returns>
        public Dictionary<string, object>? GetTestCaseData(string testCaseId)
        {
            return TestCases.TryGetValue(testCaseId, out var data) ? data : null;
        }

        /// <summary>
        /// Checks if test case exists in the data
        /// </summary>
        /// <param name="testCaseId">Test case identifier</param>
        /// <returns>True if test case exists</returns>
        public bool HasTestCase(string testCaseId)
        {
            return TestCases.ContainsKey(testCaseId);
        }

        /// <summary>
        /// Gets all available test case IDs
        /// </summary>
        /// <returns>Collection of test case identifiers</returns>
        public IEnumerable<string> GetAvailableTestCases()
        {
            return TestCases.Keys;
        }
    }
}
