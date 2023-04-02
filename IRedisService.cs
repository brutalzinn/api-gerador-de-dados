namespace GeradorDeDados
{
    public interface IRedisService
    {
        T Get<T>(string chave);
        T Set<T>(string chave, T valor, int expiracao);
        T Set<T>(string chave, T valor);
        T ItemAdd<T>(string chave, T valor);
        void ItemRemove<T>(string chave, int index);
        bool Clear(string chave);
        bool Exists(string chave);
    }
}
