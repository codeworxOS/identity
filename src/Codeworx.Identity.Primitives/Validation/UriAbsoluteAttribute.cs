using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.Validation
{
    public class UriAbsoluteAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            return Uri.TryCreate(value as string, UriKind.Absolute, out _)
                       ? ValidationResult.Success
                       : new ValidationResult($"{OAuth.Constants.RedirectUriName} not URI or relative.", new[] { validationContext.MemberName });
        }
    }
}
