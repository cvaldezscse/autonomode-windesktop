using Autonomode.WindowsDesktop.Core.Services;
using Autonomode.WindowsDesktop.Core.Factories;
using Autonomode.WindowsDesktop.Core.Models;
using Autonomode.WindowsDesktop.Core.Logging;

namespace Autonomode.WindowsDesktop.Demo
{
    /// <summary>
    /// Functional test class to validate the complete test data system
    /// Run this to verify everything works correctly
    /// </summary>
    public class TestDataDemo
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🚀 Starting Autonomode Test Data System Demo");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("📂 Reading configuration and test data from TestData project");
            Console.WriteLine("   Make sure these files exist:");
            Console.WriteLine("   - Autonomode.WindowsDesktop.TestData/Config/config-dev.yml");
            Console.WriteLine("   - Autonomode.WindowsDesktop.TestData/dev/testdata.json");
            Console.WriteLine();

            try
            {
                await RunCompleteDemo();
                Console.WriteLine("\n✅ Demo completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Demo failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static async Task RunCompleteDemo()
        {
            // Step 1: Initialize Configuration
            await DemoStep1_InitializeConfiguration();

            // Step 2: Basic Test Data Access
            await DemoStep2_BasicTestDataAccess();

            // Step 3: Complex Data Types
            await DemoStep3_ComplexDataTypes();

            // Step 4: Global Variables
            await DemoStep4_GlobalVariables();

            // Step 5: Error Handling
            await DemoStep5_ErrorHandling();

            // Step 6: Multiple Test Cases
            await DemoStep6_MultipleTestCases();
        }

        private static async Task DemoStep1_InitializeConfiguration()
        {
            Console.WriteLine("\n🔧 Step 1: Initializing Configuration");
            Console.WriteLine(new string('-', 40));

            try
            {
                // Try different possible paths for the config file
                var possiblePaths = new[]
                {
                    "./Config/config-dev.yml",  // If files are copied to output
                    "../TestData/Config/config-dev.yml",  // Relative to Demo project
                    "../../Source/TestData/Config/config-dev.yml",  // From bin directory
                    "../../../Autonomode.WindowsDesktop.TestData/Config/config-dev.yml"  // Alternative structure
                };

                string? configPath = null;
                foreach (var path in possiblePaths)
                {
                    var fullPath = Path.GetFullPath(path);
                    Console.WriteLine($"🔍 Checking: {fullPath}");

                    if (File.Exists(fullPath))
                    {
                        configPath = fullPath;
                        Console.WriteLine($"✅ Found config at: {configPath}");
                        break;
                    }
                }

                if (configPath == null)
                {
                    throw new FileNotFoundException(
                        $"Config file not found. Please ensure config-dev.yml exists in TestData/Config/ directory.\n" +
                        $"Current directory: {Directory.GetCurrentDirectory()}\n" +
                        $"Searched paths:\n{string.Join("\n", possiblePaths.Select(p => $"  - {Path.GetFullPath(p)}"))}");
                }

                // Set working directory to the config file's directory for relative paths to work
                var configDir = Path.GetDirectoryName(configPath)!;
                var testDataRoot = Path.GetDirectoryName(configDir)!;
                Directory.SetCurrentDirectory(testDataRoot);

                Console.WriteLine($"📂 Working directory set to: {Directory.GetCurrentDirectory()}");

                ConfigurationService.Instance.LoadConfiguration(Path.Combine(configDir, "config-dev.yml"), "dev");

                Console.WriteLine($"✅ Configuration loaded successfully");
                Console.WriteLine($"   Environment: {ConfigurationService.Instance.Environment}");
                Console.WriteLine($"   Test Data Path: {ConfigurationService.Instance.TestData.BasePath}");
                Console.WriteLine($"   Log Level: {ConfigurationService.Instance.Logging.Level}");

                Logger.Info("Configuration initialized for demo");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Configuration failed: {ex.Message}");
                Console.WriteLine($"\n💡 Setup Instructions:");
                Console.WriteLine($"   1. Create: Autonomode.WindowsDesktop.TestData/Config/config-dev.yml");
                Console.WriteLine($"   2. Create: Autonomode.WindowsDesktop.TestData/dev/testdata.json");
                Console.WriteLine($"   3. Add project reference from Demo to TestData");
                Console.WriteLine($"   4. Ensure files are set to 'Copy to Output Directory'");
                throw;
            }
        }

        private static async Task DemoStep2_BasicTestDataAccess()
        {
            Console.WriteLine("\n📊 Step 2: Basic Test Data Access");
            Console.WriteLine(new string('-', 40));

            try
            {
                // Initialize test case
                var testData = await TestDataService.InitializeTestCaseAsync("TEST-001");

                // Get basic string data
                var username = testData.GetTestData<string>("username");
                var password = testData.GetTestData<string>("password");
                var search = testData.GetTestData<string>("search");

                Console.WriteLine($"✅ Username: {username}");
                Console.WriteLine($"✅ Password: {password}");
                Console.WriteLine($"✅ Search: {search}");

                // Get array data
                var locations = testData.GetTestData<List<string>>("locations");
                Console.WriteLine($"✅ Locations: {string.Join(", ", locations)}");

                // Get numeric data
                var timeout = testData.GetTestData<int>("timeout");
                var retryAttempts = testData.GetTestData<int>("retryAttempts");

                Console.WriteLine($"✅ Timeout: {timeout} seconds");
                Console.WriteLine($"✅ Retry Attempts: {retryAttempts}");

                Logger.TestStep("Basic test data access validated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Basic access failed: {ex.Message}");
                throw;
            }
        }

        private static async Task DemoStep3_ComplexDataTypes()
        {
            Console.WriteLine("\n🔍 Step 3: Complex Data Types");
            Console.WriteLine(new string('-', 40));

            try
            {
                // Switch to TEST-002 for complex data
                var testData = await TestDataService.InitializeTestCaseAsync("TEST-002");

                // Get complex object
                var formData = testData.GetTestData<Dictionary<string, object>>("formData");
                Console.WriteLine($"✅ Form Data Retrieved:");

                foreach (var kvp in formData)
                {
                    Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
                }

                // Get array of strings
                var permissions = testData.GetTestData<List<string>>("permissions");
                Console.WriteLine($"✅ Permissions: {string.Join(", ", permissions)}");

                // Switch to TEST-003 for nested data
                testData = await TestDataService.InitializeTestCaseAsync("TEST-003");

                // Get nested navigation data
                var navigation = testData.GetTestData<Dictionary<string, object>>("navigation");
                var startPage = navigation["startPage"].ToString();
                Console.WriteLine($"✅ Start Page: {startPage}");

                Logger.TestStep("Complex data types validated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Complex data access failed: {ex.Message}");
                throw;
            }
        }

        private static async Task DemoStep4_GlobalVariables()
        {
            Console.WriteLine("\n🌐 Step 4: Global Variables");
            Console.WriteLine(new string('-', 40));

            try
            {
                var testData = await TestDataService.GetContextAsync();

                // Get global variables
                var environment = testData.GetGlobalVariable<string>("demoEnvironment");
                var baseUrl = testData.GetGlobalVariable<string>("baseUrl");
                var apiTimeout = testData.GetGlobalVariable<int>("apiTimeout");

                Console.WriteLine($"✅ Demo Environment: {environment}");
                Console.WriteLine($"✅ Base URL: {baseUrl}");
                Console.WriteLine($"✅ API Timeout: {apiTimeout}ms");

                // Get complex global variable
                var demoSettings = testData.GetGlobalVariable<Dictionary<string, object>>("demoSettings");
                Console.WriteLine($"✅ Demo Settings:");

                foreach (var kvp in demoSettings)
                {
                    Console.WriteLine($"   {kvp.Key}: {kvp.Value}");
                }

                Logger.TestStep("Global variables validated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Global variables failed: {ex.Message}");
                throw;
            }
        }

        private static async Task DemoStep5_ErrorHandling()
        {
            Console.WriteLine("\n⚠️ Step 5: Error Handling Validation");
            Console.WriteLine(new string('-', 40));

            try
            {
                var testData = await TestDataService.GetContextAsync();

                // Test 1: Missing test case
                Console.WriteLine("Testing missing test case...");
                try
                {
                    await TestDataService.InitializeTestCaseAsync("TEST-MISSING");
                    Console.WriteLine("❌ Should have thrown exception for missing test case");
                }
                catch (TestDataException ex)
                {
                    Console.WriteLine($"✅ Correctly caught missing test case: {ex.Message.Substring(0, 50)}...");
                }

                // Test 2: Missing data key
                Console.WriteLine("Testing missing data key...");
                await TestDataService.InitializeTestCaseAsync("TEST-001");
                testData = await TestDataService.GetContextAsync();

                try
                {
                    var missingValue = testData.GetTestData<string>("nonexistentKey");
                    Console.WriteLine("❌ Should have thrown exception for missing key");
                }
                catch (TestDataException ex)
                {
                    Console.WriteLine($"✅ Correctly caught missing key: {ex.Message.Substring(0, 50)}...");
                }

                // Test 3: Default values work
                Console.WriteLine("Testing default values...");
                var defaultValue = testData.GetTestDataOrDefault("missingKey", "default_value");
                Console.WriteLine($"✅ Default value working: {defaultValue}");

                Logger.TestStep("Error handling validated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error handling validation failed: {ex.Message}");
                throw;
            }
        }

        private static async Task DemoStep6_MultipleTestCases()
        {
            Console.WriteLine("\n📋 Step 6: Multiple Test Cases Management");
            Console.WriteLine(new string('-', 40));

            try
            {
                // Get all available test cases
                var availableTestCases = await TestDataService.GetAvailableTestCasesAsync();
                Console.WriteLine($"✅ Available test cases: {string.Join(", ", availableTestCases)}");

                // Validate each test case exists
                foreach (var testCaseId in availableTestCases)
                {
                    var exists = await TestDataService.ValidateTestCaseAsync(testCaseId);
                    Console.WriteLine($"✅ {testCaseId}: {(exists ? "Valid" : "Invalid")}");

                    // Quick access test
                    var username = await TestDataService.GetTestDataOrDefaultAsync<string>(testCaseId, "username", "N/A");
                    Console.WriteLine($"   Username: {username}");
                }

                Logger.TestStep("Multiple test cases validated");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Multiple test cases validation failed: {ex.Message}");
                throw;
            }
        }

        private static async Task DemoBonusStep_PerformanceTest()
        {
            Console.WriteLine("\n⚡ Bonus: Performance Test");
            Console.WriteLine(new string('-', 40));

            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                // Test loading performance
                for (int i = 0; i < 100; i++)
                {
                    var testData = await TestDataService.InitializeTestCaseAsync("TEST-001");
                    var username = testData.GetTestData<string>("username");
                }

                stopwatch.Stop();
                Console.WriteLine($"✅ 100 iterations completed in {stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"✅ Average per iteration: {stopwatch.ElapsedMilliseconds / 100.0:F2}ms");

                if (stopwatch.ElapsedMilliseconds < 1000)
                {
                    Console.WriteLine("✅ Performance: EXCELLENT");
                }
                else if (stopwatch.ElapsedMilliseconds < 3000)
                {
                    Console.WriteLine("✅ Performance: GOOD");
                }
                else
                {
                    Console.WriteLine("⚠️ Performance: NEEDS OPTIMIZATION");
                }

                Logger.TestStep("Performance test completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Performance test failed: {ex.Message}");
                throw;
            }
        }
    }
}