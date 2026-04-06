using API.Exceptions;
using Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssembly(typeof(IAssemblyMarker).Assembly);
});

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<ConflictExceptionHandler>();
builder.Services.AddExceptionHandler<ForbiddenExceptionHandler>();
builder.Services.AddExceptionHandler<UnauthorizedExceptionHandler>(); 
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();      

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/", () => "Hello World!");

app.Run();
