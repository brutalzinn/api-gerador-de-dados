using ConfigurationSubstitution;
using GeradorDeDados;
using GeradorDeDados.Authentication;
using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;
using GeradorDeDados.Models.Settings;
using GeradorDeDados.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using RestEase.HttpClientFactory;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var contact = new OpenApiContact()
        {
            Name = "Roberto Paes",
            Email = "contato@robertinho.net",
            Url = new Uri("http://robertocpaes.dev")
        };


        var info = new OpenApiInfo()
        {
            Version = "v1",
            Title = "Gerador de documentos (CNPJ)",
            Description = "Minimal API para gerar lista de cnpj validados pela receitaWS",
            Contact = contact
        };


        var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .EnableSubstitutions("%", "%")
                    .Build();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", info);
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "ApiKey must appear in header",
                Type = SecuritySchemeType.ApiKey,
                Name = "ApiKey",
                In = ParameterLocation.Header,
                Scheme = "ApiKeyScheme"
            });
            var key = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            };
            var requirement = new OpenApiSecurityRequirement
                    {
                             { key, new List<string>() }
                    };
            c.AddSecurityRequirement(requirement);

        });






        builder.Services.AddSingleton<IRedisService, RedisService>();
        builder.Services.AddHostedService<CNPJBackgroundWorker>();
        builder.Services.AddRestEaseClient<IReceitaWS>("https://receitaws.com.br");
        builder.Services.AddSingleton<ApiCicloDeVidaService>();
        builder.Services.AddSingleton<ConfigReceitaWSService>();
        builder.Services.Configure<ApiConfig>(options => config.GetSection("ApiConfig").Bind(options));

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = ObterRedisContext();
        });


        builder.Services.AddAuthentication("ApiKey")
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>
            ("ApiKey", null);

        builder.Services.AddAuthorization();


        var app = builder.Build();


        app.UseAuthentication();
        app.UseAuthorization();
        // Configure the HTTP request pipeline.
        if (config.GetSection("ApiConfig").Get<ApiConfig>().Swagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.MapGet("/obterCNPJValido/{unicoSocio}", [Authorize(AuthenticationSchemes = "ApiKey")] ([FromRoute] bool unicoSocio, [FromServices] IRedisService redisService) =>
        {
            var listaCNPJValido = redisService.Get<List<ReceitaWSResponse>>("cnpjs").ToList();
            var CNPJValido = listaCNPJValido.FirstOrDefault();

            if (unicoSocio)
            {
                CNPJValido = listaCNPJValido.FirstOrDefault(x => x.Qsa.Count == 1);
                if (CNPJValido == null)
                {
                    return Results.Ok("Nenhuma empresa com único sócio encontrada.");
                }
            }

            if (CNPJValido == null)
            {
                return Results.Ok("Nenhuma empresa encontrada.");
            }
            var resultado = CNPJValido;

            var removeIndex = listaCNPJValido.IndexOf(CNPJValido);
            if (removeIndex != -1)
            {
                redisService.ItemRemove<ReceitaWSResponse>("cnpjs", removeIndex);
            }

            return Results.Ok(resultado);
        }).WithTags("Geradores");

        app.MapGet("/", ([FromServices] ApiCicloDeVidaService apiCicloDeVida, [FromServices] ConfigReceitaWSService configReceitaWSService, [FromServices] IRedisService redisService) =>
        {
            var ultimoDeploy = "Último deploy " + apiCicloDeVida.iniciouEm.ToString("dd/MM/yyyy HH:mm:ss");
            var upTime = DateTime.Now.Subtract(apiCicloDeVida.iniciouEm).ToString("c");
            var ambiente = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var cnpjsRegistrados = redisService.Get<List<ReceitaWSResponse>>("cnpjs").ToList().Count;
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

        string ObterRedisContext()
        {
            var redisContextUrl = config.GetConnectionString("Redis");
            Uri redisUrl;
            bool isRedisUrl = Uri.TryCreate(redisContextUrl, UriKind.Absolute, out redisUrl);
            if (isRedisUrl)
            {
                redisContextUrl = string.Format("{0}:{1},password={2}", redisUrl.Host, redisUrl.Port, redisUrl.UserInfo.Split(':')[1]);
            }
            return redisContextUrl;
        }

        app.Run();
    }
}