namespace Application.Services.Common;

public interface IRedisService
{
    Task SetValueAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<T> GetValueAsync<T>(string key);
    Task<bool> DeleteKeyAsync(string key);
    Task<bool> KeyExistsAsync(string key);
    Task<(T value, string key)> SearchValueAsync<T>(string searchKey);
}