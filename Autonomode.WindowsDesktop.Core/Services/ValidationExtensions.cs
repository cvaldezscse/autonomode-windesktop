using System.ComponentModel.DataAnnotations;

namespace Autonomode.WindowsDesktop.Core.Services
{
    /// <summary>
    /// Extension method for recursive validation
    /// Ensures nested objects are also validated
    /// </summary>
    public static class ValidationExtensions
    {
        public static bool TryValidateObjectRecursively<T>(T obj, ValidationContext context, IList<ValidationResult> results)
        {
            return Validator.TryValidateObject(obj, context, results, true);
        }
    }
}