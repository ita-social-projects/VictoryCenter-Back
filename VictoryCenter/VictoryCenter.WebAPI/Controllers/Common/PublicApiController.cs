using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VictoryCenter.WebAPI.Extensions;

namespace VictoryCenter.WebAPI.Controllers.Common;

/// <summary>
/// Base class for anonymous public endpoints. Applies the default public rate limit to every action.
/// </summary>
/// <remarks>
/// An action can use a stricter policy with its own <see cref="EnableRateLimitingAttribute"/>:
/// the action-level attribute overrides this one.
/// </remarks>
[EnableRateLimiting(RateLimitingPolicyNameConstants.PublicRead)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
public abstract class PublicApiController : BaseApiController
{
}
