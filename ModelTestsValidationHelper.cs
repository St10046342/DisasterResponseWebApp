using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestProjectDisasterApp.ModelTests
{
    public static class ValidationHelper
    {
        public static IList<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);
            return results;
        }
    }
}
