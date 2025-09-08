using Inventory.Data.Service.Data;
using Inventory.Data.Service.Middleware;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Inventory.Data.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Base de Datos 
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseInMemoryDatabase("InventoryDB"));



builder.Services.AddHostedService<QueueProcessorService>();

// Configuración de FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);


// Añade los servicios para los controladores de la API.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
