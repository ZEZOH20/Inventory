using Microsoft.AspNetCore.Mvc;
using Inventory.Shares;
using Inventory.DTO.UserDto.Requests;
using Inventory.DTO.UserDto.Responses;
using Inventory.Services;
using Inventory.Models;
using FluentValidation;

namespace Inventory.Controllers
{
    public abstract class PersonCrudController<TService, TEntity> : BaseController
        where TService : IPersonCrudService<TEntity>
        where TEntity : Person
    {
        protected readonly TService _service;
        protected readonly IValidator<UserCreateDTO> _createValidator;
        protected readonly IValidator<UserUpdateDTO> _updateValidator;

        protected PersonCrudController(
            TService service,
            IValidator<UserCreateDTO> createValidator,
            IValidator<UserUpdateDTO> updateValidator)
        {
            _service = service;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet("getAll")]
        public IActionResult GetAll(int page = 1, int pageSize = 10)
        {
            try
            {
                var response = _service.SelectAll(page, pageSize);
                if (!response.IsSuccess)
                    return BadRequest(response.Message);

                var paginatedData = new
                {
                    Data = response.Data.Data.Select(entity => new UserResponseDTO((dynamic)entity)),
                    response.Data.Page,
                    response.Data.PageSize,
                    response.Data.TotalCount,
                    response.Data.TotalPages
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return BadRequest("Can't return data: " + ex.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] UserCreateDTO dto)
        {
            //validation
            var validationResult = _createValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            try
            {
                var entityExistsResponse = _service.CheckExistByMail(dto);
                if (!entityExistsResponse.IsSuccess)
                    return BadRequest(entityExistsResponse.Message);

                if (entityExistsResponse.Data)
                    return Conflict($"Entity already exists with this mail");

                var createResponse = _service.Create(dto);
                if (!createResponse.IsSuccess)
                    return BadRequest(createResponse.Message);
                return Ok("Entity created successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't create entity: " + ex.Message);
            }
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody] UserUpdateDTO dto)
        {
            //validation
            var validationResult = _updateValidator.Validate(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            try
            {
                var result = _service.UpdateById(dto);
                if (!result.IsSuccess)
                    return BadRequest(result.Message);

                return !result.Data ?
                     NotFound($"Entity with ID {dto.Id} not found") :
                     Ok("Entity updated successfully");

            }
            catch (Exception ex)
            {
                return BadRequest("Can't update entity: " + ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            //validate ID
            if (id <= 0)
                return BadRequest($"The entity id {id} can't be zero and should be positive ");

            try
            {
                var result = _service.Delete(id);
                if (!result.IsSuccess)
                    return BadRequest(result.Message);

                return !result.Data ?
                     NotFound($"Entity with ID {id} not found") :
                     Ok("Entity deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Can't delete entity: " + ex.Message);
            }
        }
    }
}