namespace DMSRAG.Web.Data
{
    public static class Preferences
    {
        static Dictionary<string, string> DataValues = new();
        
        public static void Set(string Key,string ValueStr)
        {
            if (DataValues.ContainsKey(Key))
            {
                DataValues[Key] = ValueStr;
            }else
                DataValues.Add(Key, ValueStr);
        }
        public static string Get(string Key,string DefaultValueStr)
        {
            if(DataValues.ContainsKey(Key))
                return DataValues[Key];
            return DefaultValueStr;
        }
    }
}
