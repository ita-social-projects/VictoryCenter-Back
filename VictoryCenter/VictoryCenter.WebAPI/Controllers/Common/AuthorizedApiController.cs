using Microsoft.AspNetCore.Authorization;

namespace VictoryCenter.WebAPI.Controllers.Common;

[Authorize]
public class AuthorizedApiController : BaseApiController
{
}
