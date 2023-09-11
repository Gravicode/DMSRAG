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
using ServiceStack;

namespace DMSRAG.Web.Data
{
    public class NotificationService : ICrud<Notification>
    {
        //FileGueDB db;
        RedisConnectionProvider provider;
        IRedisCollection<Notification> db;
        UserProfileService UserSvc;

        public NotificationService(RedisConnectionProvider provider, UserProfileService userservice)
        {
            this.UserSvc = userservice;
            this.provider = provider;
            db = this.provider.RedisCollection<Notification>();
        }
        public List<Notification> GetLatestNotifications(string username)
        {
            var data = db.Where(x => x.UserName == username).ToList();
            return data.Where(x=>!x.IsRead).OrderByDescending(x => x.CreatedDate).ToList();
        }
        public void RefreshEntity(Notification item)
        {
            try
            {
                //do nothing
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }


        }
        public bool DeleteData(Notification item)
        {
            db.Delete(item);
            return true;
        }

        public List<Notification> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.Message.Contains(Keyword));
            return data.ToList();
        }

        public List<Notification> GetAllData()
        {
            return db.ToList();
        }

        public Notification GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(Notification data)
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

        public bool UpdateData(Notification data)
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

        public Notification GetDataById(object Id)
        {
            return db.Where(x => x.Id == Id.ToString()).FirstOrDefault();
        }

        public long GetLastId()
        {
            throw new NotImplementedException();
        }
    }

}
