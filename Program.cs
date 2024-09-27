using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PruebaTecnica_DVP_Net_Kubernetes.Data;
using PruebaTecnica_DVP_Net_Kubernetes.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// Cargar la configuración de UserData desde appsettings.json ANTES de construir la aplicación
builder.Services.Configure<UserDataConfig>(builder.Configuration.GetSection("UserData"));

// Construir la aplicación
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Para no usar https en local
// app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
