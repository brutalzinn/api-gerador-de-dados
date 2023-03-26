using Bogus;
using Bogus.Extensions;
using Bogus.Extensions.Brazil;
using StringPlaceholder;

namespace GeradorDeDados.Placeholder
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
                new StringExecutor("LOGRADOURO", ()=> _faker.Address.StreetAddress(false), "Cria um logradouro"),
                new StringExecutor("CEP", ()=> _faker.Address.ZipCode("########"), "Cria um cep"),
                new StringExecutor("EMAIL", ()=> _faker.Person.Email.ToLower(), "Cria um email"),
                new StringExecutor("CIDADE", ()=> _faker.Address.City(), "Cria uma cidade"),
                new StringExecutor("BAIRRO", ()=> _faker.Address.City(), "Cria um bairro"),
                new StringExecutor("DINHEIRO", ()=>  _faker.Random.Decimal2(100,5000).ToString("0.##")),
                new StringExecutor("NOME_COMPLETO", ()=> _faker.Person.FullName, "Cria um nome completo de pessoa"),
                new StringExecutor("RG", ()=> _faker.Random.String2(8, "0123456789"), "Cria um RG"),
                new StringExecutor("CPF", ()=> _faker.Person.Cpf(false), "Cria um CPF"),
                new StringExecutor("CNPJ",()=> _faker.Company.Cnpj(false), "Cria um CNPJ"),
                new StringExecutor("COMPANYNAME",()=> _faker.Company.CompanyName(), "Cria um nome de empresa aleatório."),
                new StringExecutor("BASE64", GetImageAsBase64Url, "Cria um base64 dado uma url de imagem como parâmetro.",
                new List<string>()
                {
                "imageUrl"
                })
            };

            return placeholders;
        }
    }
}
