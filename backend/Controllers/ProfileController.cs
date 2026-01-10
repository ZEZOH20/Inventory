using Inventory.DTO.AuthDtos.Requests;
using Inventory.DTO.UserDto.Requests;
using Inventory.DTO.UserDto.Responses;
using Inventory.Models;
using Inventory.Services.Auth;
using Inventory.Services.CurrentUser;
using Inventory.Shares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Inventory.Services;

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUser _currentUserService;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(UserManager<ApplicationUser> userManager, ICurrentUser currentUserService, IImageService imageService, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _imageService = imageService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (user == null)
                return NotFound("User not found");

            var response = new UserResponseDTO(user);

            return Ok(Response<UserResponseDTO>.Success(response, "Profile retrieved successfully"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromForm] UserUpdateDTO dto)
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (user == null)
                return NotFound("User not found");

            // Update allowed fields
            if (!string.IsNullOrEmpty(dto.Name))
                user.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Phone))
                user.PhoneNumber = dto.Phone;
            if (!string.IsNullOrEmpty(dto.Mail))
                user.Email = dto.Mail;
            // Fax and Domain are not in ApplicationUser, so ignore

            // Handle profile image
            if (dto.ProfileImage != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    await _imageService.DeleteImageAsync(user.ProfileImage);
                }

                // Upload new image
                var imagePath = await _imageService.UploadImageAsync(dto.ProfileImage);
                if (imagePath != null)
                {
                    user.ProfileImage = imagePath;
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest($"Update failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return Ok(Inventory.Shares.Response.Success("Profile updated successfully"));
        }

        [HttpGet("image")]
        public async Task<IActionResult> GetProfileImage()
        {
            var user = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (user == null || string.IsNullOrEmpty(user.ProfileImage))
                return NotFound("Profile image not found");

            var path = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, user.ProfileImage);
            if (!System.IO.File.Exists(path))
                return NotFound("Profile image not found");

            var extension = System.IO.Path.GetExtension(path).ToLowerInvariant();
            string mimeType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return PhysicalFile(path, mimeType);
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