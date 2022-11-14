namespace GeradorDeDados.Models.Settings
{
    public class ApiConfig
    {
        public bool Swagger { get; set; }
        public Authorization Authorization { get; set; }
    }

    public class Authorization
    {
        public bool Activate { get; set; }
        public string ApiHeader { get; set; }
        public string ApiKey { get; set; }
    }
}
