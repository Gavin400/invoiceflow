using System.Text.Json.Serialization;                                    
using InvoiceFlow.Infrastructure;
using Scalar.AspNetCore;
using InvoiceFlow.Api.Errors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not found.");

builder.Services.AddInfrastructure(connectionString);


builder.Services.AddExceptionHandler<UniqueConstraintExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers()                                      
    .AddJsonOptions(options =>                                           
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); 

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseStatusCodePages();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();