using FluentValidation;

namespace ProxyChecker.Loaders.GithubIpLocate;

internal class LoaderSettingsValidator : AbstractValidator<LoaderSettings>
{
  public LoaderSettingsValidator()
  {
    RuleFor(s => s.Timeout)
      .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)).WithMessage(Resource.TimeoutValidationMessage);
  }
}
