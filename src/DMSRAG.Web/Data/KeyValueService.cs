using DMSRAG.Models;
using Redis.OM.Searching;
using Redis.OM;
using ServiceStack.Redis;

namespace DMSRAG.Web.Data
{
    public class KeyValueService 
    {
        //FileGueDB db;
        RedisConnectionProvider provider;
        IRedisClient db;
        UserProfileService UserSvc;

        public KeyValueService()
        {
           
            db = new RedisClient(AppConstants.RedisCon);
        }

        public void SetKey(string Key,string Value)
        {
            db.Set<string>(Key, Value);
        }

        public string GetKey(string Key)
        {
            try
            {
                return db.Get<string>(Key);
            }
            catch (Exception)
            {

                return null;
            }
           
        }
    }
    }
