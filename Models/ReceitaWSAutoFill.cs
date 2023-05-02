namespace GeradorDeDados.Models
{
    public class ReceitaWSAutoFill
    {
        public bool AutoFill { get; set; } = true
        public int MinFill { get; set; } = 1000
        public int MaxFill { get; set; } = 2000
    }
}
