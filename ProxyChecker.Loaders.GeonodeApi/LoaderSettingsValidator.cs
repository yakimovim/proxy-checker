using FluentValidation;

namespace ProxyChecker.Loaders.GeonodeApi;

internal class LoaderSettingsValidator : AbstractValidator<LoaderSettings>
{
  public LoaderSettingsValidator()
  {
    RuleFor(s => s.Timeout)
      .GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)).WithMessage(Resource.TimeoutValidationMessage);

    RuleFor(s => s.Limit)
      .GreaterThanOrEqualTo(1).WithMessage(Resource.LimitValidationMessage);

    RuleFor(s => s.Port)
      .GreaterThan(0).When(s => s.Port.HasValue).WithMessage(Resource.PortValidationMessage)
      .LessThanOrEqualTo(65535).When(s => s.Port.HasValue).WithMessage(Resource.PortValidationMessage);

    RuleFor(s => s.Uptime)
      .GreaterThan(0).When(s => s.Uptime.HasValue).WithMessage(Resource.UptimeValidationMessage)
      .LessThanOrEqualTo(100).When(s => s.Uptime.HasValue).WithMessage(Resource.UptimeValidationMessage);

    RuleFor(s => s.LastChecked)
      .GreaterThan(0).When(s => s.LastChecked.HasValue).WithMessage(Resource.LastCheckedValidationMessage);
  }
}
