using Bogus;
using Bogus.Extensions.Brazil;
using GeradorDeDados.Integrations.ReceitaWS;
using GeradorDeDados.Integrations.ReceitaWS.Models;
using GeradorDeDados.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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
            while (_configReceitaWSService.WorkerAtivo)
            {
                _logger.LogInformation("Serviço rodando em {time}", DateTimeOffset.Now);
                await GerarCNPJValido();
                await Task.Delay(TimeSpan.FromSeconds(20));
            }
        }

        private async Task GerarCNPJValido()
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
                _logger.LogInformation("{0} falha na solicitação: {1}", cnpj, e);
            }
        }
    }
}
