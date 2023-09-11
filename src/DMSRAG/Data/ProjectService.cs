using DMSRAG.Models;
using Microsoft.EntityFrameworkCore;
using DMSRAG.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMSRAG.Web.Data
{
    public class ProjectService : ICrudDb<Project>
    {
        DMSRAGDb db;

        public ProjectService()
        {
            if (db == null) db = new DMSRAGDb();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.Projects.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.Projects.Remove(selData);
            db.SaveChanges();
            return true;
        }
        
        public List<Project> FindByKeyword(string Keyword)
        {
            var data = from x in db.Projects
                       where x.Name.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<Project> GetAllData()
        {
            return db.Projects.ToList();
        }
        public List<Project> GetAllData(string username)
        {
            return db.Projects.Where(x => x.Username == username).ToList();
        }
        public Project GetDataById(object Id)
        {
            return db.Projects.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(Project data)
        {
            try
            {
                db.Projects.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }
        
        public bool UpdateData(Project data)
        {
            try
            {
                db.Entry(data).State = EntityState.Modified;
                db.SaveChanges();

                /*
                if (sel != null)
                {
                    sel.Nama = data.Nama;
                    sel.Keterangan = data.Keterangan;
                    sel.Tanggal = data.Tanggal;
                    sel.DocumentUrl = data.DocumentUrl;
                    sel.StreamUrl = data.StreamUrl;
                    return true;

                }*/
                return true;
            }
            catch
            {

            }
            return false;
        }

        public long GetLastId()
        {
            return db.Projects.Max(x => x.Id);
        }
    }

}