namespace GeradorDeDados.Models
{
    public class HealthCheckResponse
    {
        public string UltimoDeploy { get; set; }
        public string UpTime { get; set; }
        public string Ambiente { get; set; }

        public int CnpjsRegistrados { get; set; }
    }
}
