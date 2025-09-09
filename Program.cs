using Inventory.Data.Service.Data;
using Inventory.Data.Service.Middleware;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Inventory.Data.Service;
using Inventory.Data.Service.Mappings;



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
builder.Services.AddAutoMapper(typeof(MappingProfile));


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<PerformanceMetricsMiddleware>();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
