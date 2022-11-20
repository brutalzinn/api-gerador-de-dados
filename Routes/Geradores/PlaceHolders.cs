using Bogus;
using Bogus.Extensions.Brazil;
using StringPlaceholder;

namespace GeradorDeDados.Routes.Geradores
{
    public static class PlaceHolders
    {
        public static string GetImageAsBase64Url(string[] strParams)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                var bytes = client.GetByteArrayAsync(strParams[0]).Result;
                return Convert.ToBase64String(bytes);
            }
        }

        public static List<StringExecutor> ObterPlaceholders()
        {
            var _faker = new Faker("pt_BR");
            var placeholders = new List<StringExecutor>()
            {
                new StringExecutor("GUID", ()=> Guid.NewGuid().ToString(), "Cria um UUID padrão"),
                new StringExecutor("CPF", ()=> _faker.Person.Cpf(false)),
                new StringExecutor("CNPJ",()=> _faker.Company.Cnpj(false)),
                new StringExecutor("COMPANYNAME",()=> _faker.Company.CompanyName()),
                new StringExecutor("BASE64", GetImageAsBase64Url, "Gera uma imagem em base64 dado uma url. Aceita url de imagem como parâmetro",
                new List<string>()
                {
                "imageUrl"
                })
            };

            return placeholders;
        }
    }
}
