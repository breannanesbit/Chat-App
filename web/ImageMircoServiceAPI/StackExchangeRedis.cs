using Newtonsoft.Json;
using StackExchange.Redis;

namespace ImageMircoServiceAPI
{

    public class RedisClient
    {
        private readonly IDatabase _database;

        public RedisClient(ConnectionMultiplexer connectionMultiplexer)
        {
            if (connectionMultiplexer == null)
            {
                throw new ArgumentNullException(nameof(connectionMultiplexer));
            }

            _database = connectionMultiplexer.GetDatabase();
        }

        public T Get<T>(string key)
        {
            var redisValue = _database.StringGet(key);
            if (redisValue.HasValue)
            {
                // Deserialize the value from Redis
                return JsonConvert.DeserializeObject<T>(redisValue);
            }

            return default(T);
        }

        public void Set<T>(string key, T value)
        {
            // Serialize the value to be stored in Redis
            var redisValue = JsonConvert.SerializeObject(value);

            // Set the value in Redis with an optional expiration time
            _database.StringSet(key, redisValue, flags: CommandFlags.FireAndForget);
        }

        public void Delete(string key)
        {
            _database.KeyDelete(key, CommandFlags.FireAndForget);
        }
    }


}
