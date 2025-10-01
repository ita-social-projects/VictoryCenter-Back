using System.Runtime.CompilerServices;
using FluentValidation;

namespace VictoryCenter.UnitTests.ValidatorsTests;

public static class ValidatorsConfig
{
    [ModuleInitializer]
    public static void Configure()
    {
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
    }
}
