using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMSRAG.Web.Data
{
    public class AppConstants
    {
       
       
        public const int TokenLimit = 4000;
        public static string OpenAIApiKey = "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";//"-- Open AI Key --";
        public static string OrgID = "org-m46V56Jp52g69q9c1h53fv1o";//"-- ORG ID --";
        public static string Model = "text-davinci-003";
        public static string Type = "openai";
        public static (string model, string apiKey, string orgId) GetSettings()
        {
            LoadSettings();
            return (Model, OpenAIApiKey, OrgID);
        }
        public static void SaveSetting()
        {
            Preferences.Set("OpenAIApiKey", OpenAIApiKey);
            Preferences.Set("OrgID", OrgID);
            Preferences.Set("Model", Model);

        }

        public static void LoadSettings()
        {
            Model = Preferences.Get("Model", Model);
            OrgID = Preferences.Get("OrgID", OrgID);
            OpenAIApiKey = Preferences.Get("OpenAIApiKey", OpenAIApiKey);
        }

        public static string StorageEndpoint = "";
        public static string StorageAccess = "";
        public static string StorageSecret = "";
        public static string StorageBucket = "";

        public static long MaxAllowedFileSize = 10*1024000;
        public static long DefaultStorageSize = 21474836480;

        public static string UploadUrlPrefix = "https://storagemurahaje.blob.core.windows.net/FileGue";
        public const string AppName = "FileGue";
    

        public static string ProxyIP = "";
        public static string SQLConn = "";
        public static string EmailAccount = "FileGue@gmail.com";
        public static string RedisCon { set; get; }
        public static string RedisPassword { set; get; }

        public static string GMapApiKey { get; set; }
        public static string BlobConn { get; set; }
        public static string? DefaultPass { get; set; }
    
        
    }
}
