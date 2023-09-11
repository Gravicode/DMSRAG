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
    public class DocFunctionService : ICrud<DocFunction>
    {
        //FileGueDB db;
        RedisConnectionProvider provider;
        IRedisCollection<DocFunction> db;
        UserProfileService UserSvc;

        public DocFunctionService(RedisConnectionProvider provider, UserProfileService userservice)
        {
            this.UserSvc = userservice;
            this.provider = provider;
            db = this.provider.RedisCollection<DocFunction>();
        }

        public void RefreshEntity(DocFunction item)
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
        public bool DeleteData(DocFunction item)
        {
            db.Delete(item);
            return true;
        }

        public List<DocFunction> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.FunctionName.Contains(Keyword));
            return data.ToList();
        }

        public List<DocFunction> GetAllData()
        {
            return db.ToList();
        }

        public DocFunction GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }

        public bool IsExist(string FuncName)
        {
            return db.Any(x => x.FunctionName == FuncName);
        }
        public bool InsertData(DocFunction data)
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

        public bool UpdateData(DocFunction data)
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

        public DocFunction GetDataById(object Id)
        {
            return db.Where(x => x.Id == Id.ToString()).FirstOrDefault();
        }

        public long GetLastId()
        {
            throw new NotImplementedException();
        }
    }

}
