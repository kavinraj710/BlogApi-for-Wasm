using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoDBService>();

builder.Services.AddControllers();

// Add Swagger generation service
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Enable Swagger middleware and Swagger UI only in Development or Production as needed
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    // This registers the Swagger endpoint JSON for Swagger UI
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API V1");

    // If you want Swagger UI at the root URL (i.e. at "/"), uncomment below:
    // c.RoutePrefix = string.Empty;
    // By default, Swagger UI is served at "/swagger"
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
