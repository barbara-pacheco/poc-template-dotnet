namespace Sample.Application.Configuration;

public static class SwaggerExtensions
{
    /// <summary>
    /// Expõe o Swagger (UI em /swagger) só em Development. Em homolog e
    /// produção ele não existe.
    /// </summary>
    public static WebApplication UseSwaggerDevelopment(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}
