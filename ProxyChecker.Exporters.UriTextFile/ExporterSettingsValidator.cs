using FluentValidation;

namespace ProxyChecker.Exporters.UriTextFile;

internal class ExporterSettingsValidator : AbstractValidator<ExporterSettings>
{
  public ExporterSettingsValidator()
  {
    RuleFor(s => s.FilePath)
      .NotEmpty().WithMessage(Resource.EmptyFilePathValidationMessage);
  }
}
