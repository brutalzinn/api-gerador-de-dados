namespace GeradorDeDados.Models
{
    public class ReceitaWSAutoFill
    {
        public bool AutoFill { get; set; }
        public int MinFill { get; set; } = 100;
        public int MaxFill { get; set; } = 1000;
    }
}
