using GeradorDeDados;
using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;
using GeradorDeDados.Models.Settings;
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

        //app.UseHttpsRedirection();

        app.MapGet("/obterCNPJValido/{filtroSocio}", [Authorize(AuthenticationSchemes = "ApiKey")] ([FromRoute] FiltroSocio filtroSocio, [FromServices] IRedisService redisService) =>
        {
            var listaCNPJValido = redisService.Get<List<ReceitaWSResponse>>("cnpjs");
            if (listaCNPJValido == null)
            {
                return Results.Ok("Nenhuma empresa disponível.");
            }
            ReceitaWSResponse CNPJEncontrado = null;
            switch (filtroSocio)
            {
                case FiltroSocio.Aleatorio:
                    CNPJEncontrado = listaCNPJValido.FirstOrDefault();
                    break;
                case FiltroSocio.VariosSocios:
                    CNPJEncontrado = listaCNPJValido.FirstOrDefault(x => x.Qsa.Count > 1);
                    break;
                case FiltroSocio.UnicoSocio:
                    CNPJEncontrado = listaCNPJValido.FirstOrDefault(x => x.Qsa != null && x.Qsa.Count == 1);
                    break;

            }

            if (CNPJEncontrado == null)
            {
                return Results.Ok("Nenhuma empresa disponível para esse filtro.");
            }

            var removeIndex = listaCNPJValido.IndexOf(CNPJEncontrado);
            if (removeIndex != -1)
            {
                redisService.ItemRemove<ReceitaWSResponse>("cnpjs", removeIndex);
            }

            return Results.Ok(CNPJEncontrado);
        }).WithTags("Geradores");

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
            }).WithTags("Health Check");

        app.MapPost("/ReceitaWSBackgroundWorker", [Authorize(AuthenticationSchemes = "ApiKey")] ([FromBody] ConfigReceitaWSRequest request, [FromServices] ConfigReceitaWSService configReceitaWSService, [FromServices] IRedisService redisService) =>
        {
            configReceitaWSService.WorkerAtivo = request.WorkerAtivo;
            return Results.Ok();
        }).WithTags("ReceitaWS");



        app.Run();
    }
}

