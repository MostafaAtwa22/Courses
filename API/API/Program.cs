using Application;
using Serilog;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using API.Extensions;

using API.Cors;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddRateLimiter(builder.Configuration);
builder.Services.AddCors(builder.Configuration);


builder.Host.UseSerilog((hostingContext, configuration) =>
    configuration.ReadFrom.Configuration(hostingContext.Configuration));

var app = builder.Build();

await app.UseAutoMigrationAsync();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseCors("DefaultPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapCarter();

app.Run();

public partial class Program { }
