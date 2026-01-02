# Development Guide for Inventory Management System

This guide documents the architectural patterns, best practices, and critical code implementations extracted from the InventoryV2 codebase. It serves as a reference for developers building similar systems using .NET Core, Entity Framework, and related technologies.

## Table of Contents
1. [Architectural Patterns & Best Practices](#architectural-patterns--best-practices)
2. [Key Design Patterns](#key-design-patterns)
3. [Code Examples](#code-examples)
4. [Best Practices](#best-practices)

## Architectural Patterns & Best Practices

### 1. Service Layer Pattern
All business logic must reside in the Service layer. Controllers should only handle HTTP requests/responses and delegate logic to services. This ensures separation of concerns and testability.

**Why?** Keeps controllers thin, centralizes business rules, and allows for easy unit testing of logic.

### 2. Interface Implementation
Always implement interfaces for services, repositories, and other injectable components. This enables dependency injection and facilitates mocking for testing.

**Why?** Promotes loose coupling, supports the Dependency Inversion Principle, and makes code more maintainable.

### 3. Fluent Validation
Use Fluent Validation for all input validation. Validators are separate classes that define rules for DTOs.

**Why?** Provides clean, readable validation rules that are easily testable and maintainable compared to data annotations.

### 4. Repository and Unit of Work Patterns
Implement the Repository pattern for data access and Unit of Work for transaction management. Use generic repositories for common CRUD operations and specific repositories only when needed.

**Why?** Abstracts data access logic, enables unit testing with mock repositories, and manages database transactions efficiently.

## Key Design Patterns

### Dependency Injection
The system uses constructor injection throughout. Services, repositories, and other dependencies are injected via interfaces.

### Soft Delete with Audit Trail
Entities inherit from `AuditableEntity` which provides soft delete functionality along with full audit tracking (created/updated/deleted by, timestamps, IP addresses).

### Response Wrapper Pattern
All service methods return a `Response` or `Response<T>` object that encapsulates success/failure status, messages, and data.

### JWT Authentication
Uses ASP.NET Core Identity with JWT tokens for authentication. Includes role-based authorization.

### AutoMapper for Object Mapping
Uses AutoMapper to map between domain models and DTOs, reducing boilerplate code.

## Code Examples

### Pagination, Filtering, and Searching Extension

```csharp
using Microsoft.EntityFrameworkCore;

namespace InventoryV2.Shares;

public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public static class PaginationExtension
{
    public static async Task<PagedResult<T>> ToPagedListAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var count = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        bool hasPreviousPage = pageNumber > 1;
        return new PagedResult<T>()
        {
            Data = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = count,
        };
    }
}
```

**Usage:** Apply to any `IQueryable<T>` to get paginated results with metadata.

### Generic Repository Pattern

```csharp
using System.Linq.Expressions;
using InventoryV2.Data.DbContexts;
using InventoryV2.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryV2.Repositeries
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly SqlDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(SqlDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetQuery() => _dbSet.AsQueryable();

        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) 
            => await _dbSet.FirstOrDefaultAsync(predicate);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities) 
            => await _dbSet.AddRangeAsync(entities);

        public void Update(T entity) => _context.Update(entity);
        
        public void Delete(T entity) => _dbSet.Remove(entity);

        public void DeleteRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);
    }
}
```

**Interface:**

```csharp
using System.Linq.Expressions;

namespace InventoryV2.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQuery();
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
```

### Unit of Work Pattern

```csharp
using InventoryV2.Data.DbContexts;
using InventoryV2.Interfaces;
using InventoryV2.Models;

namespace InventoryV2.Repositeries;

public class UnitOfWork : IUnitOfWork
{
    private readonly SqlDbContext _context;

    public UnitOfWork(SqlDbContext context)
    {
        _context = context;
            
        Suppliers = new GenericRepository<Supplier>(_context);
        Customers = new GenericRepository<Customer>(_context);
        Warehouses = new GenericRepository<Warehouse>(_context);
        Products = new GenericRepository<Product>(_context);
        WarehouseProducts = new GenericRepository<Warehouse_Product>(_context);
        SupplyOrders = new GenericRepository<Supply_Order>(_context);
        ReleaseOrders = new GenericRepository<Release_Order>(_context);
        TransferOrders = new GenericRepository<Transfer_Order>(_context);
        SOProducts = new GenericRepository<SO_Product>(_context);
        ROProducts = new GenericRepository<RO_Product>(_context);
        TOProducts = new GenericRepository<TO_Product>(_context);
    }

    public IGenericRepository<Supplier> Suppliers { get; }
    public IGenericRepository<Customer> Customers { get; }
    public IGenericRepository<Warehouse> Warehouses { get; }
    public IGenericRepository<Product> Products { get; }
    public IGenericRepository<Warehouse_Product> WarehouseProducts { get; }
    public IGenericRepository<Supply_Order> SupplyOrders { get; }
    public IGenericRepository<Release_Order> ReleaseOrders { get; }
    public IGenericRepository<Transfer_Order> TransferOrders { get; }
    public IGenericRepository<SO_Product> SOProducts { get; }
    public IGenericRepository<RO_Product> ROProducts { get; }
    public IGenericRepository<TO_Product> TOProducts { get; }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
```

**Interface:**

```csharp
using InventoryV2.Models;

namespace InventoryV2.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<Supplier> Suppliers { get; }
    IGenericRepository<Customer> Customers { get; }
    IGenericRepository<Warehouse> Warehouses { get; }
    IGenericRepository<Product> Products { get; }
    IGenericRepository<Warehouse_Product> WarehouseProducts { get; }
    IGenericRepository<Supply_Order> SupplyOrders { get; }
    IGenericRepository<Release_Order> ReleaseOrders { get; }
    IGenericRepository<Transfer_Order> TransferOrders { get; }
    IGenericRepository<SO_Product> SOProducts { get; }
    IGenericRepository<RO_Product> ROProducts { get; }
    IGenericRepository<TO_Product> TOProducts { get; }

    Task<int> SaveChangesAsync();
}
```

### Authentication Service

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
            var SearchedUser = await _manager.FindByEmailAsync(dto.Email);
            if (SearchedUser is not null)
                return Response<AuthDto>.Failure("Email is already registered", HttpStatusCode.Unauthorized);

            bool IsVerified = await IsVerifiedEmail(dto.UserKey, dto.Otp, cancellationToken);
            if (!IsVerified)
                return Response<AuthDto>.Failure("Your email is not verified. Please verify it again");

            ApplicationUser user = _mapper.Map<ApplicationUser>(dto);
            var result = await _manager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
               return Response<AuthDto>.Failure($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}"
              ,HttpStatusCode.InternalServerError);

            await _manager.AddToRoleAsync(user, dto.Role);

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

            bool IsVerified = await IsVerifiedEmail(dto.UserKey, dto.Otp, cancellationToken);
            if (!IsVerified)
                return Response.Failure("Your email is not verified. Please verify it again");

            var passwordResetToken = await _manager.GeneratePasswordResetTokenAsync(user);
            var result = await _manager.ResetPasswordAsync(user, passwordResetToken, dto.NewPassword);

            return Response.Success("Password reset Succesfully");
        }

        public async Task<Response<SendVerificationEmailRsDto>> SendVerificationEmailAsync(SendVerificationEmailRqDto dto, CancellationToken cancellationToken)
        {
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

### Response Wrapper Pattern

```csharp
using System.Net;

namespace InventoryV2.Shares
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; }
        public HttpStatusCode StatusCode { get; }

        public Response(bool isSuccess, string message, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            IsSuccess = isSuccess;
            Message = message;
            StatusCode = statusCode;
        }

        public static Response Success(string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
            => new Response(true, message, statusCode);
        public static Response Failure(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            => new Response(false, message, statusCode);
    }

    public class Response<T>:Response
    {
        public T Data { get; }
        public Response(bool isSuccess, string message, HttpStatusCode statusCode = HttpStatusCode.OK, T data = default!)
            :base(isSuccess, message , statusCode)
         {
            Data = data;
         }
        public static Response<T> Success(T data, string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
            => new Response<T>(true, message, statusCode, data);
        public static new Response<T> Failure(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
      => new Response<T>(false, message, statusCode, default!);
    }
}
```

### Auditable Entity Base Class

```csharp
using System.ComponentModel.DataAnnotations;
using InventoryV2.Shares;

namespace InventoryV2.Models;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    public string? CreatedIP { get; private set; }
    public string? UpdatedIP { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }
    [Timestamp]
    public byte[] RowVersion { get; private set; } = null!;

    public void SetCreated(string userId, string? ip = null)
    {
        CreatedBy = userId;
        CreatedIP = ip?.Truncate(45);
        CreatedAt = DateTime.UtcNow;
    }

    public void SetUpdated(string userId, string? ip = null)
    {
        UpdatedBy = userId;
        UpdatedIP = ip?.Truncate(45);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete(string userId, string? ip = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
        SetUpdated(userId, ip);
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}
```

### Fluent Validation Example

```csharp
using FluentValidation;
using InventoryV2.Dtos.AuthDtos.Requests;
using InventoryV2.Seeders;

namespace InventoryV2.Dtos.AuthDtos.Validators
{
    public class RegisterDtoValidator:AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(role => SystemRoles.All.Contains(role))
                .WithMessage($"Role must be one of: {string.Join(", ", SystemRoles.All)}");

            RuleFor(x => x.UserKey)
               .NotNull().WithMessage("UserKey is required.");

            RuleFor(x => x.Otp)
               .NotEmpty().WithMessage("OTP is required.")
               .Length(6).WithMessage("OTP must be exactly 6 digits.")
               .Matches(@"^\d{6}$").WithMessage("OTP must contain only numbers.");
        }
    }
}
```

### JWT Token Service

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
    }
}
```

## Best Practices

1. **Always use interfaces** for services and repositories to enable dependency injection and testing.

2. **Keep controllers thin** - they should only handle HTTP concerns and delegate business logic to services.

3. **Use async/await** throughout for I/O operations to avoid blocking threads.

4. **Implement proper error handling** using try-catch blocks and return appropriate HTTP status codes.

5. **Use DTOs** for all API inputs and outputs to decouple the API contract from domain models.

6. **Validate inputs** using Fluent Validation in controllers before passing to services.

7. **Use AutoMapper** for object-to-object mapping to reduce boilerplate code.

8. **Implement audit trails** using the AuditableEntity base class for all entities that require tracking.

9. **Use soft deletes** instead of hard deletes to maintain data integrity and audit trails.

10. **Return consistent response formats** using the Response/Response<T> wrapper classes.

11. **Use pagination** for list endpoints to handle large datasets efficiently.

12. **Implement role-based authorization** using ASP.NET Core Identity and JWT tokens.

13. **Store sensitive configuration** (like JWT keys) in environment variables or secure configuration stores.

14. **Use meaningful commit messages** and follow conventional commit standards.

15. **Write unit tests** for services, repositories, and validation logic.

This guide captures the core patterns and implementations from the InventoryV2 system. Following these patterns will result in maintainable, testable, and scalable .NET applications.