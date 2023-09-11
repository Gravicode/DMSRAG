using Redis.OM;
using Redis.OM.Searching;
using DMSRAG.Models;
using System.Linq;
using Newtonsoft.Json;

namespace DMSRAG.Web.Data
{
    public class CacheDataService : ICrud<CacheData>
    {
        //FileGueDB db;
        RedisConnectionProvider provider;
        IRedisCollection<CacheData> db;
        UserProfileService UserSvc;

        public CacheDataService(RedisConnectionProvider provider, UserProfileService userservice)
        {
            this.UserSvc = userservice;
            this.provider = provider;
            db = this.provider.RedisCollection<CacheData>();
        }

        public bool DeleteData(CacheData item)
        {
            db.Delete(item);
            return true;
        }

        public T GetDataByKey<T>(string Username, string Key)
        {
            try
            {
                var data = db.Where(x => x.KeyString == Key && x.Username == Username).FirstOrDefault();
            return data==null ? default(T) : JsonConvert.DeserializeObject<T>(data.DataString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return default(T);
        }

        public bool InsertData<T>(string Username, string Key, T objData)
        {
            var res = false;
            try
            {
                var data = db.Where(x => x.KeyString == Key && x.Username == Username).FirstOrDefault();
                if (data == null)
                {
                    data = new CacheData() { Username = Username, LastUpdate = DateHelper.GetLocalTimeNow(), KeyString = Key, DataString = JsonConvert.SerializeObject(objData) };
                    db.Insert(data);
                }
                else
                {
                    data.DataString = JsonConvert.SerializeObject(objData);
                    data.LastUpdate = DateHelper.GetLocalTimeNow();
                    db.Update(data);
                }
                res = true;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return res;
        }
        public List<CacheData> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.KeyString.Contains(Keyword));
            return data.ToList();
        }

        public List<CacheData> GetAllData()
        {
            return db.ToList();
        }

        public CacheData GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(CacheData data)
        {
            try
            {
                db.Insert(data);
                //db.Save();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;

        }

        public bool UpdateData(CacheData data)
        {
            try
            {
                db.Update(data);
                //db.Save();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public CacheData GetDataById(object Id)
        {
            return db.Where(x => x.Id == Id.ToString()).FirstOrDefault();
        }

        public long GetLastId()
        {
            throw new NotImplementedException();
        }

        public bool DeleteData(object Id)
        {
            throw new NotImplementedException();
        }
    }

}
