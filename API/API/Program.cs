using API.Exceptions;
using Application;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Host.UseSerilog((hostingContext, configuration) =>
    configuration.ReadFrom.Configuration(hostingContext.Configuration));

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<ConflictExceptionHandler>();
builder.Services.AddExceptionHandler<ForbiddenExceptionHandler>();
builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>(); 
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();      

var app = builder.Build();

app.UseExceptionHandler();
app.UseSerilogRequestLogging();

app.Run();
