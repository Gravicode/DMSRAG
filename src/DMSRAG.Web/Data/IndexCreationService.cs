

using DMSRAG.Models;
using DMSRAG.Web.Data;
using StackExchange.Redis;

namespace Redis.OM.Skeleton.HostedServices;

public class IndexCreationService 
{
    private readonly RedisConnectionProvider _provider;
    public IndexCreationService()
    {
        var options = ConfigurationOptions.Parse(AppConstants.RedisCon); // host1:port1, host2:port2, ...
        if (!string.IsNullOrEmpty(AppConstants.RedisPassword))
        {

            options.Password = AppConstants.RedisPassword;

        }
        _provider = new RedisConnectionProvider(options);

    }

    public async Task CreateIndex()
    {
        await _provider.Connection.CreateIndexAsync(typeof(CacheData));
        await _provider.Connection.CreateIndexAsync(typeof(UserProfile));
        await _provider.Connection.CreateIndexAsync(typeof(Log));
        await _provider.Connection.CreateIndexAsync(typeof(PageView));
        await _provider.Connection.CreateIndexAsync(typeof(StorageInfo));
        await _provider.Connection.CreateIndexAsync(typeof(Recycle));
        await _provider.Connection.CreateIndexAsync(typeof(FileStat));
        
    }

}