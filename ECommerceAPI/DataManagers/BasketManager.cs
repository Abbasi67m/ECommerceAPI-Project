using StackExchange.Redis;

public class BasketManager : IBasketManager
{
    private readonly IDatabase _database;

    public BasketManager(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis");
        var redis = ConnectionMultiplexer.Connect(connectionString);
        _database = redis.GetDatabase();
    }

    public IEnumerable<string> GetAllBasket()
    {
        var endPoint = _database.Multiplexer.GetEndPoints()[0];
        var server = _database.Multiplexer.GetServer(endPoint);
        var keys = server.Keys(pattern: "buyerId:*");
        
        foreach (var key in keys)
        {
            yield return _database.StringGet(key);
        }
    }

    public BasketModel GetBasketByBuyerId(string buyerId)
    {
        var key = GetBasketKey(buyerId);
        var result = _database.StringGet(key);
        if (!result.IsNullOrEmpty)
            return JsonSerializer.Deserialize<BasketModel>(result);

        return null;
    }

    public async Task<bool> AddOrUpdateBasketAsync(BasketModel customer)
    {
        var basketKey = GetBasketKey(customer.BuyerId);
        var basketjson = JsonSerializer.Serialize(customer);
        var result = await _database.StringSetAsync(basketKey, basketjson);
        return result;
    }

    public async Task<bool> DeleteBasket(string buyerId)
    {
        var basketKey = GetBasketKey(buyerId);
        if (await _database.KeyExistsAsync(basketKey))
        {
            return _database.KeyDelete(basketKey);
        }
        return true;
    }

    private string GetBasketKey(string buyerId)
    {
        return $"buyerId:{buyerId}";
    }
}
