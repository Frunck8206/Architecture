using DotNetCore.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNetCore.AspNetCore
{
    public class ApiResultX : IActionResult
    {
        private readonly IResult _result;

        private ApiResultX(IResult result)
        {
            _result = result;
        }

        private ApiResultX(object data)
        {
            _result = DataResult<object>.Success(data);
        }

        public static IActionResult Create(IResult result)
        {
            return new ApiResultX(result);
        }

        public static IActionResult Create(object data)
        {
            return new ApiResultX(data);
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            object value = default;

            if (_result.Failed)
            {
                value = _result.Message;
            }
            else if (_result.GetType().IsGenericType && _result.GetType().GetGenericTypeDefinition() == typeof(DataResult<>))
            {
                value = (_result as dynamic)?.Data;
            }

            var objectResult = new ObjectResult(value)
            {
                StatusCode = _result.Succeeded ? StatusCodes.Status200OK : StatusCodes.Status422UnprocessableEntity
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
}
