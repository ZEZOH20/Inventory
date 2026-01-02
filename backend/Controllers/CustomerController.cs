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
    public class CustomerController : PersonCrudController<ICustomerCrudService, Customer>
    {
        public CustomerController(
            ICustomerCrudService customerCrudService,
            IValidator<UserUpdateDTO> UpdateDTOValidator,
            IValidator<UserCreateDTO> CreateDTOValidator
            ) : base(customerCrudService, CreateDTOValidator, UpdateDTOValidator)
        {
        }
    }
}
