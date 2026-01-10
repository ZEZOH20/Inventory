# Authentication and Profile Management Documentation

This document contains the code from the attached files related to Authentication and Profile Management in the InventoryV2 project. Each file's code is presented unchanged, followed by a detailed line-by-line explanation.

## CurrentUserService.cs

### Code

```csharp
using System.Security.Claims;
using InventoryV2.Interfaces.IServices;

namespace InventoryV2.Services.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        readonly IHttpContextAccessor _context;
        public CurrentUserService(IHttpContextAccessor context)
        {
            _context = context;
        }
        public string? UserId
            => _context.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? UserRole
         => _context.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

        public string? UserIp
        => _context.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}
```

### Line-by-Line Explanation

1. `using System.Security.Claims;`: Imports the System.Security.Claims namespace, which provides classes for handling claims-based identity, such as ClaimTypes used in the properties.
2. `using InventoryV2.Interfaces.IServices;`: Imports the custom interface ICurrentUserService from the project's interfaces.
3. `namespace InventoryV2.Services.Auth`: Defines the namespace for the Auth services in the InventoryV2 project.
4. `public class CurrentUserService : ICurrentUserService`: Declares the CurrentUserService class that implements the ICurrentUserService interface.
5. `readonly IHttpContextAccessor _context;`: Declares a readonly field to access the HTTP context, injected via dependency injection.
6. `public CurrentUserService(IHttpContextAccessor context)`: Constructor that takes IHttpContextAccessor as a parameter and assigns it to the field.
7. `{ _context = context; }`: Assigns the injected context to the private field.
8. `public string? UserId => _context.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);`: A property that retrieves the user's ID from the claims in the current HTTP context using the NameIdentifier claim type.
9. `public string? UserRole => _context.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);`: A property that retrieves the user's role from the claims.
10. `public string? UserIp => _context.HttpContext?.Connection?.RemoteIpAddress?.ToString();`: A property that gets the user's IP address from the connection.

## AuthService.cs

### Code

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using AutoMapper;
using InventoryV2.Dtos.AuthDtos.Requests;
using InventoryV2.Dtos.AuthDtos.Responses;
using InventoryV2.Interfaces.IServices;
using InventoryV2.Models;
using InventoryV2.Shares;
using Microsoft.AspNetCore.Identity;

namespace InventoryV2.Services.Auth
{
    public class AuthService : IAuthService
    {
        /*
          UserManager<User> -> create, find, update users.
          RoleManager<IdentityRole> -> create, find, update roles.
          SignInManager<User> -> login , logout.
       */
        public UserManager<ApplicationUser> _manager;
        ITokenService _tokenService;
        ISendEmailService _sendEmailService;
        IOtpService _otpService;
        readonly IMapper _mapper;


        public AuthService(
            UserManager<ApplicationUser> manager,
            ITokenService tokenService,
            IOtpService otpService,
            ISendEmailService sendEmailService,
            IMapper mapper
            )
        {
            _manager = manager;
            _tokenService = tokenService;
            _mapper = mapper;
            _sendEmailService = sendEmailService;
            _otpService = otpService;
        }
        public async Task<Response<AuthDto>> LoginAsync(LoginDto dto)
        {
            var user = await _manager.FindByEmailAsync(dto.Email);

            if (user is null || !await _manager.CheckPasswordAsync(user, dto.Password))
                return Response<AuthDto>.Failure("Invalid email or password", HttpStatusCode.Unauthorized);

            var userRoles = await _manager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, userRoles[0]);

            AuthDto response = new AuthDto
            {
                Id = user.Id,
                Email = dto.Email,
                UserName = user.UserName,
                Role = userRoles[0],
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresOn = token.ValidTo
            };

            return Response<AuthDto>.Success(response, "Successfully");
        }

        public async Task<Response<AuthDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken)
        {

            //check if email id not exists
            var SearchedUser = await _manager.FindByEmailAsync(dto.Email);
            if (SearchedUser is not null)
                return Response<AuthDto>.Failure("Email is already registered", HttpStatusCode.Unauthorized);

            //if (SearchedUser is not null && SearchedUser.UserName == dto.UserName)
            //    return Response<AuthDto>.Failure("UserName already exists", HttpStatusCode.Unauthorized);

            //check Otp is Correct
            bool IsVerified = await IsVerifiedEmail(dto.UserKey, dto.Otp, cancellationToken);
            if (!IsVerified)
                return Response<AuthDto>.Failure("Your email is not verified. Please verify it again");

            //register user email
            ApplicationUser user = _mapper.Map<ApplicationUser>(dto);
            var result = await _manager.CreateAsync(user, dto.Password);


            if (!result.Succeeded)
               return Response<AuthDto>.Failure($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}"
              ,HttpStatusCode.InternalServerError);

            // validation : Roles.All.Contains(dto.Role)
            //Add role to user
            await _manager.AddToRoleAsync(user, dto.Role);

            //generate stateless token
            var token = _tokenService.GenerateToken(user, dto.Role);

            AuthDto response = new AuthDto
            {
                Id = user.Id,
                Email = dto.Email,
                UserName = dto.UserName,
                Role = dto.Role,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresOn = token.ValidTo
            };
            return Response<AuthDto>.Success(response, "Successfully");

        }

        public async Task<Response> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            var user = await _manager.FindByEmailAsync(dto.Email);
            if (user is null)
                return Response.Failure("Email doesn't exists");

            //check Otp is Correct
            bool IsVerified = await IsVerifiedEmail(dto.UserKey, dto.Otp, cancellationToken);
            if (!IsVerified)
                return Response.Failure("Your email is not verified. Please verify it again");

            //reset password
            var passwordResetToken = await _manager.GeneratePasswordResetTokenAsync(user);
            var result = await _manager.ResetPasswordAsync(user, passwordResetToken, dto.NewPassword);

            return Response.Success("Password reset Succesfully");

        }
        public async Task<Response<SendVerificationEmailRsDto>> SendVerificationEmailAsync(SendVerificationEmailRqDto dto, CancellationToken cancellationToken)
        {
            //check if email id not exists
            //if (await _manager.FindByEmailAsync(dto.Email) is not null)
            //    return Response<SendVerificationEmailRsDto>.Failure("Email is already registered");

            var otpGenerResult = await _otpService.GenerateAndStoreOtpAsync(dto.Email, cancellationToken);
            var isSended = await _sendEmailService.SendVerificationEmail(dto.Email, otpGenerResult.Otp);

            if (!isSended)
                Response<SendVerificationEmailRsDto>.Failure("Email is not valid or Network error try again");


            SendVerificationEmailRsDto response = new SendVerificationEmailRsDto
            {
                Otp = otpGenerResult.Otp,
                UserKey = otpGenerResult.UserKey,
                AvailableUntil = otpGenerResult.AvailableUntil
            };

            return Response<SendVerificationEmailRsDto>.Success(response, "Email Successfully Sended", HttpStatusCode.OK);
        }


        public async Task<bool> IsVerifiedEmail(string userKey, string otp, CancellationToken cancellationToken)
              => await _otpService.ValidateOtpAsync(userKey, otp, cancellationToken);
    }
}
```

### Line-by-Line Explanation

1. `using System.IdentityModel.Tokens.Jwt;`: Imports for JWT token handling.
2. `using System.Net;`: For HTTP status codes.
3. `using AutoMapper;`: For object mapping.
4. `using InventoryV2.Dtos.AuthDtos.Requests;`: Custom DTOs for requests.
5. `using InventoryV2.Dtos.AuthDtos.Responses;`: Custom DTOs for responses.
6. `using InventoryV2.Interfaces.IServices;`: Interfaces for services.
7. `using InventoryV2.Models;`: Project models.
8. `using InventoryV2.Shares;`: Shared utilities.
9. `using Microsoft.AspNetCore.Identity;`: ASP.NET Identity for user management.
10. `namespace InventoryV2.Services.Auth`: Namespace declaration.
11. `public class AuthService : IAuthService`: Class implementing IAuthService.
12. `/* Comments */`: Explains the roles of UserManager, etc.
13. Fields: Dependencies injected.
14. Constructor: Initializes dependencies.
15. `public async Task<Response<AuthDto>> LoginAsync(LoginDto dto)`: Login method.
16. Finds user by email.
17. Checks if user exists and password is correct.
18. Gets user roles.
19. Generates token.
20. Creates response DTO.
21. Returns success response.
22. `public async Task<Response<AuthDto>> RegisterAsync(...)`: Register method.
23. Checks if email already exists.
24. Verifies OTP.
25. Maps DTO to user.
26. Creates user.
27. Adds role.
28. Generates token.
29. Returns response.
30. `public async Task<Response> ResetPasswordAsync(...)`: Reset password method.
31. Finds user.
32. Verifies OTP.
33. Resets password.
34. Returns success.
35. `public async Task<Response<SendVerificationEmailRsDto>> SendVerificationEmailAsync(...)`: Sends verification email.
36. Generates OTP.
37. Sends email.
38. Returns response.
39. `public async Task<bool> IsVerifiedEmail(...)`: Validates OTP.

## OtpService.cs

### Code

```csharp
using System.Text.Json;
using InventoryV2.Dtos.AuthDtos.Responses;
using InventoryV2.Interfaces.IServices;
using InventoryV2.Shares;
using Microsoft.Extensions.Caching.Distributed;

namespace InventoryV2.Services.Auth
{
    public class OtpService : IOtpService
    {
        readonly IDistributedCache _cache;
        readonly IConfiguration _config;
        public OtpService(
            IDistributedCache cache,
            IConfiguration config,
            ISendEmailService sendEmailService
            ) {
            _cache = cache;
            _config = config;
        }
        public async Task<SendVerificationEmailRsDto> GenerateAndStoreOtpAsync(string userEmail, CancellationToken cancellationToken)
        {
            string userKey = Hashing.Generate(userEmail);

            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            string hashedOtp = Hashing.Generate(otp);

            TimeSpan expiry = TimeSpan.FromMinutes(long.Parse(_config["Otp:expiry"])); // OTP valid for 5 minutes
            DateTimeOffset availableUntil = DateTimeOffset.UtcNow + expiry;
            var options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(availableUntil);

            await _cache.SetStringAsync(userKey, JsonSerializer.Serialize(hashedOtp),options, cancellationToken );
            // should return otp and userKey

            return new SendVerificationEmailRsDto{
                Otp = otp,
                UserKey = userKey,
                AvailableUntil = availableUntil,
            }; 
        }

        public async Task<bool> ValidateOtpAsync(string userKey, string otp, CancellationToken cancellationToken)
        {
            string cachedOtp = await _cache.GetStringAsync(userKey, cancellationToken); 
            if (cachedOtp is null) return false;

            string hashedOtp = Hashing.Generate(otp);
            cachedOtp = JsonSerializer.Deserialize<string>(cachedOtp);

            if (cachedOtp != hashedOtp) return false;
            return true;
            
        }
    }
}
```

### Line-by-Line Explanation

1. `using System.Text.Json;`: For JSON serialization.
2. `using InventoryV2.Dtos.AuthDtos.Responses;`: Response DTOs.
3. `using InventoryV2.Interfaces.IServices;`: Interfaces.
4. `using InventoryV2.Shares;`: Hashing utility.
5. `using Microsoft.Extensions.Caching.Distributed;`: For distributed cache.
6. `namespace InventoryV2.Services.Auth`: Namespace.
7. `public class OtpService : IOtpService`: Implements IOtpService.
8. Fields: Cache and config.
9. Constructor: Initializes dependencies.
10. `public async Task<SendVerificationEmailRsDto> GenerateAndStoreOtpAsync(...)`: Generates and stores OTP.
11. Creates user key by hashing email.
12. Generates random OTP.
13. Hashes OTP.
14. Sets expiry.
15. Stores in cache.
16. Returns DTO with OTP details.
17. `public async Task<bool> ValidateOtpAsync(...)`: Validates OTP.
18. Retrieves cached OTP.
19. Hashes input OTP.
20. Compares hashes.
21. Returns true if match.

## SendEmailService.cs

### Code

```csharp
using FluentEmail.Core;
using InventoryV2.Interfaces.IServices;

namespace InventoryV2.Services.Auth
{
    public class SendEmailService : ISendEmailService
    {
        readonly IConfiguration _config;
        readonly IFluentEmail _email;
        public SendEmailService(IConfiguration config , IFluentEmail email) { 
            _config = config;
            _email = email;
        }
        public async Task<bool> SendVerificationEmail(string recipientEmail, string code)
        {
                //.From(_config["Email:SenderEmail"])

            var email = await _email
                .To(recipientEmail)
                .Subject(_config["Email:Subject"])
                .Body(code)
                .SendAsync();

            return email.Successful;
        }
    }
}
```

### Line-by-Line Explanation

1. `using FluentEmail.Core;`: For email sending.
2. `using InventoryV2.Interfaces.IServices;`: Interfaces.
3. `namespace InventoryV2.Services.Auth`: Namespace.
4. `public class SendEmailService : ISendEmailService`: Implements ISendEmailService.
5. Fields: Config and email service.
6. Constructor: Initializes.
7. `public async Task<bool> SendVerificationEmail(...)`: Sends email.
8. Builds email with recipient, subject, body.
9. Sends asynchronously.
10. Returns success status.

## TokenService.cs

### Code

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryV2.Interfaces.IServices;
using InventoryV2.Models;
using InventoryV2.Shares;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InventoryV2.Services.Auth
{
    public class TokenService: ITokenService
    {

        readonly JwtSettings _Jwt;
        public TokenService(IOptions<JwtSettings> Jwt) {
            _Jwt = Jwt.Value;
        }


        /*
         Token 3 Parts :
         header : hash algo , type of library token
         payload  : claims, expiredate
         signture : hash(hash(header) + hash(payload) + hash(secret Key))

         iat : issued Time (creation Time)
         jti : Jwt ID 
         Sub : user ID
         issuer : who create the token (your server url)
         audience : who will capable to use it (client url)
         SigningCredentials ensure key is compatible with creditionl

         
         */
        public JwtSecurityToken GenerateToken(ApplicationUser user , string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                          DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("roles",role)   
            };

            var token = new JwtSecurityToken(
                issuer: _Jwt.Issuer,
                audience: _Jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_Jwt.DurationInHours),
                signingCredentials: creds);

            return token;
        }

        //public bool ValidateToken()
        //{
        //    //check if token exists in request header  

        //}
    }
}
```

### Line-by-Line Explanation

1. `using System.IdentityModel.Tokens.Jwt;`: JWT handling.
2. `using System.Security.Claims;`: Claims.
3. `using System.Text;`: Encoding.
4. `using InventoryV2.Interfaces.IServices;`: Interfaces.
5. `using InventoryV2.Models;`: Models.
6. `using InventoryV2.Shares;`: Shares.
7. `using Microsoft.Extensions.Options;`: Options pattern.
8. `using Microsoft.IdentityModel.Tokens;`: Token security.
9. `namespace InventoryV2.Services.Auth`: Namespace.
10. `public class TokenService: ITokenService`: Implements ITokenService.
11. Field: JwtSettings.
12. Constructor: Initializes settings.
13. Comments: Explains JWT structure.
14. `public JwtSecurityToken GenerateToken(...)`: Generates JWT.
15. Creates symmetric key.
16. Signing credentials.
17. Claims list.
18. Creates token with issuer, audience, claims, expiry, creds.
19. Returns token.
20. Commented ValidateToken method.

## AuditableEntityService.cs

### Code

```csharp
using System;
using InventoryV2.Data.DbContexts;
using InventoryV2.Interfaces.IServices;
using InventoryV2.Migrations;
using InventoryV2.Models;

namespace InventoryV2.Services;

public class AuditableEntityService
{
      readonly SqlDbContext _context;
      readonly ICurrentUserService _currentUserService;
        public AuditableEntityService(SqlDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task SoftDeleteAsync<T>(T entity,CancellationToken cancellationToken = default) where T : AuditableEntity
        {
            entity.SoftDelete(_currentUserService.UserId, _currentUserService.UserIp);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RestoreAsync<T>(T entity,CancellationToken cancellationToken = default) where T : AuditableEntity
        {
            entity.Restore();
            await _context.SaveChangesAsync(cancellationToken);
        }

}
```

### Line-by-Line Explanation

1. `using System;`: System namespace.
2. `using InventoryV2.Data.DbContexts;`: DbContext.
3. `using InventoryV2.Interfaces.IServices;`: Interfaces.
4. `using InventoryV2.Migrations;`: Migrations (possibly unused).
5. `using InventoryV2.Models;`: Models.
6. `namespace InventoryV2.Services;`: Namespace.
7. `public class AuditableEntityService`: Service for auditable entities.
8. Fields: Context and current user service.
9. Constructor: Initializes.
10. `public async Task SoftDeleteAsync<T>(...)`: Soft deletes entity.
11. Calls entity's SoftDelete method with user ID and IP.
12. Saves changes.
13. `public async Task RestoreAsync<T>(...)`: Restores entity.
14. Calls Restore.
15. Saves changes.

## ImageService.cs

### Code

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryV2.Interfaces.IServices;

namespace InventoryV2.Services
{
    public class ImageService : IImageService
    {
        readonly IWebHostEnvironment _environment;
        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public Task DeleteImageAsync(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return Task.CompletedTask;

            // Convert web path (with /) to OS-specific file system path
            var normalizedPath = imagePath.Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_environment.WebRootPath, normalizedPath);

            // Delete the file if it exists
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }

        public Task<(Stream? Stream, string? ContentType)?> GetImageAsync(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return Task.FromResult<(Stream?, string?)?>(null);

            // Convert web path (with /) to OS-specific file system path
            var normalizedPath = imagePath.Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_environment.WebRootPath, normalizedPath);
            if (!File.Exists(fullPath))
                return Task.FromResult<(Stream?, string?)?>(null);

            // Open the file as a read-only stream
            var stream = File.OpenRead(fullPath);
            var extension = Path.GetExtension(fullPath).ToLower();

            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",  // Standard MIME for JPEG
                ".png" => "image/png",             // Standard MIME for PNG
                ".gif" => "image/gif",             // Standard MIME for GIF
                _ => "application/octet-stream"     // Default for unknown
            };

            // Return the stream and content type as a tuple
            return Task.FromResult<(Stream?, string?)?>((stream, contentType));
        }

        public async Task<string?> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Only image files (JPG, PNG, GIF) are allowed.");

            const long maxFileSize = 5 * 1024 * 1024;  // 5MB in bytes
            if (file.Length > maxFileSize)
                throw new InvalidOperationException("Image size must be less than 5MB.");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);  // Make folder if needed

            var datePart = DateTime.UtcNow.ToString("yyyy-MM-dd-HHmmss");
            var uniqueFileName = $"{Guid.NewGuid()}-{datePart}{extension}";

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream); // Copy uploaded file to disk
                }
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("Failed to save image file.", ex);
            }
            // Return relative path for DB (always use forward slashes for web URLs)
            return $"images/{uniqueFileName}";
        }
    }
}
```

### Line-by-Line Explanation

1. `using System;`: System.
2. `using System.Collections.Generic;`: Collections.
3. `using System.Linq;`: LINQ.
4. `using System.Threading.Tasks;`: Tasks.
5. `using InventoryV2.Interfaces.IServices;`: Interfaces.
6. `namespace InventoryV2.Services`: Namespace.
7. `public class ImageService : IImageService`: Implements IImageService.
8. Field: Web host environment.
9. Constructor: Initializes.
10. `public Task DeleteImageAsync(...)`: Deletes image.
11. Checks if path is null/empty.
12. Normalizes path.
13. Deletes file if exists.
14. `public Task<(Stream?, string?)?> GetImageAsync(...)`: Gets image stream.
15. Checks path.
16. Normalizes path.
17. Opens file stream.
18. Determines content type.
19. Returns tuple.
20. `public async Task<string?> UploadImageAsync(...)`: Uploads image.
21. Validates file.
22. Checks extension.
23. Checks size.
24. Creates folder.
25. Generates unique name.
26. Saves file.
27. Returns relative path.

## AccountController.cs

### Code

```csharp
using FluentValidation;
using InventoryV2.Dtos.AuthDtos.Requests;
using InventoryV2.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;


namespace InventoryV2.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("SlidingPolicy")]
    public class AccountController : ControllerBase
    {
      
        public IAuthService _authService;
        readonly IServiceProvider _serviceProvider;
        public AccountController(
            IAuthService authService,
            IServiceProvider serviceProvider
            )
        {
            _authService = authService; 
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Authenticates a user with email and password.
        /// </summary>
        /// <param name="dto">The login request containing email and password.</param>
        /// <returns>A JWT token if login is successful.</returns>
        /// 

        //[Authorize(Roles = SystemRoles.Manager)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<LoginDto>>();
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var response = await _authService.LoginAsync(dto);

                return StatusCode((int)response.StatusCode, new
                {
                    IsAuthenticated = response.IsSuccess,
                    message = response.Message,
                    statusCode = response.StatusCode,
                    data = response.Data
                });

        }

       
        [HttpPost("register")] 
        public async Task<IActionResult> Register([FromBody] RegisterDto dto , CancellationToken cancellationToken)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<RegisterDto>>();
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);


            var response = await _authService.RegisterAsync(dto , cancellationToken);

      
                return StatusCode((int) response.StatusCode, new
                {
                    IsAuthenticated = response.IsSuccess,
                    message = response.Message,
                    statusCode = response.StatusCode,
                    data = response.Data
                });

        }
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto, CancellationToken cancellationToken)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<ResetPasswordDto>>();
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var response = await _authService.ResetPasswordAsync(dto , cancellationToken);

    
            return StatusCode((int)response.StatusCode, new
               {
                    IsAuthenticated = response.IsSuccess,
                    message = response.Message,
                    statusCode = response.StatusCode,
               });

        }

        [HttpPost("verifyEmail")]
        public async Task<IActionResult> SendVerificationEmail([FromBody] SendVerificationEmailRqDto dto , CancellationToken cancellationToken)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<SendVerificationEmailRqDto>>();
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var response = await _authService.SendVerificationEmailAsync(dto, cancellationToken);

         
            return StatusCode((int)response.StatusCode, new
              {
                 message = response.Message,
                 statusCode = response.StatusCode,
                 data = response.Data
            });
        }


    }
}
```

### Line-by-Line Explanation

1. `using FluentValidation;`: Validation.
2. `using InventoryV2.Dtos.AuthDtos.Requests;`: DTOs.
3. `using InventoryV2.Interfaces.IServices;`: Interfaces.
4. `using Microsoft.AspNetCore.Mvc;`: MVC.
5. `using Microsoft.AspNetCore.RateLimiting;`: Rate limiting.
6. `namespace InventoryV2.Controllers`: Namespace.
7. `[Route("api/[controller]")]`: Route attribute.
8. `[ApiController]`: API controller.
9. `[EnableRateLimiting("SlidingPolicy")]`: Rate limiting.
10. `public class AccountController : ControllerBase`: Controller class.
11. Fields: Auth service and service provider.
12. Constructor: Initializes.
13. `[HttpPost("login")]`: Login endpoint.
14. Validates DTO.
15. Calls service.
16. Returns response.
17. `[HttpPost("register")]`: Register endpoint.
18. Similar to login.
19. `[HttpPost("resetPassword")]`: Reset password.
20. `[HttpPost("verifyEmail")]`: Send verification email.

## ProfileController.cs

### Code

```csharp
using AutoMapper;
using FluentValidation;
using InventoryV2.Dtos.ProfileDto.Requests;
using InventoryV2.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryV2.Controllers
{
    [Authorize] 
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController:ControllerBase
    {
        readonly IProfileService _profileS;
        readonly IServiceProvider _serviceProvider;
        public ProfileController(IProfileService profileS , IServiceProvider serviceProvider)
        {
            _profileS = profileS;
            _serviceProvider = serviceProvider;
        }
        [HttpGet("get")]
        public async Task<IActionResult> GetProfile()
        {
            var response = await _profileS.Get();
            return StatusCode((int)response.StatusCode, new
            {
                message = response.Message,
                statusCode = response.StatusCode,
                data = response.Data
            });

        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UpdateUserProfileDto>>();
            var validationResult = await validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);
            
            var response = await _profileS.Update(dto);
            return StatusCode((int)response.StatusCode, response.Message);
        }
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteProfile([FromRoute] string userId)
        {
            var response = await _profileS.Delete(userId);
            return StatusCode((int)response.StatusCode, response.Message);
        }
       
    }
}
```

### Line-by-Line Explanation

1. `using AutoMapper;`: Mapping.
2. `using FluentValidation;`: Validation.
3. `using InventoryV2.Dtos.ProfileDto.Requests;`: Profile DTOs.
4. `using InventoryV2.Interfaces.IServices;`: Interfaces.
5. `using Microsoft.AspNetCore.Authorization;`: Authorization.
6. `using Microsoft.AspNetCore.Mvc;`: MVC.
7. `namespace InventoryV2.Controllers`: Namespace.
8. `[Authorize]`: Requires authorization.
9. `[ApiController]`: API controller.
10. `[Route("api/[controller]")]`: Route.
11. `public class ProfileController:ControllerBase`: Controller.
12. Fields: Profile service and provider.
13. Constructor: Initializes.
14. `[HttpGet("get")]`: Get profile.
15. Calls service.
16. Returns response.
17. `[HttpPut("update")]`: Update profile.
18. Validates.
19. Calls service.
20. `[HttpDelete("delete/{userId}")]`: Delete profile.

## JwtSettings.cs

### Code

```csharp
namespace InventoryV2.Shares
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInHours { get; set; }
    }
}
```

### Line-by-Line Explanation

1. `namespace InventoryV2.Shares`: Namespace.
2. `public class JwtSettings`: Settings class.
3. `public string Key { get; set; }`: JWT secret key.
4. `public string Issuer { get; set; }`: Token issuer.
5. `public string Audience { get; set; }`: Token audience.
6. `public double DurationInHours { get; set; }`: Token duration.