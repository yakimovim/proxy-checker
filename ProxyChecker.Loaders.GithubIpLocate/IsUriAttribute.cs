using System.ComponentModel.DataAnnotations;

namespace ProxyChecker.Loaders.GithubIpLocate;

internal class IsUriAttribute : ValidationAttribute
{
	protected override ValidationResult? IsValid(
	   object? value, ValidationContext context)
	{
		if (value is not string strValue)
		{
			return ValidationResult.Success;
		}

		if (string.IsNullOrEmpty(strValue))
		{
			return ValidationResult.Success;
		}

		if (!Uri.TryCreate(strValue, UriKind.Absolute, out var _))
		{
        return new ValidationResult(
          ErrorMessage ?? Resource.IsUriErrorMessage
        );
      }

      return ValidationResult.Success;
	}
}
