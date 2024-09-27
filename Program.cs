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
using PruebaTecnica_DVP_Net_Kubernetes.Token;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework con Identity
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Agregar Identity y configurar los servicios de Identity para User y Role
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Registrar RoleManager y otros servicios de Identity
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();

// Registrar la implementación de IUserService
builder.Services.AddScoped<IUserService, UserService>();

// Registrar la implementación de IUserSesion
builder.Services.AddScoped<IUserSesion, UserSesion>();

// Registrar la implementación de IJwtGenerator
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();

// Cargar configuraciones de UserData y JwtSettings
builder.Services.Configure<UserDataConfig>(builder.Configuration.GetSection("UserData"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Configuración de autenticación JWT con esquemas por defecto explícitos
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configurar políticas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Administrador"));
});

// Configurar CORS
builder.Services.AddCors(o => o.AddPolicy("corsApp", builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod()));

// Añadir controladores con filtro global de autorización (opcional)
builder.Services.AddControllers(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

// Configurar Swagger con seguridad JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tu API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese el token JWT con el prefijo Bearer",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
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
        {
            securityScheme,
            Array.Empty<string>()
        }
    });
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware para manejar errores de autenticación y autorización
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 401)
    {
        await context.Response.WriteAsync("No autorizado. Por favor, proporciona un token válido.");
    }
    else if (context.Response.StatusCode == 403)
    {
        await context.Response.WriteAsync("Prohibido. No tienes permisos para acceder a este recurso.");
    }
});

app.UseCors("corsApp");

app.UseAuthentication();  // Valida el token JWT
app.UseAuthorization();   // Aplica las políticas de autorización

app.MapControllers();

// Inicializar la base de datos
using (var ambient = app.Services.CreateScope())
{
    var services = ambient.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var context = services.GetRequiredService<AppDbContext>();
        var userDataConfig = services.GetRequiredService<IOptions<UserDataConfig>>();

        await context.Database.MigrateAsync(); // Aplicar migraciones
        await LoadDataBase.InsertDataAsync(context, userManager, roleManager, userDataConfig);
        await context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        // Aquí podrías registrar el error en un log o similar
        throw;
    }
}

app.Run();
