using PruebaTecnica_DVP_Net_Kubernetes.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Load UserData settings from appsettings.json BEFORE building the app
builder.Services.Configure<UserDataConfig>(builder.Configuration.GetSection("UserData"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// To not use https locally
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
