using Application;
using Serilog;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((hostingContext, configuration) =>
    configuration.ReadFrom.Configuration(hostingContext.Configuration));

builder.Services.AddProblemDetails();
builder.Services.AddApplicationServices();
builder.Services.AddSwaggerervices();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseSerilogRequestLogging();

app.Run();
