namespace DMSRAG.Web.Helpers
{
    public class DriveHelper
    {
        public static string GetSize(double FileSize)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            
            int order = 0;
            while (FileSize >= 1024 && order < sizes.Length - 1)
            {
                order++;
                FileSize = FileSize / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", FileSize, sizes[order]);
            return result;
        }
    }
}
