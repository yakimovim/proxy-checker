using System.ComponentModel.DataAnnotations;

namespace ProxyChecker.Loaders.UriTextFile
{
	internal class FileExistsAttribute : ValidationAttribute
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

			if (!File.Exists(strValue))
				return new ValidationResult(
					ErrorMessage ?? $"File '{strValue}' does not exist");

			return ValidationResult.Success;
		}
	}
}
