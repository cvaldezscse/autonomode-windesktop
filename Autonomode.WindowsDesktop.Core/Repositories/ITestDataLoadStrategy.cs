using Autonomode.WindowsDesktop.Core.Models;

namespace Autonomode.WindowsDesktop.Core.Repositories
{
    /// <summary>
    /// Strategy interface for loading dynamic test data
    /// Allows different data sources (JSON files, databases, APIs)
    /// </summary>
    public interface ITestDataLoadStrategy
    {
        Task<DynamicTestData> LoadAsync();
    }
}