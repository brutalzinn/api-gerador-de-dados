namespace GeradorDeDados.Models
{
    [Serializable]
    public class CustomException : Exception
    {
        public int StatusCode { get; }
        public TipoExcecao Type { get; }

        public CustomException(TipoExcecao tipoExcecao, string message)
       : base(message)
        {
            Type = tipoExcecao;
            StatusCode = (int)tipoExcecao;
        }

        public CustomExceptionResponse ObterResponse()
        {
            var response = new CustomExceptionResponse()
            {
                Type = GetType(),
                Message = Message
            };

            return response;
        }

        private string GetType()
        {
            switch (Type)
            {
                case TipoExcecao.NEGOCIO:
                    return "negocio";
                case TipoExcecao.AUTORIZACAO:
                    return "nao_autorizado";
                case TipoExcecao.VALIDACAO:
                    return "validacao";

            }
            return "not_recognized";
        }
    }
}
