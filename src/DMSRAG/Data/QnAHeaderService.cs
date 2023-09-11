using Microsoft.EntityFrameworkCore;
using DMSRAG.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMSRAG.Models;

namespace DMSRAG.Web.Data
{
    public class QnAHeaderService : ICrudDb<QnAHeader>
    {
        DMSRAGDb db;

        public QnAHeaderService()
        {
            if (db == null) db = new DMSRAGDb();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.QnAHeaders.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.QnAHeaders.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<QnAHeader> FindByKeyword(string Keyword)
        {
            var data = from x in db.QnAHeaders
                       where x.Title.Contains(Keyword) || x.Title.Contains(Keyword)
                       select x;
            return data.ToList();
        }
        
  

        public List<QnAHeader> GetAllData()
        {
            return db.QnAHeaders.Include(c=>c.Project).ToList();
        }
        public List<QnAHeader> GetAllData(string username)
        {
            return db.QnAHeaders.Include(c=>c.Project).Where(x => x.Username == username).ToList();
        }
        public QnAHeader GetDataById(object Id)
        {
            return db.QnAHeaders.Include(c=>c.Project).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(QnAHeader data)
        {
            try
            {
                db.QnAHeaders.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }

        public QnAHeader GetDataByProjectId(long ProjectId)
        {
            return db.QnAHeaders.Include(c => c.Project).Include(c => c.QnADetails).Where(x => x.ProjectId == ProjectId).FirstOrDefault();
        }

        public bool UpdateData(QnAHeader data)
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
            return db.QnAHeaders.Max(x => x.Id);
        }
    }

}