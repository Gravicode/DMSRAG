using Redis.OM;
using DMSRAG.Models;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System.Text;
using DMSRAG.Web.Helpers;
using ServiceStack.Model;

namespace DMSRAG.Web.Data
{
    public class DriveService
    {
        public Drive MyDrive { set; get; }
        IRedisTypedClient<Drive> db;
        public string Username { get; set; }
        public string KeyStr { get; set; }
        public bool IsInit { get; set; } = false;
        public DriveService(PooledRedisClientManager redisManager)
        {
            //var con = !string.IsNullOrEmpty(AppConstants.RedisPassword) ? $"{AppConstants.RedisPassword}@{AppConstants.RedisCon}":AppConstants.RedisCon;
            //using var redisManager = new PooledRedisClientManager(con);
            using var redis = redisManager.GetClient();
            db = redis.As<Drive>();
            IsInit = false;
        }

        public void InitDrive(string Username)
        {
            this.Username = Username;
            Refresh();
            if (MyDrive == null)
            {
                //init with first drive
                MyDrive = new() { CreatedDate = DateHelper.GetLocalTimeNow(), UserName = Username, RootFolder = new DriveFolder() { CreatedDate = DateHelper.GetLocalTimeNow(), Favorite = true, Files = new(), Folders = new() { new DriveFolder() { CreatedDate = DateHelper.GetLocalTimeNow(), UpdatedDate = DateHelper.GetLocalTimeNow(), Favorite=true, Files = new(), Folders= new(), IsRoot=false, Name = "My Files", Path = "/MyFiles", Size=0, UID = UIDHelper.CreateNewUID()  } }, IsRoot = true, Name = "My Documents",  Path = "/", Size=0, UID = UIDHelper.CreateNewUID(), UpdatedDate = DateHelper.GetLocalTimeNow() } };
                Save();
            }
            IsInit = true;
        }

        public long GetUsedSize()
        {
            if (!IsInit) return -1;
            return GetSize(MyDrive.RootFolder);
        }

        public long GetSize(DriveFolder folder)
        {
            long size = 0;
            foreach(var file in folder.Files)
            {
                size += file.Size;
            }
            foreach(var subfolder in folder.Folders)
            {
                if(subfolder.Files.Count>0)
                    size += GetSize(subfolder);
            }
            return size;
        }

        public void Refresh()
        {
            this.KeyStr = $"Drive:{Username}";
            MyDrive = db.GetValue(KeyStr);
        }
        public void Save()
        {
            db.SetValue(KeyStr, MyDrive);
        }
        public DriveFolder GetFolder(string FolderUid)
        {
            var find = TraceFolder(MyDrive.RootFolder, FolderUid);
            return find;
        }

        public List<DriveFile> GetRecentFiles(int Limit=10)
        {
            try
            {
                var files = TraceFiles(MyDrive.RootFolder);
                return files.OrderByDescending(x=>x.CreatedDate).Take(Limit).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new List<DriveFile>();
        }
        
        public List<DriveFolder> GetRecentFolders(int Limit=10)
        {
            try
            {
                var folders = TraceFolders(MyDrive.RootFolder);
                return folders.OrderByDescending(x=>x.CreatedDate).Take(Limit).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new List<DriveFolder>();
        }

        public List<DriveFile> GetAllFiles()
        {
            try
            {
                var files = TraceFiles(MyDrive.RootFolder);
                return files;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new List<DriveFile>();
        } 
        
        public List<DriveFile> GetDeletedFiles()
        {
            try
            {
                var files = TraceDeletedFiles(MyDrive.RootFolder);
                return files;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new List<DriveFile>();
        } 
        
        public List<DriveFile> GetFavoriteFiles()
        {
            try
            {
                var files = TraceFiles(MyDrive.RootFolder,true);
                return files;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return new List<DriveFile>();
        }
        List<DriveFolder> TraceFolders(DriveFolder folder)
        {
            var folders = new List<DriveFolder>();
            
            foreach (var subfolder in folder.Folders)
            {
                if (!subfolder.IsDeleted )
                {
                    folders.Add(subfolder);
                    var adds = TraceFolders(subfolder);
                    if (adds.Count > 0)
                        folders.AddRange(adds);
                }
            }
            return folders;
        }

        public DriveFolder GetFolderByUid(string FolderUid)
        {
            var find = TraceFolderByUID(MyDrive.RootFolder,FolderUid);
            return find;
        } 
        public DriveFile GetFileByUid(string FileUid)
        {
            var find = TraceFileByUID(MyDrive.RootFolder,FileUid);
            return find;
        }

        DriveFolder TraceFolderByUID(DriveFolder folder, string FolderUid)
        {
            if (folder.UID == FolderUid) return folder;
            foreach (var subfolder in folder.Folders)
            {
                var find = TraceFolderByUID(subfolder,FolderUid);
                if (find != null) return find;
            }
            return default;
        } 
        
        DriveFile TraceFileByUID(DriveFolder folder, string FileUid)
        {
            foreach(var file in folder.Files)
            {
                if (file.UID == FileUid) return file;
            }
            foreach (var subfolder in folder.Folders)
            {
                var find = TraceFileByUID(subfolder,FileUid);
                if (find != null) return find;
            }
            return default;
        }

        List<DriveFolder> TraceFolders(DriveFolder folder, bool Favorite)
        {
            var folders = new List<DriveFolder>();
            
            foreach (var subfolder in folder.Folders)
            {
                if (!subfolder.IsDeleted && subfolder.Favorite == Favorite)
                {
                    folders.Add(subfolder);
                    var adds = TraceFolders(subfolder,Favorite);
                    if (adds.Count > 0)
                        folders.AddRange(adds);
                }
            }
            return folders;
        }
        List<DriveFile> TraceFiles(DriveFolder folder,bool Favorite)
        {
            var files = new List<DriveFile>();
            foreach (var file in folder.Files)
            {
                if(file.Favorite == Favorite && !file.IsDeleted)
                    files.Add(file);
            }
            foreach (var subfolder in folder.Folders)
            {
                var adds = TraceFiles(subfolder,Favorite);
                if (adds.Count > 0)
                    files.AddRange(adds);
            }
            return files;
        }
        List<DriveFile> TraceFiles(DriveFolder folder)
        {
            var files = new List<DriveFile>();
            foreach(var file in folder.Files)
            {
                if(!file.IsDeleted)
                    files.Add(file);
            }
            foreach(var subfolder in folder.Folders)
            {
                var adds = TraceFiles(subfolder);
                if (adds.Count > 0)
                    files.AddRange(adds);
            }
            return files;
        }

        List<DriveFile> TraceDeletedFiles(DriveFolder folder)
        {
            var files = new List<DriveFile>();
            foreach (var file in folder.Files)
            {
                if (file.IsDeleted)
                    files.Add(file);
            }
            foreach (var subfolder in folder.Folders)
            {
                var adds = TraceDeletedFiles(subfolder);
                if (adds.Count > 0)
                    files.AddRange(adds);
            }
            return files;
        }

        DriveFolder TraceFolder(DriveFolder folder,string FolderUid)
        {
            if (folder.UID == FolderUid) return folder;
            foreach(var subfolder in folder.Folders)
            {
                var find = TraceFolder(subfolder, FolderUid);
                if (find != null) return find;
            }
            return null;
        }
        public bool DeleteFolder(DriveFolder Folder, string FolderUid)
        {
            var item = Folder.Folders.FirstOrDefault(x => x.UID == FolderUid);
            if (item != null)
            {
                return Folder.Folders.Remove(item);
            }
            return false;
        }

        public bool DeleteFile(DriveFolder Folder, string FileUid)
        {
            var item = Folder.Files.FirstOrDefault(x => x.UID == FileUid);
            if (item != null)
            {
                var res = Folder.Files.Remove(item);
                if (res)
                {
                    Save();
                    return res;
                }
               
            }
            return false;
        }
        public List<DriveFile> FindFiles(DriveFolder folder, string Keyword, string Extension)
        {
            if (!IsInit) return default;
            
            var files = SearchFiles(folder, Keyword, Extension);
            return files;
        }
        public List<DriveFile> SearchFiles(DriveFolder currentFolder, string Keyword, string Extension = "")
        {
            var files = new List<DriveFile>();
            var query = string.IsNullOrEmpty(Keyword) ? currentFolder.Files :  currentFolder.Files.Where(x => x.Name.Contains(Keyword));
            if (!string.IsNullOrEmpty(Extension))
            {
                query = query.Where(x => Extension.Contains(x.Extension));
            }
            files = query.ToList();

            foreach (var folder in currentFolder.Folders)
            {
                var datas = SearchFiles(folder, Keyword, Extension);
                if (datas != null)
                    files.AddRange(datas);
            }

            return files;
        }
        public List<DriveFile> FindItem(DriveFile file)
        {
            if (!IsInit) return default;

            var files = SearchFile(MyDrive.RootFolder, file);
            return files;
        }

        public List<DriveFolder> FindItem(DriveFolder folder)
        {
            if (!IsInit) return default;

            var folders = SearchFolder(MyDrive.RootFolder, folder);
            return folders;
        }

        List<DriveFile> SearchFile(DriveFolder currentFolder, DriveFile findFile)
        {
            var files = new List<DriveFile>();
            foreach (var currentFile in currentFolder.Files)
            {
                if (currentFile == findFile)
                {
                    files.Add(currentFile);
                    return files;
                }
            }
            if (files.Count <= 0)
            {
                foreach (var folder in currentFolder.Folders)
                {
                    return SearchFile(folder, findFile);
                }
            }
            return files;
        }

        List<DriveFolder> SearchFolder(DriveFolder currentFolder, DriveFolder findFolder)
        {
            var folders = new List<DriveFolder>();

            foreach (var folder in currentFolder.Folders)
            {
                if (folder == findFolder)
                {
                    folders.Add(folder);
                    return folders;
                }
                return SearchFolder(folder, findFolder);
            }

            return folders;
        }


    }

}
