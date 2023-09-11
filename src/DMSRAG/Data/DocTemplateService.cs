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
    public class DocTemplateService : ICrud<DocTemplate>
    {
        //FileGueDB db;
        RedisConnectionProvider provider;
        IRedisCollection<DocTemplate> db;
        UserProfileService UserSvc;

        public DocTemplateService(RedisConnectionProvider provider, UserProfileService userservice)
        {
            this.UserSvc = userservice;
            this.provider = provider;
            db = this.provider.RedisCollection<DocTemplate>();
        }

        public void RefreshEntity(DocTemplate item)
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
        public bool DeleteData(DocTemplate item)
        {
            db.Delete(item);
            return true;
        }

        public List<DocTemplate> FindByKeyword(string Keyword)
        {
            var data = db.Where(x => x.DocName.Contains(Keyword));
            return data.ToList();
        }

        public List<DocTemplate> GetAllData()
        {
            return db.ToList();
        }

        public DocTemplate GetDataById(string Id)
        {
            return db.Where(x => x.Id == Id).FirstOrDefault();
        }


        public bool InsertData(DocTemplate data)
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

        public bool UpdateData(DocTemplate data)
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

        public DocTemplate GetDataById(object Id)
        {
            return db.Where(x => x.Id == Id.ToString()).FirstOrDefault();
        }

        public long GetLastId()
        {
            throw new NotImplementedException();
        }
    }

}
