using Inventory.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Inventory.DTO;
using Inventory.Services;
namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : ControllerBase
    {
        readonly IUserCrudService _UserCrudService;
        public UserController (IUserCrudService UserCrudService) {
            _UserCrudService = UserCrudService;
        }
        [HttpGet("getAll")]
        public IActionResult getAll()
        {
            
            try
            {
                var responseDTOs = _UserCrudService
                    .SelectAll()
                    .Select(user => new UserResponseDTO(user))
                    .ToList();
                
                return Ok(responseDTOs);
            }
            catch (Exception ex)
            {
                return BadRequest("Can't return Users Something Wrong" + ex.Message);
            }
        }
        
    }
}
