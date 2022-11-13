namespace MinecraftServer.Api.Models
{
    public class ApiCicloDeVida
    {
        public DateTime iniciouEm { get; set; }

        public ApiCicloDeVida()
        {
            this.iniciouEm = DateTime.Now;
        }
    }
}
