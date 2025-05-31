using Inventory.Data.DbContexts;
using Inventory.DTO.UserDto.Validations;
using Inventory.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Inventory.DTO.UserDto.Requests;
using Inventory.DTO.WarehouseDto.Validations;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at 
builder.Services.AddOpenApi();

//connect to Database EF ......
builder.Services.AddDbContext<SqlDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlDbConnection"))
);
//connect to Database EF ......

// Register Custom Services .....
builder.Services.AddScoped<IUserCrudService, UserCrudService>();
builder.Services.AddScoped<ICustomerCrudService, CustomerCrudService>();
builder.Services.AddScoped<ISupplierCrudService, SupplierCrudService>();

//Automatic Registeration
builder.Services.AddValidatorsFromAssemblyContaining<UserUpdateDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserCreateDTOValidator>();

builder.Services.AddValidatorsFromAssemblyContaining<WarehouseCreateDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<WarehouseUpdateDTOValidator>();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "lksdjflkds");

app.Run();
