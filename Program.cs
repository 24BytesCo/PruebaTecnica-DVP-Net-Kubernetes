using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PruebaTecnica_DVP_Net_Kubernetes.Data;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using PruebaTecnica_DVP_Net_Kubernetes.Services.UserService;
using PruebaTecnica_DVP_Net_Kubernetes.Services.WorkTaskService;
using PruebaTecnica_DVP_Net_Kubernetes.Token;
using System.Text;
using PruebaTecnica_DVP_Net_Kubernetes.MappingProfile;
using PruebaTecnica_DVP_Net_Kubernetes.Services;
using PruebaTecnica_DVP_Net_Kubernetes.Filters;
using PruebaTecnica_DVP_Net_Kubernetes.Services.TaskState;
using PruebaTecnica_DVP_Net_Kubernetes.Services.Roles;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// 1. Configure Services
// ==============================

// 1.1. Configure Entity Framework with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlServerOptions => sqlServerOptions.CommandTimeout(600)));


// 1.2. Configure Identity with User and Role
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Nota: RoleManager, UserManager y SignInManager ya están registrados por AddIdentity,
// por lo que no es necesario registrarlos explícitamente a menos que se requiera personalización.

// 1.3. Register Application Services
var encryptionEnabled = builder.Configuration.GetValue<bool>("EncryptionEnabled");
var secretKey = builder.Configuration.GetValue<string>("EncryptionKey");

builder.Services.AddSingleton(new EncryptionService(secretKey!, encryptionEnabled));
// Registrando el filtro como un servicio
builder.Services.AddScoped<EncryptResponseFilter>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserSesion, UserSesion>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<IWorkTaskService, WorkTaskService>();
builder.Services.AddScoped<ITaskStateService, TaskStateService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// 1.4. Configure Strongly Typed Settings
builder.Services.Configure<UserDataConfig>(builder.Configuration.GetSection("UserData"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// 1.5. Configure Authentication with JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Configure JWT Token Validation Parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Validate the signing key
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)),
        ValidateIssuer = false, // Disable issuer validation
        ValidateAudience = false, // Disable audience validation
        ValidateLifetime = true, // Validate token expiration
        ClockSkew = TimeSpan.Zero // No clock skew
    };
});

// 1.6. Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Policy requiring 'Administrador' role
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Administrador"));

    // Policy requiring 'Supervisor' role
    options.AddPolicy("RequireSupervisorRole", policy =>
        policy.RequireRole("Supervisor"));

    // Policy requiring either 'Administrador' or 'Supervisor' roles
    options.AddPolicy("RequireAdminOrSupervisorRole", policy =>
        policy.RequireRole("Administrador", "Supervisor"));

    // Policy requiring either 'Administrador' or 'Supervisor' or 'Empleado' roles
    options.AddPolicy("RequireAdminOrSupervisorOrEmployedRole", policy =>
        policy.RequireRole("Administrador", "Supervisor", "Empleado"));
});

// 1.7. Configure CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder
            .AllowAnyOrigin() // Consider restricting to specific origins in production
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 1.8. Add Controllers with Global Authorization Filter
builder.Services.AddControllers(config =>
{
    // Apply a global authorization policy requiring authenticated users
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
    config.Filters.Add<EncryptResponseFilter>();
});

// 1.9. Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile)); // Automatically scans for profiles

// 1.10. Configure Swagger with JWT Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Define the security scheme for JWT Bearer tokens
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the JWT Bearer token **_only_**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // Must be "bearer"
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// 1.11. Add Endpoints API Explorer
builder.Services.AddEndpointsApiExplorer();

// ==============================
// 2. Build Application
// ==============================

var app = builder.Build();

// ==============================
// 3. Configure Middleware Pipeline
// ==============================

// 3.1. Enable Swagger in Development Environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3.2. Custom Middleware for Handling Unauthorized and Forbidden Responses
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 401)
    {
        await context.Response.WriteAsync("Unauthorized. Please provide a valid token.");
    }
    else if (context.Response.StatusCode == 403)
    {
        await context.Response.WriteAsync("Forbidden. You do not have permission to access this resource.");
    }
});

// 3.3. Enable CORS
app.UseCors("CorsPolicy");

// 3.4. Enable Authentication and Authorization
app.UseAuthentication(); // Validates the token
app.UseAuthorization();  // Applies authorization policies

// 3.5. Map Controllers
app.MapControllers();

// 3.6. Initialize Database and Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var userDataConfig = services.GetRequiredService<IOptions<UserDataConfig>>();

        // Apply pending migrations
        await context.Database.MigrateAsync();

        // Seed initial data
        await LoadDataBase.InsertDataAsync(context, userManager, roleManager, userDataConfig);

        // Save changes if any
        await context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        // Log the exception (consider using a logging framework)
        Console.WriteLine($"An error occurred during migration or seeding: {ex.Message}");
        throw; // Re-throw the exception after logging
    }
}

// ==============================
// 4. Run Application
// ==============================

app.Run();
