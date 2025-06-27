using Autonomode.WindowsDesktop.Core.Services;
using Autonomode.WindowsDesktop.Core.Factories;
using Autonomode.WindowsDesktop.Core.Models;
using Autonomode.WindowsDesktop.Core.Logging;

namespace Autonomode.WindowsDesktop.Tests.Examples
{
    /// <summary>
    /// Examples demonstrating how to use the dynamic test data system
    /// Leadership tip: Clear examples accelerate team adoption and reduce support overhead
    /// </summary>
    public class TestDataUsageExample
    {
        /// <summary>
        /// Example 1: Basic test data usage in a test method
        /// Shows the simple API: testData.GetTestData("username")
        /// </summary>
        public async Task BasicTestDataUsageExample()
        {
            try
            {
                // Initialize configuration (done once per test suite)
                ConfigurationService.Instance.LoadConfiguration("./config-dev.yml", "dev");

                // Initialize test data context for specific test case
                var testData = await TestDataService.InitializeTestCaseAsync("TEST-001");

                // Get test data using simple API
                var username = testData.GetTestData<string>("username");
                var password = testData.GetTestData<string>("password");
                var search = testData.GetTestData<string>("search");
                var locations = testData.GetTestData<List<string>>("locations");

                Logger.TestStep($"Using credentials: {username}");
                Logger.Info($"Search term: {search}");
                Logger.Info($"Available locations: {string.Join(", ", locations)}");

                // Your test logic here...
                // LoginToApplication(username, password);
                // PerformSearch(search);
                // ValidateLocations(locations);

                Logger.TestResult("TEST-001", true, "All test data retrieved successfully");
            }
            catch (TestDataException ex)
            {
                Logger.Error($"Test data error: {ex.Message}");
                Logger.TestResult("TEST-001", false, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Example 2: Advanced test data usage with complex objects
        /// Shows handling of nested objects and arrays
        /// </summary>
        public async Task AdvancedTestDataUsageExample()
        {
            try
            {
                var testData = await TestDataService.InitializeTestCaseAsync("TEST-004");

                // Get complex form data
                var formFields = testData.GetTestData<Dictionary<string, object>>("formFields");
                var validationRules = testData.GetTestData<Dictionary<string, object>>("validationRules");

                // Access nested properties
                var firstName = formFields["firstName"].ToString();
                var lastName = formFields["lastName"].ToString();
                var email = formFields["email"].ToString();

                Logger.TestStep($"Filling form for: {firstName} {lastName}");
                Logger.Info($"Email: {email}");

                // Get validation rules
                var requiredFields = testData.GetTestData<List<string>>("validationRules.required");
                var emailFormatRequired = testData.GetTestData<bool>("validationRules.emailFormat");

                Logger.Info($"Required fields: {string.Join(", ", requiredFields)}");
                Logger.Info($"Email validation required: {emailFormatRequired}");

                // Your test logic here...
                // FillForm(formFields);
                // ValidateRequiredFields(requiredFields);

                Logger.TestResult("TEST-004", true, "Complex form data processed successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Advanced test failed: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Example 3: Using default values and error handling
        /// Shows safe data access patterns for robust tests
        /// </summary>
        public async Task SafeTestDataUsageExample()
        {
            try
            {
                var testData = await TestDataService.InitializeTestCaseAsync("TEST-005");

                // Get data with default values if not found
                var timeout = testData.GetTestDataOrDefault("timeout", 30);
                var retryAttempts = testData.GetTestDataOrDefault("retryAttempts", 3);
                var debugMode = testData.GetTestDataOrDefault("debugMode", false);

                Logger.TestStep($"Configuration: timeout={timeout}s, retries={retryAttempts}, debug={debugMode}");

                // Check if optional data exists
                if (testData.HasTestData("specialInstructions"))
                {
                    var instructions = testData.GetTestData<string>("specialInstructions");
                    Logger.Info($"Special instructions: {instructions}");
                }

                // Get navigation data
                var navigation = testData.GetTestData<Dictionary<string, object>>("navigation");
                var startPage = navigation["startPage"].ToString();
                var menuSequence = testData.GetTestData<List<string>>("navigation.menuSequence");

                Logger.TestStep($"Starting navigation from: {startPage}");

                // Your test logic here...
                // NavigateToPage(startPage);
                // FollowMenuSequence(menuSequence);

                Logger.TestResult("TEST-005", true, "Safe navigation completed");
            }
            catch (TestDataException ex)
            {
                Logger.Warning($"Test data issue handled gracefully: {ex.Message}");
                // Continue with default behavior
            }
        }

        /// <summary>
        /// Example 4: Working with global variables
        /// Shows how to access environment-wide configuration
        /// </summary>
        public async Task GlobalVariablesUsageExample()
        {
            try
            {
                var testData = await TestDataService.GetContextAsync();

                // Access global variables
                var baseUrl = testData.GetGlobalVariable<string>("baseUrl");
                var apiTimeout = testData.GetGlobalVariable<int>("apiTimeout");
                var environment = testData.GetGlobalVariable<string>("environment");

                Logger.Info($"Environment: {environment}");
                Logger.Info($"Base URL: {baseUrl}");
                Logger.Info($"API Timeout: {apiTimeout}ms");

                // Get complex global configuration
                var browserSettings = testData.GetGlobalVariable<Dictionary<string, object>>("browserSettings");
                var windowSize = browserSettings["windowSize"] as Dictionary<string, object>;
                var width = Convert.ToInt32(windowSize["width"]);
                var height = Convert.ToInt32(windowSize["height"]);

                Logger.Info($"Browser window size: {width}x{height}");

                // Your global setup logic here...
                // ConfigureEnvironment(baseUrl, apiTimeout);
                // SetupBrowser(width, height);

                Logger.TestResult("Global Setup", true, "Global variables configured successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Global configuration failed: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Example 5: Quick access methods for simple scenarios
        /// Shows the most concise way to get test data
        /// </summary>
        public async Task QuickAccessExample()
        {
            try
            {
                // Quick one-liner access
                var username = await TestDataService.GetTestDataAsync<string>("TEST-001", "username");
                var password = await TestDataService.GetTestDataAsync<string>("TEST-001", "password");

                Logger.TestStep($"Quick login with: {username}");

                // Quick access with default
                var timeout = await TestDataService.GetTestDataOrDefaultAsync("TEST-001", "timeout", 30);

                Logger.Info($"Using timeout: {timeout} seconds");

                // Validate test case exists before running
                var exists = await TestDataService.ValidateTestCaseAsync("TEST-999");
                if (!exists)
                {
                    Logger.Warning("TEST-999 not found, skipping optional test");
                    return;
                }

                // Your quick test logic here...
                // QuickLogin(username, password, timeout);

                Logger.TestResult("Quick Access Test", true, "Quick access methods work perfectly");
            }
            catch (Exception ex)
            {
                Logger.Error($"Quick access failed: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Example 6: Error handling and debugging patterns
        /// Shows how to handle common test data issues
        /// </summary>
        public async Task ErrorHandlingExample()
        {
            try
            {
                // Example of handling missing test case
                try
                {
                    var testData2 = await TestDataService.InitializeTestCaseAsync("TEST-MISSING");
                }
                catch (TestDataException ex)
                {
                    Logger.Warning($"Expected error for missing test case: {ex.Message}");

                    // Show available test cases for debugging
                    var availableTestCases = await TestDataService.GetAvailableTestCasesAsync();
                    Logger.Info($"Available test cases: {string.Join(", ", availableTestCases)}");
                }

                // Example of handling missing data key
                var testData = await TestDataService.InitializeTestCaseAsync("TEST-001");

                try
                {
                    var missingValue = testData.GetTestData<string>("nonexistentKey");
                }
                catch (TestDataException ex)
                {
                    Logger.Warning($"Expected error for missing key: {ex.Message}");

                    // Use default value instead
                    var defaultValue = testData.GetTestDataOrDefault("nonexistentKey", "default");
                    Logger.Info($"Using default value: {defaultValue}");
                }

                // Example of handling type conversion errors
                try
                {
                    // Trying to convert string to int
                    var invalidNumber = testData.GetTestData<int>("username"); // This should be a string
                }
                catch (TestDataException ex)
                {
                    Logger.Warning($"Type conversion error handled: {ex.Message}");

                    // Get as string and parse manually if needed
                    var stringValue = testData.GetTestData<string>("username");
                    Logger.Info($"Retrieved as string: {stringValue}");
                }

                Logger.TestResult("Error Handling", true, "All error scenarios handled correctly");
            }
            catch (Exception ex)
            {
                Logger.Error($"Unexpected error in error handling example: {ex.Message}", ex);
                throw;
            }
        }
    }

    /// <summary>
    /// Example BDD Step Definition showing test data integration
    /// Leadership tip: Show how test data integrates with existing BDD framework
    /// </summary>
    public class TestDataStepDefinitions
    {
        // Note: [Given], [When], [Then] attributes would come from SpecFlow
        // Shown here as example - actual implementation in Phase 3

        // [Given(@"I am testing scenario ""(.*)""")]
        public async Task GivenIAmTestingScenario(string testCaseId)
        {
            try
            {
                var testData = await TestDataService.InitializeTestCaseAsync(testCaseId);
                Logger.TestStep($"Initialized test data for scenario: {testCaseId}");
            }
            catch (TestDataException ex)
            {
                Logger.Error($"Failed to initialize test data for {testCaseId}: {ex.Message}");
                throw;
            }
        }

        // [When(@"I login with test data credentials")]
        public async Task WhenILoginWithTestDataCredentials()
        {
            try
            {
                var testData = await TestDataService.GetContextAsync();
                var username = testData.GetTestData<string>("username");
                var password = testData.GetTestData<string>("password");

                Logger.TestStep($"Logging in with user: {username}");

                // Your login logic here
                // LoginPage.EnterUsername(username);
                // LoginPage.EnterPassword(password);
                // LoginPage.ClickLoginButton();
            }
            catch (TestDataException ex)
            {
                Logger.Error($"Login failed due to test data issue: {ex.Message}");
                throw;
            }
        }

        // [Then(@"I should see the expected result from test data")]
        public async Task ThenIShouldSeeTheExpectedResultFromTestData()
        {
            try
            {
                var testData = await TestDataService.GetContextAsync();
                var expectedResult = testData.GetTestData<string>("expectedResult");

                Logger.TestStep($"Validating expected result: {expectedResult}");

                // Your validation logic here
                // var actualResult = HomePage.GetWelcomeMessage();
                // Assert.AreEqual(expectedResult, actualResult);

                Logger.TestResult("Expected Result Validation", true, $"Result matched: {expectedResult}");
            }
            catch (TestDataException ex)
            {
                Logger.Error($"Validation failed due to test data issue: {ex.Message}");
                throw;
            }
        }
    }
}