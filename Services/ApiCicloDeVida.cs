using System;

namespace GeradorDeDados.Services
{
    public class ApiCicloDeVida
    {
        public DateTime iniciouEm { get; set; }
        public ApiCicloDeVida()
        {
            iniciouEm = DateTime.Now;
        }
    }
}
