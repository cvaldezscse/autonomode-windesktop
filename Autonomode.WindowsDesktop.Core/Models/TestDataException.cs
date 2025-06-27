namespace Autonomode.WindowsDesktop.Core.Models
{
    /// <summary>
    /// Custom exception for test data related errors
    /// Provides clear error messages for team debugging
    /// </summary>
    public class TestDataException : Exception
    {
        public TestDataException(string message) : base(message) { }
        public TestDataException(string message, Exception innerException) : base(message, innerException) { }
    }
}
