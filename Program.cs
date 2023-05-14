using GeradorDeDados;
using GeradorDeDados.Handlers;
using GeradorDeDados.Models.Settings;
using GeradorDeDados.Routes.Configuracoes;
using GeradorDeDados.Routes.Geradores;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        DependencyInjection.CriarInjecao(builder.Services);
        var app = builder.Build();
        var apiConfig = app.Services.GetRequiredService<IOptions<ApiConfig>>().Value;
        app.UseCors("corsapp");

        app.UseAuthentication();
        app.UseAuthorization();
        app.AddCustomExceptionHandler();
        if (apiConfig.Swagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        ConfiguracaoRoute.CriarRota(app);
        GeradorRoute.CriarRota(app);
        app.Run();
    }
}

