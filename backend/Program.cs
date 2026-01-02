using Inventory.Data.DbContexts;
using Inventory.DTO.UserDto.Validations;
using Inventory.Services;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Inventory.DTO.WarehouseDto.Validations;
using Inventory.DTO.SupplyOrderDto.Validations;
using Inventory.DTO.SO_ProductDto.Validators;
using Inventory.DTO.ReleaseOrderDto.Validators;
using backend.DTO.TransferOrderDto.Validations;
using Microsoft.AspNetCore.Identity;
using Inventory.Interfaces;
using Inventory.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Inventory.DTO.AuthDtos.Validators;
using Inventory.Services.Auth;
using Inventory.Models;
using Inventory.Shares;


var builder = WebApplication.CreateBuilder(args);

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:5173")  // Your React app URL
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at 
builder.Services.AddOpenApi();

//connect to Database EF ......
builder.Services.AddDbContext<SqlDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlDbConnection"))
);
//connect to Database EF ......
// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<SqlDbContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Configure JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
// Register Custom Services .....
builder.Services.AddScoped<IUserCrudService, UserCrudService>();
builder.Services.AddScoped<ICustomerCrudService, CustomerCrudService>();
builder.Services.AddScoped<ISupplierCrudService, SupplierCrudService>();
builder.Services.AddScoped<IWarehouse_ProductService, Warehouse_ProductService>();

// Auth Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Register Repository and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

//Automatic Registeration
builder.Services.AddValidatorsFromAssemblyContaining<UserUpdateDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateDTOValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<WarehouseCreateDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<WarehouseUpdateDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SupplyOrderCreateDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TransferOrderCreateDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ReleaseOrderCreateDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SO_ProductCreateDTOValidator>();
//builder.Services.AddScoped<IValidator<UserUpdateDTO>, UserUpdateDTOValidator>();
// Register Custom Services .....

//Swagger Services
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//CORS
app.UseCors("AllowReactApp");
//CORS

app.UseHttpsRedirection();

app.UseAuthorization();

// Seed roles and initial user
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await SeedRolesAndUserAsync(roleManager, userManager);
}

app.MapControllers();

app.MapGet("/", () => "lksdjflkds");

async Task SeedRolesAndUserAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
{
    // Seed roles
    string[] roles = { "Owner", "Manager", "Employee" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed initial Owner user
    var ownerEmail = "owner@example.com";
    var ownerUser = await userManager.FindByEmailAsync(ownerEmail);
    if (ownerUser == null)
    {
        var user = new ApplicationUser
        {
            UserName = "owner",
            Email = ownerEmail,
            Name = "System Owner"
        };
        var result = await userManager.CreateAsync(user, "Owner123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Owner");
        }
    }
}

app.Run();
