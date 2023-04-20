using Bogus;
using Bogus.Extensions.Brazil;
using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Models;
using System.Text.Json;

namespace GeradorDeDados.Works
{
    public class ReceitaWSWorker : BackgroundService
    {
        readonly ILogger<ReceitaWSWorker> _logger;
        readonly IReceitaWS _receitaWS;
        readonly IRedisService _redisService;
        readonly ReceitaWSConfig _configReceitaWSService;
        readonly Faker _faker;

        public ReceitaWSWorker(ILogger<ReceitaWSWorker> logger, IReceitaWS receitaWS,
            ReceitaWSConfig configReceitaWSService,
            IRedisService redisService)
        {
            _logger = logger;
            _configReceitaWSService = configReceitaWSService;
            _receitaWS = receitaWS;
            _redisService = redisService;
            _faker = new Faker("pt_BR");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço iniciado");
            while (true)
            {
                if (_configReceitaWSService.WorkerAtivo)
                {
                    _logger.LogInformation("Serviço rodando em {time}", DateTimeOffset.Now);
                    await GerarCNPJValido();
                }
                if (_configReceitaWSService.ReceitaWSAutoFill.AutoFill)
                {
                    var min = _configReceitaWSService.ReceitaWSAutoFill.MinFill;
                    var max = _configReceitaWSService.ReceitaWSAutoFill.MaxFill;
                    var quantity = _redisService.Get<List<ReceitaWSResponse>?>("cnpjs")?.Count() ?? 0;
                    var autoFill = quantity >= min && quantity <= max;
                    _configReceitaWSService.WorkerAtivo = autoFill;
                }
                await Task.Delay(TimeSpan.FromSeconds(20));
            }
        }

        public async Task GerarCNPJValido()
        {
            var cnpj = _faker.Company.Cnpj(false);
            ReceitaWSResponse receitaWSResponse;
            try
            {
                receitaWSResponse = await _receitaWS.ObterDadoEmpresa(cnpj);
                if (receitaWSResponse.Status.ToLower().Equals("error") == false)
                {
                    _logger.LogInformation("{cnpj} {json}", cnpj, JsonSerializer.Serialize(receitaWSResponse));
                    _redisService.ItemAdd("cnpjs", receitaWSResponse);
                }
              ;
            }
            catch (Exception e)
            {
                _logger.LogInformation("{cnpj} falha na solicitação", cnpj);
            }
        }
    }
}
