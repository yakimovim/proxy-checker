using FluentValidation;

namespace ProxyChecker.Loaders.FlashProxyApi;

internal class LoaderSettingsValidator : AbstractValidator<LoaderSettings>
{
  public LoaderSettingsValidator()
  {
    RuleFor(s => s.Timeout)
      .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)).WithMessage(Resource.TimeoutValidationMessage);

    RuleFor(s => s.Limit)
      .GreaterThanOrEqualTo(1).WithMessage(Resource.LimitValidationMessage);

    RuleFor(s => s.SpeedMs)
      .GreaterThan(0).When(s => s.SpeedMs.HasValue).WithMessage(Resource.SpeedMsValidationMessage);
  }
}
