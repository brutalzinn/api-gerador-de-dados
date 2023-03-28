using Bogus;
using Bogus.Extensions;
using Bogus.Extensions.Brazil;
using StringPlaceholder;

namespace GeradorDeDados.Services
{
    public class PlaceHolders
    {
        private readonly DadosReceitaWS dadosReceitaWS;
        private List<StringExecutor> executores = new List<StringExecutor>();
        private readonly Faker faker;
        public PlaceHolders(DadosReceitaWS dadosReceitaWS)
        {
            this.dadosReceitaWS = dadosReceitaWS;
            this.faker = new Faker("pt_BR");
        }

        public static string GetImageAsBase64Url(string[] strParams)
        {
            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler))
            {
                var bytes = client.GetByteArrayAsync(strParams[0]).Result;
                return Convert.ToBase64String(bytes);
            }
        }

        public List<StringExecutor> CriarPlaceholders()
        {
            executores = new List<StringExecutor>()
            {
                new StringExecutor("GUID", ()=> Guid.NewGuid().ToString(), "Cria um UUID padrão"),
                new StringExecutor("LOGRADOURO", ()=> faker.Address.StreetAddress(false), "Cria um logradouro"),
                new StringExecutor("CEP", ()=> faker.Address.ZipCode("########"), "Cria um cep"),
                new StringExecutor("EMAIL", ()=> faker.Person.Email.ToLower(), "Cria um email"),
                new StringExecutor("CIDADE", ()=> faker.Address.City(), "Cria uma cidade"),
                new StringExecutor("BAIRRO", ()=> faker.Address.City(), "Cria um bairro"),
                new StringExecutor("DINHEIRO", ()=>  faker.Random.Decimal2(100,5000).ToString("0.##")),
                new StringExecutor("NOME_COMPLETO", ()=> faker.Person.FullName, "Cria um nome completo de pessoa"),
                new StringExecutor("RG", ()=> faker.Random.String2(8, "0123456789"), "Cria um RG"),
                new StringExecutor("CPF", ()=> faker.Person.Cpf(false), "Cria um CPF"),
                new StringExecutor("CNPJ",()=> faker.Company.Cnpj(false), "Cria um CNPJ"),
                new StringExecutor("CNPJ_RECEITAWS_QSA_UM", () => dadosReceitaWS.ObterDadosEmpresaRegistrada(Models.FiltroSocio.UnicoSocio, Models.FiltroSituacao.Ativa, false, false).Cnpj, "Cria um CNPJ usando a ReceitaWS com único sócio"),
                new StringExecutor("CNPJ_RECEITAWS_QSA_VARIOS", () => dadosReceitaWS.ObterDadosEmpresaRegistrada(Models.FiltroSocio.VariosSocios, Models.FiltroSituacao.Ativa, false, false).Cnpj, "Cria um CNPJ usando a ReceitaWS com mais de um sócio"),
                new StringExecutor("COMPANYNAME",()=> faker.Company.CompanyName(), "Cria um nome de empresa aleatório."),
                new StringExecutor("IMAGE_BASE64", GetImageAsBase64Url, "Cria um base64 dado uma url de imagem como parâmetro.",
                new List<string>()
                {
                "imageUrl"
                })
            };
            return executores;
        }
    }
}
