using Sample.Application.Configuration;
using Sample.Shared.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddServices(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwaggerDevelopment();

app.MapGet("/", () => Results.Ok(new { service = "Sample.Application", status = "ok" }));
app.MapModules();

app.Run();
