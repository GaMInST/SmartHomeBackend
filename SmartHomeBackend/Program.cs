using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // Add this using directive  
using SmartHomeBackend.Config;
using SmartHomeBackend.Services;
using SmartHomeBackend.Models;

var builder = WebApplication.CreateBuilder(args);


// Add configuration  
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<TuyaSettings>(builder.Configuration.GetSection("Tuya"));
builder.Services.AddHttpClient<TuyaService>();
// Register TuyaService with named HttpClient  
builder.Services.Configure<TuyaSettings>(builder.Configuration.GetSection("Tuya"));
builder.Services.AddHttpClient<TuyaService>();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment()) // This requires Microsoft.Extensions.Hosting  
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
