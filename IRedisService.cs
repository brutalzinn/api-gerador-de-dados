namespace GeradorDeDados
{
    public interface IRedisService
    {
        T Get<T>(string chave);
        void Set<T>(string chave, T valor);
        void ItemAdd<T>(string chave, T valor);
        void ItemRemove<T>(string chave, int index);
        bool Clear(string chave);
        bool Exists(string chave);
    }
}
