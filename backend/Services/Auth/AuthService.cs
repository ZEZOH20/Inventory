using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Inventory.DTO.AuthDtos.Requests;
using Inventory.DTO.AuthDtos.Responses;
using Inventory.Models;
using Inventory.Shares;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Response<AuthDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Response<AuthDto>.Failure("Invalid email or password", HttpStatusCode.Unauthorized);

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, userRoles.FirstOrDefault() ?? "Employee");

            var authDto = new AuthDto
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                Role = userRoles.FirstOrDefault() ?? "Employee",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresOn = token.ValidTo
            };

            return Response<AuthDto>.Success(authDto, "Login successful");
        }

        public async Task<Response<AuthDto>> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return Response<AuthDto>.Failure("Email is already registered", HttpStatusCode.BadRequest);

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Name = dto.UserName // Assuming Name is UserName for now
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return Response<AuthDto>.Failure($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}", HttpStatusCode.InternalServerError);

            await _userManager.AddToRoleAsync(user, dto.Role);

            var token = _tokenService.GenerateToken(user, dto.Role);

            var authDto = new AuthDto
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                Role = dto.Role,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresOn = token.ValidTo
            };

            return Response<AuthDto>.Success(authDto, "Registration successful");
        }
    }

    public interface IAuthService
    {
        Task<Response<AuthDto>> LoginAsync(LoginDto dto);
        Task<Response<AuthDto>> RegisterAsync(RegisterDto dto);
    }
}