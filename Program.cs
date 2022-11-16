using Bogus;
using Bogus.Extensions.Brazil;
using GeradorDeDados;
using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;
using GeradorDeDados.Models.Settings;
using GeradorDeDados.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StringPlaceholder;
using System.Text.Json;

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
        }).WithTags("Geradores")
        .WithOpenApi(options =>
         {
             options.Summary = "Obtém um CNPJ aleatório ou filtrado e validado pela ReceitaWS";
             options.Description = "Obtém um CNPJ aleatório ou filtrado e validado pela ReceitaWS. Cada CNPJ gerado é excluído do cache. Certifique-se que há empresas cadastradas disponíveis.";
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
            }).WithTags("Health Check")
              .WithOpenApi(options =>
              {
                  options.Summary = "Obtém informações da API";
                  options.Description = "Exibe informações sobre deploy, status do BackgroundWorker de consulta e exibe número de empresas cadastradas";
                  return options;
              });

        app.MapPost("/ReceitaWSBackgroundWorker", [Authorize(AuthenticationSchemes = "ApiKey")] ([FromBody] ConfigReceitaWSRequest request, [FromServices] ConfigReceitaWSService configReceitaWSService, [FromServices] IRedisService redisService) =>
        {
            configReceitaWSService.WorkerAtivo = request.WorkerAtivo;
            return Results.Ok();
        }).WithTags("Background Workers")
          .WithOpenApi(options =>
           {
               options.Summary = "Controla a tarefa de consulta em background";
               options.Description = "Use WorkerAtivo para Ligar/Desligar o BackgroundWorker de consulta ReceitaWS.";
               return options;
           });


        app.MapPost("/placeholder", ([FromBody] JsonElement data) =>
        {
            var stringPlaceholder = new PlaceholderCreator();
            var _faker = new Faker("pt_BR");
            var listaExecutors = new List<StringExecutor>()
            {
                new StringExecutor("CPF", ()=> _faker.Person.Cpf(false)),
                new StringExecutor("CNPJ",()=> _faker.Company.Cnpj(false)),
                new StringExecutor("COMPANYNAME",()=> _faker.Company.CompanyName())
            };
            var dictionary = new Dictionary<string, string>();
            var resultado = "";

            if (data.ValueKind == JsonValueKind.Object)
            {
                var enumerate = data.EnumerateObject();
                foreach (JsonProperty p in enumerate)
                {
                    if (p.Value.ValueKind == JsonValueKind.String)
                    {
                        dictionary.Add(p.Name, p.Value.GetString());
                    }
                }
                resultado = JsonSerializer.Serialize(dictionary);
            }
            else
            {
                resultado = JsonSerializer.Serialize(data);
            }
            var result = stringPlaceholder.Creator(resultado, listaExecutors);
            return Results.Text(result, contentType: "application/json");
        }).WithTags("Fake Json")
        .WithOpenApi(options =>
         {
             options.Summary = "Substitui um placeholder por um dado aleatório ( EM TESTE )";
             options.Description = "Rota em teste. Você pode inserir um CPF/CNPJ aleatório no texto utilizando os placeholders [CPF] ou [CNPJ]";
             return options;
         });


        app.Run();
    }
}

