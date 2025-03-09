using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OoS.TestProject.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator? _mediator;

        protected IMediator Mediator => _mediator ??=
            HttpContext.RequestServices.GetService<IMediator>()!;

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                if (result.Value == null)
                {
                    return NotFound("Found result matching null");
                }

                return Ok(result.Value);
            }

            return BadRequest(result.Reasons);
        }
    }
}
