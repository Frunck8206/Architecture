using Architecture.CrossCutting;
using Architecture.Model;
using DotNetCore.AspNetCore;
using DotNetCore.Extensions;
using DotNetCore.Results;
using Microsoft.AspNetCore.Mvc;

namespace Architecture.Web
{
    public abstract class BaseController : ControllerBase
    {
        protected SignedInModel SignedInModel => new SignedInModel
        {
            UserId = User.Id(),
            Roles = User.RolesFlag<Roles>()
        };

        public static IActionResult Result(IResult result)
        {
            return ApiResultX.Create(result);
        }

        public static IActionResult Result(object data)
        {
            return ApiResultX.Create(data);
        }
    }
}
