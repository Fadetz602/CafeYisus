using CafeYisus.Models;
using CafeYisus.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddControllers();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminRole",
        policy => policy.RequireRole("Admin"));

    options.AddPolicy("StaffRole",
        policy => policy.RequireRole("Staff"));

    options.AddPolicy("CustomerRole",
        policy => policy.RequireRole("Customer"));

    options.AddPolicy("StaffOrAdminRole",
        policy => policy.RequireRole("Staff", "Admin"));

});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
             options =>
             {
                 options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = builder.Configuration["Jwt:Issuer"],
                     ValidAudience = builder.Configuration["Jwt:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
                 };

                 options.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                         {
                             context.Response.Headers.Append("Token-Expired", "true");
                         }
                         return Task.CompletedTask;
                     },
                 };
             });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CafeYisusDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyCnn")
    ));



var app = builder.Build();

//Seed admin if does not exist

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CafeYisusDbContext>();

    if (!context.Users.Any(u => u.RoleId == 1))
    {
        context.Users.Add(new User
        {
            Email = "admin@cafeyisus.com",
            Username = "admin",
            PasswordHash = "Admin@123", 
            FullName = "System Admin",
            RoleId = 1
        });

        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
