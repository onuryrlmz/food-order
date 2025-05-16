using System.Text.Json;
using StackExchange.Redis;

namespace Application.Services.Common;

public class RedisManager : IRedisService
{
    private readonly IDatabase _database;

    public RedisManager(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task SetValueAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, expiry);
    }

    public async Task<T> GetValueAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task<(T value, string key)> SearchValueAsync<T>(string searchKey)
    {
        var endpoints = _database.Multiplexer.GetEndPoints();
        if (endpoints.Length == 0)
        {
            return (default, null)!;
        }

        var server = _database.Multiplexer.GetServer(endpoints.First());
        var keys = server.Keys();

        foreach (var key in keys)
        {
            var value = await _database.StringGetAsync(key);
            if (!value.HasValue || !value.ToString().Contains(searchKey))
                continue;

            try
            {
                var deserializedValue = JsonSerializer.Deserialize<T>(value);
                return (deserializedValue, key);
            }
            catch (JsonException)
            {
            }
        }

        return (default, null)!;
    }

    public async Task<bool> DeleteKeyAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> KeyExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }
}