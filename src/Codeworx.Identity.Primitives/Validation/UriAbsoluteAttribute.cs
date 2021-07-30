using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.Validation
{
    public class UriAbsoluteAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var validationResult = new ValidationResult($"{Constants.OAuth.RedirectUriName} not URI or relative.", new[] { validationContext.MemberName });

            if (value is string uriString)
            {
                if (string.IsNullOrWhiteSpace(uriString))
                {
                    return ValidationResult.Success;
                }

                return Uri.TryCreate(uriString, UriKind.Absolute, out _)
                           ? ValidationResult.Success
                           : validationResult;
            }

            return value is null
                       ? ValidationResult.Success
                       : validationResult;
        }
    }
}
