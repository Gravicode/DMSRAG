using DMSRAG.Models;
using Microsoft.EntityFrameworkCore;
using DMSRAG.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMSRAG.Web.Data
{
    public class QnADetailService : ICrudDb<QnADetail>
    {
        DMSRAGDb db;

        public QnADetailService()
        {
            if (db == null) db = new DMSRAGDb();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.QnADetails.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.QnADetails.Remove(selData);
            db.SaveChanges();
            return true;
        } 
        public bool DeleteData(IEnumerable<QnADetail> details)
        {
            foreach(var selData in details)
                db.QnADetails.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<QnADetail> FindByKeyword(string Keyword)
        {
            var data = from x in db.QnADetails
                       where x.Question.Contains(Keyword) || x.Answer.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<QnADetail> GetAllData()
        {
            return db.QnADetails.ToList();
        }
        public List<QnADetail> GetAllData(long QnAHeaderId)
        {
            return db.QnADetails.Include(c=>c.QnAHeader).Where(x => x.QnAHeaderId == QnAHeaderId).ToList();
        }
        public QnADetail GetDataById(object Id)
        {
            return db.QnADetails.Include(c => c.QnAHeader).Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(QnADetail data)
        {
            try
            {
                db.QnADetails.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }
        public bool InsertData(IEnumerable< QnADetail> datas)
        {
            try
            {
                foreach(var data in datas)
                    db.QnADetails.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }



        public bool UpdateData(QnADetail data)
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
            return db.QnADetails.Max(x => x.Id);
        }
    }

}