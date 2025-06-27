using Autonomode.WindowsDesktop.Core.Models;

namespace Autonomode.WindowsDesktop.Core.Repositories
{
    /// <summary>
    /// Interface for dynamic test data repository
    /// Simplified to focus on flexible test data access
    /// Leadership tip: Simple interfaces are easier for teams to implement and test
    /// </summary>
    public interface ITestDataRepository
    {
        Task<TestDataContext> LoadTestDataContextAsync();
        Task<DynamicTestData> LoadTestDataAsync();
        Task<bool> ValidateTestCaseExistsAsync(string testCaseId);
        Task<IEnumerable<string>> GetAvailableTestCasesAsync();
    }
}