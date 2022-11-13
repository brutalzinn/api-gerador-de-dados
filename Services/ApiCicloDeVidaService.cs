namespace GeradorDeDados.Services
{
    public class ApiCicloDeVidaService
    {
        public DateTime iniciouEm { get; set; }

        public ApiCicloDeVidaService()
        {
            iniciouEm = DateTime.Now;
        }
    }
}
