using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMSRAG.Web.Data
{
    public class AppConstants
    {
       
        public const int TokenLimit = 4000;
        public static string OpenAIApiKey = "";//"-- Open AI Key --";
        public static string OrgID = "";//"-- ORG ID --";
        public static string Model = "";
        public static string Type = "openai";
        public static (string model, string apiKey, string orgId) GetSettings()
        {  
            return (Model, OpenAIApiKey, OrgID);
        }
       

        public static string StorageEndpoint = "";
        public static string StorageAccess = "";
        public static string StorageSecret = "";
        public static string StorageBucket = "";

        public static long MaxAllowedFileSize = 10*1024000;
        public static long DefaultStorageSize = 21474836480;

        public static string UploadUrlPrefix = "https://storagemurahaje.blob.core.windows.net/FileGue";
        public const string AppName = "DMS-RAG";
    

        public static string ProxyIP = "";
        public static string SQLConn = "";
        public static string EmailAccount = "dmsrag@gmail.com";
        public static string RedisCon { set; get; }
        public static string RedisPassword { set; get; }

        public static string GMapApiKey { get; set; }
        public static string BlobConn { get; set; }
        public static string? DefaultPass { get; set; }
    
        
    }
}
