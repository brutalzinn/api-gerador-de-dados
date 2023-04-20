using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace GeradorDeDados
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _redisCache;
        private readonly DistributedCacheEntryOptions _distributedCacheEntry;

        public RedisService(IDistributedCache redisCache, DistributedCacheEntryOptions distributedCacheEntry)
        {
            _redisCache = redisCache;
            _distributedCacheEntry = distributedCacheEntry;
        }

        public T Get<T>(string chave)
        {
            var value = _redisCache.GetString(chave);
            if (value != null)
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public void Set<T>(string chave, T valor)
        {
            _redisCache.SetString(chave, JsonSerializer.Serialize(valor), _distributedCacheEntry);
        }

        public bool Clear(string chave)
        {
            _redisCache.Remove(chave);
            return default;
        }

        public bool Exists(string chave)
        {
            return _redisCache.Get(chave) != null;
        }

        /// <summary>
        /// Usar apenas para cache de listas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="chave"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        public void ItemAdd<T>(string chave, T valor)
        {
            var value = _redisCache.GetString(chave);
            var lista = new List<T>();
            if (value != null)
            {
                lista = JsonSerializer.Deserialize<List<T>>(value);
                lista.Add(valor);
                _redisCache.SetString(chave, JsonSerializer.Serialize(lista), _distributedCacheEntry);
            }
            lista.Add(valor);
            _redisCache.SetString(chave, JsonSerializer.Serialize(lista), _distributedCacheEntry);
        }


        /// <summary>
        /// Usar apenas para cache de listas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="chave"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        public void ItemRemove<T>(string chave, int index)
        {
            var value = _redisCache.GetString(chave);
            if (value != null)
            {
                return;
            }
            var lista = JsonSerializer.Deserialize<List<T>>(value);
            lista.RemoveAt(index);
            _redisCache.SetString(chave, JsonSerializer.Serialize(lista));

        }
    }
}
