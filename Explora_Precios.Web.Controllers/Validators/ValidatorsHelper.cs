using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Explora_Precios.Web.Controllers.Validators
{
    public static class ValidatorsHelper
    {
        public static void AddRuleViolations(this ModelStateDictionary modelState,
        IEnumerable<KeyValuePair<string, string>> errors)
        {
            foreach (var issue in errors)
            {
                modelState.AddModelError(issue.Key, issue.Value);
            }
        }

        public static bool isValid(this IEnumerable<KeyValuePair<string, string>> errors)
        {
            if (errors.Count() > 0)
                return false;
            return true;
        }
    }
}
