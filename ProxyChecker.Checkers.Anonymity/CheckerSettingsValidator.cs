using FluentValidation;

namespace ProxyChecker.Checkers.Anonymity;

internal class CheckerSettingsValidator : AbstractValidator<CheckerSettings>
{
  public CheckerSettingsValidator()
  {
    RuleFor(s => s.Timeout)
      .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)).WithMessage(Resource.TimeoutValidationMessage);
  }
}
