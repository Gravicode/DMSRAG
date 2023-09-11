using Redis.OM;
using DMSRAG.Models;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace DMSRAG.Web.Data
{
    public class ShareLinkService:ICrud<ShareLink>
    {
        IRedisTypedClient<ShareLink> db;
       
        public ShareLinkService()
        {
            using var redisManager = new PooledRedisClientManager(AppConstants.RedisCon);
            using var redis = redisManager.GetClient();
            var db = redis.As<ShareLink>();
         
        }

        public bool InsertData(ShareLink data)
        {
            try
            {
                data.Id = db.GetNextSequence();
                db.Store(data);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
           
        }

        public bool UpdateData(ShareLink data)
        {
            try
            {
                
                db.Store(data);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public List<ShareLink> GetAllData()
        {
            return db.GetAll().ToList();
        }

        public List<ShareLink> FindByKeyword(string Keyword)
        {
            return db.GetAll().Where(x=>x.Name.Contains(Keyword)).ToList();
        }
        public ShareLink GetByUid(string UID)
        {
            return db.GetAll().Where(x => x.LinkUid == UID).FirstOrDefault();
        }
        public ShareLink GetDataById(object Id)
        {
            return db.GetById(Id);
        }

        public bool DeleteData(ShareLink Id)
        {
            try
            {
                var item = db.GetById(Id);
                if (item != null)
                {
                    db.Delete(item);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public long GetLastId()
        {
            return db.GetNextSequence()-1;
        }
    }

}
