namespace DMSRAG.Web.Data
{
    public interface ICrudDb<T> where T : class
    {
        bool InsertData(T data);
        bool UpdateData(T data);
        List<T> GetAllData();

        List<T> FindByKeyword(string Keyword);
        T GetDataById(object Id);
        bool DeleteData(object Id);
        long GetLastId();
    }
}
