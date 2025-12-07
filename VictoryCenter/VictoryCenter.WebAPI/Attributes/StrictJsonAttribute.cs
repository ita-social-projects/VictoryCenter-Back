using Microsoft.AspNetCore.Mvc;
using VictoryCenter.WebAPI.Filters;

namespace VictoryCenter.WebAPI.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class StrictJsonAttribute : ServiceFilterAttribute
{
    public StrictJsonAttribute()
        : base(typeof(StrictJsonValidationFilter))
    {
    }
}
