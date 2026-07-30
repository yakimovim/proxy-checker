using FluentValidation;

namespace ProxyChecker.Checkers.OkResponse;

internal class CheckerSettingsValidator : AbstractValidator<CheckerSettings>
{
  public CheckerSettingsValidator()
  {
    RuleFor(s => s.Timeout)
      .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)).WithMessage(Resource.TimeoutValidationMessage);

    RuleFor(s => s.TargetUris)
      .NotEmpty().WithMessage(Resource.TargetUrisValidationMessage);
  }
}
