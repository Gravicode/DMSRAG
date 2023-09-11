using DMSRAG.Web.Helpers;

namespace DMSRAG.Web.Data
{
    public class AppState
    {

        public event Action<string> OnProfileChange;
        public event Action<string> OnDriveChange;
        public event Action<string> OnStorageChange;
        public event Action<string> OnFolderChange;

        public event Action<string, GeoLocation> OnLocationChange;


        public void RefreshProfile(string username)
        {
            ProfileStateChanged(username);
        }

        public void RefreshDrive(string username)
        {
            DriveStateChanged(username);
        }
        public void RefreshStorage(string username)
        {
            StorageStateChanged(username);
        }
        public void RefreshFolder(string UID)
        {
            FolderStateChanged(UID);
        }


        public void SelectLocation(string username, GeoLocation loc)
        {
            LocationStateChanged(username, loc);
        }


        private void FolderStateChanged(string UID) => OnFolderChange?.Invoke(UID);
        private void StorageStateChanged(string username) => OnStorageChange?.Invoke(username);
        private void DriveStateChanged(string username) => OnDriveChange?.Invoke(username);
        private void ProfileStateChanged(string username) => OnProfileChange?.Invoke(username);

        private void LocationStateChanged(string username, GeoLocation loc) => OnLocationChange?.Invoke(username, loc);

    }
}
