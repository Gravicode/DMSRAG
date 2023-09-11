using DMSRAG.Models;
using Microsoft.EntityFrameworkCore;
using DMSRAG.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMSRAG.Web.Data
{
    public class FormulaDataService : ICrudDb<FormulaData>
    {
        DMSRAGDb db;

        public FormulaDataService()
        {
            if (db == null) db = new DMSRAGDb();

        }
        public bool DeleteData(object Id)
        {
            var selData = (db.FormulaDatas.Where(x => x.Id == (long)Id).FirstOrDefault());
            db.FormulaDatas.Remove(selData);
            db.SaveChanges();
            return true;
        }

        public List<FormulaData> FindByKeyword(string Keyword)
        {
            var data = from x in db.FormulaDatas
                       where x.FormulaName.Contains(Keyword)
                       select x;
            return data.ToList();
        }

        public List<FormulaData> GetAllData()
        {
            return db.FormulaDatas.ToList();
        }
        public List<FormulaData> GetAllData(string username)
        {
            return db.FormulaDatas.Where(x => x.Username == username).ToList();
        }
        public FormulaData GetDataById(object Id)
        {
            return db.FormulaDatas.Where(x => x.Id == (long)Id).FirstOrDefault();
        }


        public bool InsertData(FormulaData data)
        {
            try
            {
                db.FormulaDatas.Add(data);
                db.SaveChanges();
                return true;
            }
            catch
            {

            }
            return false;

        }

        public bool UpdateData(FormulaData data)
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
            return db.FormulaDatas.Max(x => x.Id);
        }
    }

}