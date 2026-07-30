using FluentValidation;

namespace ProxyChecker.Loaders.UriTextFile;

internal class LoaderSettingsValidator : AbstractValidator<LoaderSettings>
{
  public LoaderSettingsValidator()
  {
    RuleFor(s => s.FilePath)
      .NotEmpty().WithMessage(Resource.EmptyFilePathValidationMessage)
      .Must(path => File.Exists(path)).WithMessage(Resource.FileNotFoundValidationMessage);
  }
}
