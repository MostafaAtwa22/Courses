using Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssembly(typeof(IAssemblyMarker).Assembly);
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
