using GeradorDeDados;
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
        var apiConfig = app.Services.GetService<IOptions<ApiConfig>>().Value;
        app.UseAuthentication();
        app.UseAuthorization();
        // Configure the HTTP request pipeline.
        if (apiConfig.Swagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors();
        ConfiguracaoRoute.CriarRota(app);
        GeradorRoute.CriarRota(app);

        app.Run();
    }
}

