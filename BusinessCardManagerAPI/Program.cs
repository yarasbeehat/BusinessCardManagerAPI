using BusinessCardManager.Infrastructure.Context;
using BusinessCardManager.Infrastructure.Interfaces;
using BusinessCardManager.Application.Mapping;
using BusinessCardManager.Infrastructure.UnitOfWork;
using BusinessCardManager.Application.Services;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using BusinessCardManager.Domain.Interfaces.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BusinessCardManagerContext>();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IBusinessCardService, BusinessCardService>();

//builder.Services.AddScoped<IRoleService, RoleService>();

//builder.Services.AddScoped<IRoleUserService, RoleUserService>();

builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder.AllowAnyOrigin()
       .AllowAnyMethod()
       .AllowAnyHeader());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type= ReferenceType.SecurityScheme,
                Id=JwtBearerDefaults.AuthenticationScheme
            }
        },new string[]{ }
        }
    });
});




builder.Services.AddHttpContextAccessor();



var app = builder.Build();

app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
