using Sample.Application;
using Sample.Infrastructure;
using Sample.WebApi;
using Sample.WebApi._Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWebApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwaggerDevelopment();
app.MapEndpoints();

app.Run();
