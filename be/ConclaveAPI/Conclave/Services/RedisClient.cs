using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Conclave.Services
{
    /*
     * Redis client wrapper
     */
    public class RedisClient
    {
        private readonly ConnectionMultiplexer _redis;

        public RedisClient(IConfiguration config)
        {
            _redis = ConnectionMultiplexer.Connect(config["AppConfig:Cache:Redis:Host"] + ":" + config["AppConfig:Cache:Redis:Port"] + ",connectRetry=5");
        }

        public ConnectionMultiplexer GetClient()
        {
            return _redis;
        }

        public bool StringSet(string key, string value)
        {
            return _redis.GetDatabase().StringSet(key, value);
        }

        public async Task<bool> StringSetAsync(string key, string value, TimeSpan? time)
        {
            if (time != null)
            {
                return await _redis.GetDatabase().StringSetAsync(key, value, time);
            }
            else
            {
                return await _redis.GetDatabase().StringSetAsync(key, value);
            }
        }

        public string StringGet(string key)
        {
            return _redis.GetDatabase().StringGet(key);
        }
        public async Task<string> StringGetAsync(string key)
        {
            return await _redis.GetDatabase().StringGetAsync(key);
        }

        public bool KeyDelete(string key)
        {
            return _redis.GetDatabase().KeyDelete(key);
        }

        public bool KeyExists(string key)
        {
            return _redis.GetDatabase().KeyExists(key);
        }
    }
}
