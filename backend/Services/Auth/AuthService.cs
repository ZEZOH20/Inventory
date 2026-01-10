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
        private readonly IOtpService _otpService;
        private readonly ISendEmailService _sendEmailService;

        public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService, IOtpService otpService, ISendEmailService sendEmailService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _otpService = otpService;
            _sendEmailService = sendEmailService;
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

        public async Task<Response<AuthDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return Response<AuthDto>.Failure("Email is already registered", HttpStatusCode.BadRequest);

            // Verify OTP
            var isVerified = await _otpService.ValidateOtpAsync(dto.UserKey, dto.Otp, cancellationToken);
            if (!isVerified)
                return Response<AuthDto>.Failure("Invalid or expired OTP", HttpStatusCode.BadRequest);

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

        public async Task<Response> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Response.Failure("Email does not exist", HttpStatusCode.NotFound);

            // Verify OTP
            var isVerified = await _otpService.ValidateOtpAsync(dto.UserKey, dto.Otp, cancellationToken);
            if (!isVerified)
                return Response.Failure("Invalid or expired OTP", HttpStatusCode.BadRequest);

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword);

            if (!result.Succeeded)
                return Response.Failure($"Password reset failed: {string.Join(", ", result.Errors.Select(e => e.Description))}", HttpStatusCode.InternalServerError);

            return Response.Success("Password reset successful");
        }

        public async Task<Response<SendVerificationEmailRsDto>> SendVerificationEmailAsync(SendVerificationEmailRqDto dto, CancellationToken cancellationToken)
        {
            // Generate and store OTP
            var otpResult = await _otpService.GenerateAndStoreOtpAsync(dto.Email, cancellationToken);

            // Send email
            var emailSent = await _sendEmailService.SendVerificationEmail(dto.Email, otpResult.Otp);
            if (!emailSent)
                return Response<SendVerificationEmailRsDto>.Failure("Failed to send email", HttpStatusCode.InternalServerError);

            return Response<SendVerificationEmailRsDto>.Success(otpResult, "Verification email sent");
        }
    }

    public interface IAuthService
    {
        Task<Response<AuthDto>> LoginAsync(LoginDto dto);
        Task<Response<AuthDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken);
        Task<Response> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken);
        Task<Response<SendVerificationEmailRsDto>> SendVerificationEmailAsync(SendVerificationEmailRqDto dto, CancellationToken cancellationToken);
    }
}