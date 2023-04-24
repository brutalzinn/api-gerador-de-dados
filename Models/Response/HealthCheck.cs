using GeradorDeDados.Services;

namespace GeradorDeDados.Models.Response
{
    public class HealthCheck
    {
        public string UltimoDeploy { get; set; }
        public string UpTime { get; set; }
        public string Ambiente { get; set; }
        public ReceitaWSConfig ReceitaWSConfig { get; set; }
        public DadosEmpresasRegistradas DadosEmpresasRegistradas { get; set; }
    }
}
