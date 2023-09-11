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
    public class PageViewService : ICrud<PageView>
    {
        //FileGueDB db;
        RedisConnectionProvider provider;
        IRedisCollection<PageView> db;
        UserProfileService UserSvc;

        public PageViewService(RedisConnectionProvider provider, UserProfileService userservice)
        {
            this.UserSvc = userservice;
            this.provider = provider;
            db = this.provider.RedisCollection<PageView>();
        }

        public void RefreshEntity(PageView item)
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
        public bool DeleteData(PageView item)
        {
            db.Delete(item);
            return true;
        }

        public List<PageView> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.PageName.Contains(Keyword));
            return data.ToList();
        }

        public List<PageView> GetAllData()
        {
            return db.ToList();
        }

        public PageView GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(PageView data)
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

        public bool UpdateData(PageView data)
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

        public PageView GetDataById(object Id)
        {
            return db.Where(x => x.Id == Id.ToString()).FirstOrDefault();
        }

        public long GetLastId()
        {
            throw new NotImplementedException();
        }
    }

}
