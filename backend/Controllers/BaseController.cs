using Microsoft.AspNetCore.Mvc;
using Inventory.Shares;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected IActionResult HandleResponse<T>(Response<T> response)
        {
            if (response.IsSuccess)
            {
                return Ok(response.Data);
            }
            return BadRequest(response.Message);
        }

        protected IActionResult HandleResponse(Response response)
        {
            if (response.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(response.Message);
        }
    }
}