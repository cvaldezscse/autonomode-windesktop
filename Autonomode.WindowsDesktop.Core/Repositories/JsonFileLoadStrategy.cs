using System.Text.Json;
using Autonomode.WindowsDesktop.Core.Models;
using Autonomode.WindowsDesktop.Core.Services;

namespace Autonomode.WindowsDesktop.Core.Repositories
{

    /// <summary>
    /// JSON file-based loading strategy for dynamic test data
    /// Supports flexible test case structures with robust error handling
    /// </summary>
    public class JsonFileLoadStrategy : ITestDataLoadStrategy
    {
        private readonly string _basePath;
        private readonly string _testDataFile;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonFileLoadStrategy()
        {
            var config = ConfigurationService.Instance.TestData;
            _basePath = config.BasePath;
            _testDataFile = config.UsersDataFile; // Repurposing for main test data file

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
        }

        /// <summary>
        /// Loads test data from JSON file with comprehensive error handling
        /// Validates JSON structure and provides meaningful error messages
        /// </summary>
        public async Task<DynamicTestData> LoadAsync()
        {
            var testDataPath = Path.Combine(_basePath, _testDataFile);

            if (!File.Exists(testDataPath))
            {
                throw new TestDataException($"Test data file not found: {testDataPath}");
            }

            try
            {
                var jsonContent = await File.ReadAllTextAsync(testDataPath);

                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    throw new TestDataException($"Test data file is empty: {testDataPath}");
                }

                // Parse JSON into flexible structure
                var jsonDocument = JsonDocument.Parse(jsonContent);
                var rootElement = jsonDocument.RootElement;

                var testData = new DynamicTestData();

                // Process each property in the root JSON object
                foreach (var property in rootElement.EnumerateObject())
                {
                    var key = property.Name;
                    var value = property.Value;

                    // Check if this is a test case (format: TEST-XXX) or global variable
                    if (IsTestCaseId(key))
                    {
                        var testCaseData = ParseTestCaseData(value);
                        testData.TestCases[key] = testCaseData;
                    }
                    else if (key.Equals("globalVariables", StringComparison.OrdinalIgnoreCase))
                    {
                        var globalVars = ParseGlobalVariables(value);
                        testData.GlobalVariables = globalVars;
                    }
                    else
                    {
                        // Treat unknown root properties as global variables
                        testData.GlobalVariables[key] = ParseJsonValue(value);
                    }
                }

                ValidateTestData(testData);
                return testData;
            }
            catch (JsonException ex)
            {
                throw new TestDataException($"Invalid JSON format in test data file '{testDataPath}': {ex.Message}", ex);
            }
            catch (Exception ex) when (!(ex is TestDataException))
            {
                throw new TestDataException($"Failed to load test data from '{testDataPath}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if a key represents a test case ID (e.g., TEST-001, TC-001, etc.)
        /// </summary>
        private static bool IsTestCaseId(string key)
        {
            return key.StartsWith("TEST-", StringComparison.OrdinalIgnoreCase) ||
                   key.StartsWith("TC-", StringComparison.OrdinalIgnoreCase) ||
                   key.StartsWith("TESTCASE-", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses test case data from JSON element into flexible dictionary
        /// </summary>
        private Dictionary<string, object> ParseTestCaseData(JsonElement testCaseElement)
        {
            var testCaseData = new Dictionary<string, object>();

            if (testCaseElement.ValueKind != JsonValueKind.Object)
            {
                throw new TestDataException($"Test case data must be a JSON object, got {testCaseElement.ValueKind}");
            }

            foreach (var property in testCaseElement.EnumerateObject())
            {
                testCaseData[property.Name] = ParseJsonValue(property.Value);
            }

            return testCaseData;
        }

        /// <summary>
        /// Parses global variables from JSON element
        /// </summary>
        private Dictionary<string, object> ParseGlobalVariables(JsonElement globalVarsElement)
        {
            var globalVars = new Dictionary<string, object>();

            if (globalVarsElement.ValueKind != JsonValueKind.Object)
            {
                throw new TestDataException($"Global variables must be a JSON object, got {globalVarsElement.ValueKind}");
            }

            foreach (var property in globalVarsElement.EnumerateObject())
            {
                globalVars[property.Name] = ParseJsonValue(property.Value);
            }

            return globalVars;
        }

        /// <summary>
        /// Converts JsonElement to appropriate .NET type
        /// Handles all JSON value types with proper type mapping
        /// </summary>
        private object ParseJsonValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.Number => element.TryGetInt64(out var longVal) ? longVal : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null!,
                JsonValueKind.Array => element.EnumerateArray().Select(ParseJsonValue).ToList(),
                JsonValueKind.Object => element, // Keep as JsonElement for later parsing
                _ => element.GetRawText()
            };
        }

        /// <summary>
        /// Validates test data structure and content
        /// Catches common issues early to prevent runtime errors
        /// </summary>
        private static void ValidateTestData(DynamicTestData testData)
        {
            if (testData.TestCases.Count == 0)
            {
                throw new TestDataException("No test cases found in test data. Ensure test case IDs follow format: TEST-001, TEST-002, etc.");
            }

            // Validate test case IDs format
            foreach (var testCaseId in testData.TestCases.Keys)
            {
                if (!IsTestCaseId(testCaseId))
                {
                    Console.WriteLine($"⚠️ Warning: Test case ID '{testCaseId}' doesn't follow recommended format (TEST-XXX)");
                }
            }

            Console.WriteLine($"📋 Validation completed: {testData.TestCases.Count} test cases, {testData.GlobalVariables.Count} global variables");
        }
    }
}