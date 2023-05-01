using GeradorDeDados.Integrations.ReceitaWS.Models;
using GeradorDeDados.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace GeradorDeDados.Works
{
    public class ReceitaWSAutoFillWorker : BackgroundService
    {
        readonly ILogger<ReceitaWSAutoFillWorker> _logger;
        readonly IRedisService _redisService;
        readonly ReceitaWSConfig _configReceitaWSService;

        public ReceitaWSAutoFillWorker(ILogger<ReceitaWSAutoFillWorker> logger,
            IRedisService redisService,
            ReceitaWSConfig configReceitaWSService)
        {
            _logger = logger;
            _redisService = redisService;
            _configReceitaWSService = configReceitaWSService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Serviço de autofill iniciado");
            while (true)
            {
                if (_configReceitaWSService.ReceitaWSAutoFill.AutoFill)
                {
                    var min = _configReceitaWSService.ReceitaWSAutoFill.MinFill;
                    var max = _configReceitaWSService.ReceitaWSAutoFill.MaxFill;
                    var quantity = _redisService.Get<List<ReceitaWSResponse>>("cnpjs")?.Count() ?? 0;
                    var autoFill = quantity <= min && quantity <= max;
                    _configReceitaWSService.WorkerAtivo = autoFill;
                    _logger.LogInformation("Alterando worker para {autoFill}", autoFill);
                }
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
