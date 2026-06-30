using Microsoft.Extensions.DependencyInjection;

namespace VictoryCenter.BLL.Behaviors;

public static class ValidationBehaviorConfiguration
{
    public static MediatRServiceConfiguration AddBehaviors(this MediatRServiceConfiguration mediatRServiceConfiguration)
    {
        mediatRServiceConfiguration.AddOpenBehavior(typeof(ValidationBehavior<,>));

        return mediatRServiceConfiguration;
    }
}
