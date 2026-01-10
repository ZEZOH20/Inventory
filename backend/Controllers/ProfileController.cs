using Inventory.DTO.AuthDtos.Requests;
using Inventory.DTO.UserDto.Requests;
using Inventory.DTO.UserDto.Responses;
using Inventory.Models;
using Inventory.Services.Auth;
using Inventory.Services.CurrentUser;
using Inventory.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUser _currentUserService;

        public ProfileController(UserManager<ApplicationUser> userManager, ICurrentUser currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (user == null)
                return NotFound("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            var response = new UserResponseDTO
            {
                Id = user.Id,
                Name = user.UserName ?? "",
                Mail = user.Email ?? "",
                Phone = user.PhoneNumber ?? "",
                Fax = "No Fax",
                Domain = "No Domain"
            };

            return Ok(Response<UserResponseDTO>.Success(response, "Profile retrieved successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDTO dto)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (user == null)
                return NotFound("User not found");

            user.UserName = dto.Name;
            user.Email = dto.Mail;
            user.Name = dto.Name;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest($"Update failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return Ok(Inventory.Shares.Response.Success("Profile updated successfully"));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (user == null)
                return NotFound("User not found");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest($"Password change failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return Ok(Inventory.Shares.Response.Success("Password changed successfully"));
        }
    }
}