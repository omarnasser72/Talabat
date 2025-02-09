using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces.Repositories;

namespace Talabat.Repository.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase database;

        public BasketRepository(IConnectionMultiplexer redis)
        {
            database = redis.GetDatabase();
        }

        public async Task<CustomerBasket?> GetBasketAsync(string BasketId)
        {
            /*
             * 1. What is RedisValue?
             * In StackExchange.Redis, RedisValue is a struct that represents the values stored in Redis.
             * It can hold various types of data including strings, integers, bytes, and more. 
             * It's a flexible type designed to handle Redis's untyped nature, where everything is stored
             * as binary data but can be interpreted in different ways depending on the application.
             *
             * Example: When you retrieve a value from Redis using database.StringGetAsync(key), 
             * the returned value is a RedisValue, which might internally hold a string representation 
             * of your serialized object (e.g., JSON string).
             */
            var basket = await database.StringGetAsync(BasketId);

            /*
             * 2. What is Deserialization?
             * Deserialization is the reverse of serialization. It is the process of converting 
             * a data format (such as JSON or XML) back into an object or a data structure that 
             * the program can manipulate.
             */
            //basket == null won't work because basket is a RedisValue, which is a value type, and value types cannot be null.
            return basket.IsNull ? null : JsonSerializer.Deserialize<CustomerBasket?>(basket);
        }

        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket)
        {
            /*
             * 1. What is Serialization?
             *    Serialization is the process of converting an object or a data structure into a format 
             *    that can be easily stored or transmitted. The format is typically a byte stream, JSON, 
             *    XML, or some other structured format. This allows the object to be saved to a file, 
             *    sent over the network, or stored in a database.
             */
            var BasketJson = JsonSerializer.Serialize(basket);

            var CreatedOrUpdated = await database.StringSetAsync(basket.Id, BasketJson, TimeSpan.FromDays(1));

            if (CreatedOrUpdated)
                return await GetBasketAsync(basket.Id);

            return null;

        }
        public async Task<bool> DeleteBasketAsync(string BasketId)
        {
            return await database.KeyDeleteAsync(BasketId);
        }

    }
}
