namespace GeradorDeDados.Models
{
    public class ReceitaWSConfig
    {
        public bool WorkerAtivo { get; set; }
        public ReceitaWSAutoFill ReceitaWSAutoFill { get; set; }
        public ReceitaWSConfig()
        {
            ReceitaWSAutoFill = new ReceitaWSAutoFill();
        }
    }
}
