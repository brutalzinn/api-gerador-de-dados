using GeradorDeDados;
using GeradorDeDados.Handlers;
using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;
using GeradorDeDados.Models.Settings;
using GeradorDeDados.Routes.Configuracoes;
using GeradorDeDados.Routes.Geradores;
using GeradorDeDados.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        DependencyInjection.CriarInjecao(builder.Services);
        var app = builder.Build();
        var apiConfig = app.Services.GetRequiredService<IOptions<ApiConfig>>().Value;
        app.UseAuthentication();
        app.UseAuthorization();
        app.AddCustomExceptionHandler();
        if (apiConfig.Swagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors("corsapp");
        ConfiguracaoRoute.CriarRota(app);
        GeradorRoute.CriarRota(app);
        PlaceholderRoute.CriarRota(app);
        app.Run();
    }
}

