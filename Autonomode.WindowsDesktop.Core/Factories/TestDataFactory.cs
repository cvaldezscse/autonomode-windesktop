using Autonomode.WindowsDesktop.Core.Repositories;
using Autonomode.WindowsDesktop.Core.Services;

namespace Autonomode.WindowsDesktop.Core.Factories
{
    /// <summary>
    /// Factory for creating dynamic JSON-based test data repositories
    /// Implements Factory pattern with environment-specific optimizations
    /// Leadership tip: Factories centralize creation logic and improve testability
    /// </summary>
    public static class TestDataFactory
    {
        /// <summary>
        /// Creates repository instance based on current environment configuration
        /// Automatically selects optimal strategy for each environment
        /// </summary>
        /// <returns>Configured ITestDataRepository instance</returns>
        public static ITestDataRepository CreateRepository()
        {
            var environment = ConfigurationService.Instance.Environment.ToLowerInvariant();

            return environment switch
            {
                "dev" or "development" => CreateDevelopmentRepository(),
                "staging" or "stage" => CreateStagingRepository(),
                "prod" or "production" => CreateProductionRepository(),
                _ => CreateDefaultRepository()
            };
        }

        /// <summary>
        /// Creates repository for development environment
        /// Uses JSON files with extended validation for debugging
        /// </summary>
        private static ITestDataRepository CreateDevelopmentRepository()
        {
            var loadStrategy = new JsonFileLoadStrategy();
            var repository = new JsonTestDataRepository(loadStrategy);

            Console.WriteLine("🔧 JsonTestDataRepository created for DEVELOPMENT environment");
            return repository;
        }

        /// <summary>
        /// Creates repository for staging environment
        /// Optimized for performance testing and pre-production validation
        /// </summary>
        private static ITestDataRepository CreateStagingRepository()
        {
            var loadStrategy = new JsonFileLoadStrategy();
            var repository = new JsonTestDataRepository(loadStrategy);

            Console.WriteLine("🧪 JsonTestDataRepository created for STAGING environment");
            return repository;
        }

        /// <summary>
        /// Creates repository for production environment
        /// Could be extended to use secure data sources or encrypted files
        /// </summary>
        private static ITestDataRepository CreateProductionRepository()
        {
            // In production, consider using encrypted JSON or database sources
            var loadStrategy = new JsonFileLoadStrategy();
            var repository = new JsonTestDataRepository(loadStrategy);

            Console.WriteLine("🚀 JsonTestDataRepository created for PRODUCTION environment");
            return repository;
        }

        /// <summary>
        /// Fallback repository creation for unknown environments
        /// </summary>
        private static ITestDataRepository CreateDefaultRepository()
        {
            var loadStrategy = new JsonFileLoadStrategy();
            var repository = new JsonTestDataRepository(loadStrategy);

            Console.WriteLine("⚠️ JsonTestDataRepository created with DEFAULT configuration");
            return repository;
        }

        /// <summary>
        /// Creates repository with custom strategy for testing purposes
        /// Useful for unit testing and custom data sources
        /// </summary>
        /// <param name="loadStrategy">Custom loading strategy</param>
        /// <returns>Repository with custom strategy</returns>
        public static ITestDataRepository CreateWithCustomStrategy(ITestDataLoadStrategy loadStrategy)
        {
            return new JsonTestDataRepository(loadStrategy);
        }
    }
}