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
    public class FileStatService : ICrud<FileStat>
    {
        //FileGueDB db;
        RedisConnectionProvider provider;
        IRedisCollection<FileStat> db;
        UserProfileService UserSvc;

        public FileStatService(RedisConnectionProvider provider, UserProfileService userservice)
        {
            this.UserSvc = userservice;
            this.provider = provider;
            db = this.provider.RedisCollection<FileStat>();
        }

        public void RefreshEntity(FileStat item)
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
        public bool DeleteData(FileStat item)
        {
            db.Delete(item);
            return true;
        }

        public List<FileStat> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.Name.Contains(Keyword));
            return data.ToList();
        }

        public List<FileStat> GetAllData()
        {
            return db.ToList();
        }

        public FileStat GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }
        
        public List<FileStat> GetByUsername(string Username)
        {
            return db.Where(x => x.Username == Username).ToList();
        }


        public bool InsertData(FileStat data)
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

        public bool UpdateData(FileStat data)
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

        public FileStat GetDataById(object Id)
        {
            return db.Where(x => x.Id == Id.ToString()).FirstOrDefault();
        }

        public long GetLastId()
        {
            throw new NotImplementedException();
        }
    }

}
