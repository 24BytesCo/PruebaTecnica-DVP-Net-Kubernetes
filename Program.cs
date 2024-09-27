using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica_DVP_Net_Kubernetes.Data;
using PruebaTecnica_DVP_Net_Kubernetes.Middleware;
using PruebaTecnica_DVP_Net_Kubernetes.Models;
using PruebaTecnica_DVP_Net_Kubernetes.Services.UserService;
using PruebaTecnica_DVP_Net_Kubernetes.Token;

var builder = WebApplication.CreateBuilder(args);

// Establecer una política de autorización para los controladores.
builder.Services.AddControllers(opt =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cargar configuraciones de UserData y JwtSettings
builder.Services.Configure<UserDataConfig>(builder.Configuration.GetSection("UserData"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

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

builder.Services.AddCors(o => o.AddPolicy("corsApp", builder => builder.WithOrigins("*").AllowAnyHeader()));

// Build the app
var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<JwtValidationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
