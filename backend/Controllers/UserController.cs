using Inventory.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Inventory.DTO;
using Inventory.Services;
using Inventory.DTO.UserDto.Requests;
using Inventory.DTO.UserDto.Responses;
using FluentValidation;
using Inventory.DTO.UserDto.Validations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize(Roles = "Owner")]
    public class UserController : PersonCrudController<IUserCrudService, User>
    {
        public UserController(
            IUserCrudService UserCrudService,
            IValidator<UserUpdateDTO> UpdateDTOValidator,
            IValidator<UserCreateDTO> CreateDTOValidator
            ) : base(UserCrudService, CreateDTOValidator, UpdateDTOValidator)
        {
        }
    }
}
