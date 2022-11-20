using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;
using GeradorDeDados.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GeradorDeDados.Routes.Configuracoes
{
    public static class ConfiguracaoRoute
    {
        public static void CriarRota(this WebApplication app)
        {
            app.MapPost("/ReceitaWSBackgroundWorker", [Authorize(AuthenticationSchemes = "ApiKey")] ([FromBody] ConfigReceitaWSRequest request, [FromServices] ConfigReceitaWSService configReceitaWSService, [FromServices] IRedisService redisService) =>
        {
            configReceitaWSService.WorkerAtivo = request.WorkerAtivo;
            return Results.Ok();
        }).WithTags("Configurações")
          .WithOpenApi(options =>
           {
               options.Summary = "Controla a tarefa de consulta em background";
               options.Description = "Use WorkerAtivo para Ligar/Desligar o BackgroundWorker de consulta ReceitaWS.";
               return options;
           });

            app.MapGet("/", ([FromServices] ApiCicloDeVidaService apiCicloDeVida, [FromServices] ConfigReceitaWSService configReceitaWSService, [FromServices] IRedisService redisService) =>
            {
                var ultimoDeploy = "Último deploy " + apiCicloDeVida.iniciouEm.ToString("dd/MM/yyyy HH:mm:ss");
                var upTime = DateTime.Now.Subtract(apiCicloDeVida.iniciouEm).ToString("c");
                var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var listaCNPJValido = redisService.Get<List<ReceitaWSResponse>>("cnpjs");
                int cnpjsRegistrados = 0;
                if (listaCNPJValido != null)
                {
                    cnpjsRegistrados = redisService.Get<List<ReceitaWSResponse>>("cnpjs").ToList().Count;
                }
                return Results.Ok(new HealthCheckResponse()
                {
                    UltimoDeploy = ultimoDeploy,
                    UpTime = upTime,
                    WorkerAtivo = configReceitaWSService.WorkerAtivo,
                    CnpjsRegistrados = cnpjsRegistrados,
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
