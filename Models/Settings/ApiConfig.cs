namespace GeradorDeDados.Models.Settings
{
    /// <summary>
    /// Mantendo as configurações de estrutura em inglês
    /// </summary>
    public class ApiConfig
    {
        public bool Swagger { get; set; }
        public string CorsOrigin { get; set; }
        public Authorization Authorization { get; set; }
        public CacheConfig CacheConfig { get; set; }
    }

    public class Authorization
    {
        public bool Activate { get; set; }
        public string ApiHeader { get; set; }
        public string ApiKey { get; set; }
    }
    public class CacheConfig
    {
        public string ExpireEvery { get; set; }
    }
}