using ConfigurationSubstitution;
using GeradorDeDados;
using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;
using GeradorDeDados.Services;
using Microsoft.AspNetCore.Mvc;
using RestEase.HttpClientFactory;

var builder = WebApplication.CreateBuilder(args);


var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .EnableSubstitutions("%", "%")
            .Build();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddHostedService<CNPJBackgroundWorker>();
builder.Services.AddRestEaseClient<IReceitaWS>("https://receitaws.com.br");
builder.Services.AddSingleton<ApiCicloDeVidaService>();
builder.Services.AddSingleton<ConfigReceitaWSService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = ObterRedisContext();
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (config.GetSection("Swagger").Get<bool>())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapGet("/obterCNPJValido/{unicoSocio}", ([FromRoute] bool unicoSocio, [FromServices] IRedisService redisService) =>
{
    var listaCNPJValido = redisService.Get<List<ReceitaWSResponse>>("cnpjs").ToList();
    var CNPJValido = listaCNPJValido.FirstOrDefault();

    if (unicoSocio)
    {
        CNPJValido = listaCNPJValido.FirstOrDefault(x => x.Qsa.Count == 1);
    }
    var resultado = CNPJValido;

    var removeIndex = listaCNPJValido.IndexOf(CNPJValido);
    redisService.ItemRemove<ReceitaWSResponse>("cnpjs", removeIndex);

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

app.MapPost("/ReceitaWSBackgroundWorker", ([FromBody] ConfigReceitaWSRequest request, [FromServices] ConfigReceitaWSService configReceitaWSService, [FromServices] IRedisService redisService) =>
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
