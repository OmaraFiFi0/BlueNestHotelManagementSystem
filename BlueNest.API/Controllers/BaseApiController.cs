using BlueNest.Shared.Reponse;
using Microsoft.AspNetCore.Mvc;

namespace BlueNest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController:ControllerBase
    {

        protected ActionResult HandleResult<T>(GenericResponse<T> response)
        {
            if (response is null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return response.StatusCode switch
            {
                StatusCodes.Status200OK => Ok(response),
                StatusCodes.Status400BadRequest => BadRequest(response),
                StatusCodes.Status401Unauthorized => Unauthorized(response),
                StatusCodes.Status403Forbidden => Forbid(),
                StatusCodes.Status404NotFound => NotFound(response),
                StatusCodes.Status500InternalServerError => StatusCode(StatusCodes.Status500InternalServerError),
                _ => StatusCode(response.StatusCode, response)
            };
        }
    }
}
