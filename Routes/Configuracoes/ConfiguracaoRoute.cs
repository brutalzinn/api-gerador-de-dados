using GeradorDeDados.Models.Request;
using GeradorDeDados.Models.Response;
using GeradorDeDados.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GeradorDeDados.Routes.Configuracoes
{
    public static class ConfiguracaoRoute
    {
        public static void CriarRota(this WebApplication app)
        {
            app.MapPost("/ReceitaWSBackgroundWorker", [Authorize(AuthenticationSchemes = "ApiKey")] ([FromBody] WorkerConfig request, [FromServices] ReceitaWSConfig configReceitaWSService, [FromServices] IRedisService redisService) =>
        {
            configReceitaWSService.WorkerAtivo = request.WorkerAtivo;
            configReceitaWSService.ReceitaWSAutoFill = request.ReceitaWSAutoFill;
            return Results.Ok();
        }).WithTags("Configurações")
          .WithOpenApi(options =>
           {
               options.Summary = "Controla a tarefa de consulta em background";
               options.Description = "Use WorkerAtivo para Ligar/Desligar o BackgroundWorker de consulta ReceitaWS.";
               return options;
           });

            app.MapGet("/", ([FromServices] ApiCicloDeVida apiCicloDeVida, [FromServices] ReceitaWSConfig receitaWSConfig, IDadosReceitaWS dadosReceitaWS) =>
            {
                var ultimoDeploy = "Último deploy " + apiCicloDeVida.iniciouEm.ToString("dd/MM/yyyy HH:mm:ss");
                var upTime = DateTime.Now.Subtract(apiCicloDeVida.iniciouEm).ToString("c");
                var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var dadosEmpresa = dadosReceitaWS.ObterDadosEmpresasRegistradas();
                return Results.Ok(new HealthCheck()
                {
                    UltimoDeploy = ultimoDeploy,
                    UpTime = upTime,
                    ReceitaWSConfig = receitaWSConfig,
                    DadosEmpresasRegistradas = dadosEmpresa,
                    Ambiente = ambiente

                });
            }).WithTags("Configurações")
          .WithOpenApi(options =>
          {
              options.Summary = "Obtém informações da API";
              options.Description = "Exibe informações sobre deploy, status do BackgroundWorker de consulta e exibe número de empresas cadastradas";
              return options;
          });
        }
    }
}
