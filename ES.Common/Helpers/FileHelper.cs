using Microsoft.Win32;

namespace ES.Common.Helper
{
    public class FileHelper
    {
        public static string OpenFile(string title = "Open File", string filter = "All files |*.*")
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = title,
                Filter = filter
                
            };
            openFileDialog.ShowDialog();
            return openFileDialog.FileName;
        }
        public static string SaveFile(string title = "Save file", string filter = "All files |*.*")
        {
            var fileDialog = new SaveFileDialog()
            {
                Title = title,
                Filter = filter
            };
            return (fileDialog.ShowDialog() == true) ? fileDialog.FileName : string.Empty;
        }
    }
}
