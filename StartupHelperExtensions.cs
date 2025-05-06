using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PixelMart.API.Data;
using PixelMart.API.DbContexts;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Repositories;
using PixelMart.API.Services;
using PixelMart.API.Services.Impl;
using System.Text;

namespace PixelMart.API;

internal static class StartupHelperExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("PixelMartDbContextConnection")
            ?? throw new InvalidOperationException("Connection string 'PixelMartDbContextConnection' not found.");

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<RequestLogHelper>();
        builder.Services.AddDbContext<PixelMartDbContext>(options => options.UseSqlServer(connectionString));

        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),

            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        builder.Services.AddSingleton(tokenValidationParameters);
        builder.Services.AddTransient<IPropertyMappingService, PropertyMappingService>();
        builder.Services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();
        builder.Services.AddScoped<IPixelMartRepository, PixelMartRepository>();

        //Add Identity
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<PixelMartDbContext>()
            .AddDefaultTokenProviders();

        //Add Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })

        //Add JWT Bearer
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = tokenValidationParameters;

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    context.NoResult();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                    {
                        StatusCode = 401,
                        Message = "Authentication failed. Token is invalid or expired."
                    }));
                },
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                    {
                        StatusCode = 401,
                        Message = "Unauthorized access. Please provide a valid token."
                    }));
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                    {
                        StatusCode = 403,
                        Message = "Forbidden. You do not have permission to access this resource."
                    }));
                }
            };
        });

        builder.Services.AddControllers(configure =>
        {
            configure.ReturnHttpNotAcceptable = true;
        }).AddNewtonsoftJson()
        .AddXmlDataContractSerializerFormatters();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAutoMapper(typeof(Program));

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        //Seed the database with user roles
        AppDbInitializer.SeedRolesToDb(app).Wait();

        return app;
    }
}
