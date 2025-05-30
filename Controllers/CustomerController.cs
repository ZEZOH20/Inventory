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
namespace Inventory.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class CustomerController : ControllerBase
    {
        readonly ICustomerCrudService _CustomerCrudService;
        readonly IValidator<UserUpdateDTO> _UpdateDTOValidator;
        readonly IValidator<UserCreateDTO> _CreateDTOValidator;
        public CustomerController(
            ICustomerCrudService customerCrudService,
            IValidator<UserUpdateDTO> UpdateDTOValidator,
            IValidator<UserCreateDTO> CreateDTOValidator
            )
        {
            _CustomerCrudService = customerCrudService;
            _UpdateDTOValidator = UpdateDTOValidator;
            _CreateDTOValidator = CreateDTOValidator;
        }
        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            try
            {
                var responseDTO = _CustomerCrudService
                    .SelectAll()
                    .Select(customer => new UserResponseDTO(customer))
                    .ToList();

                return Ok(responseDTO);
            }
            catch (Exception ex)
            {
                return BadRequest("Can't return Users" + ex.Message);
            }
        }
        [HttpPost("create")]
        public IActionResult Create([FromBody] UserCreateDTO dto)
        {
            //validation
            var validationResult = _CreateDTOValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            try
            {
                var userExists = _CustomerCrudService.CheckExistByMail(dto);


                if (userExists)
                    return Conflict($"User Mail already exists try to change");


                _CustomerCrudService.Create(dto);
                return Ok("User Created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Create User" + ex.Message);
            }
        }

        [HttpPut("Update")]
        public IActionResult UpdateBYId([FromBody] UserUpdateDTO dto)
        {
            //validation
            var validationResult = _UpdateDTOValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            try
            {
                var result = _CustomerCrudService.UpdateById(dto);

                return !result ?
                     NotFound($"User with ID {dto.Id} not found") :
                     Ok("User updated successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't Update Users" + ex.Message);
            }
        }
        [HttpDelete("delete/{id}")]
        public IActionResult UpdateBYId(int id)
        {
            //validate ID
            if (id <= 0)
                return BadRequest($"the user id {id} can't be zero and should be positive ");

            try
            {
                var result = _CustomerCrudService.Delete(id);

                return !result ?
                     NotFound($"User with ID {id} not found") :
                     Ok("User deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Can't delete User" + ex.Message);
            }
        }
    }
}
