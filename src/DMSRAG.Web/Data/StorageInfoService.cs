using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Pdf.Content.Objects;
using Redis.OM;
using Redis.OM.Searching;
using DMSRAG.Web.Data;
using DMSRAG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
namespace DMSRAG.Web.Data
{
    public class StorageInfoService : ICrud<StorageInfo>
    {
        //FileGueDB db;
        RedisConnectionProvider provider;
        IRedisCollection<StorageInfo> db;
        UserProfileService UserSvc;
        DriveService DriveSvc;

        public StorageInfoService(RedisConnectionProvider provider, UserProfileService userservice)
        {
            this.UserSvc = userservice;
            this.provider = provider;
            
            db = this.provider.RedisCollection<StorageInfo>();
        }

      
        public bool DeleteData(StorageInfo item)
        {
            db.Delete(item);
            return true;
        }

        public StorageInfo GetByUsername(string Username)
        {
            var data = db.Where(x => x.Username==Username).FirstOrDefault();
            if (data == null)
            {
                var newItem = new StorageInfo() { Username = Username, CreatedDate = DateHelper.GetLocalTimeNow() };
                var res = db.Insert(newItem);
                data = newItem;
            }
            return data;
        }

        public List<StorageInfo> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.Username.Contains(Keyword) || x.PackageName.Contains(Keyword));
            return data.ToList();
        }

        public List<StorageInfo> GetAllData()
        {
            return db.ToList();
        }

        public StorageInfo GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(StorageInfo data)
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

        public bool UpdateData(StorageInfo data)
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

        public StorageInfo GetDataById(object Id)
        {
            return db.Where(x => x.Id == Id.ToString()).FirstOrDefault();
        }

        public long GetLastId()
        {
            throw new NotImplementedException();
        }
    }

}
