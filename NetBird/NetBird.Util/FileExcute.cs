using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NetBird.Util
{
    public class FileExcute
    {
        public static string GetPostfixStr(string filename)
        {
            int start = filename.LastIndexOf(".");
            int length = filename.Length;
            string postfix = filename.Substring(start, length - start);
            return postfix;

        }

        public static void WriteFile(string Path, string Strings)
        {
            if (!System.IO.File.Exists(Path))
            {
                System.IO.FileStream f = System.IO.File.Create(Path);
                f.Close();
            }
            System.IO.StreamWriter f2 = new System.IO.StreamWriter(Path, false, System.Text.Encoding.UTF8);
            f2.Write(Strings);
            f2.Close();
            f2.Dispose();
        }

        public static string ReadFile(string Path)
        {
            string s = "";
            if (!System.IO.File.Exists(Path))
                s = "Corresponding directory does not exist";
            else
            {
                StreamReader f2 = new StreamReader(Path, System.Text.Encoding.UTF8);
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }
            return s;
        }

        public static void FileAdd(string Path, string strings)
        {
            File.AppendAllText(Path, strings);
        }

        public static void FileCopy(string orignFile, string NewFile)
        {
            File.Copy(orignFile, NewFile, true);
        }

        public static void FileDel(string Path)
        {
            File.Delete(Path);
        }

        public static void FileMove(string orignFile, string NewFile)
        {
            File.Move(orignFile, NewFile);
        }

        public static void FolderCreate(string NewFloder)
        {
            if (!Directory.Exists(NewFloder))
                Directory.CreateDirectory(NewFloder);
        }

        public static void FolderCreate(string orignFolder, string NewFloder)
        {
            Directory.SetCurrentDirectory(orignFolder);
            Directory.CreateDirectory(NewFloder);
        }

        public static void DeleteFolder(string dir)
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }

        public static void CleanFolder(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                        File.Delete(d);
                }
            }
        }

        public static void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);
                string[] fileList = Directory.GetFileSystemEntries(srcPath);

                foreach (string file in fileList)
                {
                    if (Directory.Exists(file))
                        CopyDir(file, aimPath + Path.GetFileName(file));
                    else
                        File.Copy(file, aimPath + Path.GetFileName(file), true);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }
        }

        public static bool IsMediaFile(FileInfo fileInfo)
        {
            if (string.Equals(fileInfo.Extension, ".bmp", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".gif", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".jpe", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".jpeg", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".jpg", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".png", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".tif", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".asf", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".asx", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".avi", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".mov", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".mp4", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".mpeg", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".mpg", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".wmv", StringComparison.InvariantCultureIgnoreCase)) { return true; }

            return false;
        }

        public static bool IsWebImageFile(FileInfo fileInfo)
        {
            if (string.Equals(fileInfo.Extension, ".gif", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".jpeg", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".jpg", StringComparison.InvariantCultureIgnoreCase)) { return true; }
            if (string.Equals(fileInfo.Extension, ".png", StringComparison.InvariantCultureIgnoreCase)) { return true; }

            return false;
        }

        public static bool IsDecendentDirectory(DirectoryInfo directoryToSearch, DirectoryInfo directoryToFind)
        {
            if (directoryToFind.FullName.StartsWith(directoryToSearch.FullName)) { return true; }

            return false;
        }

        public static bool IsDecendentDirectory(DirectoryInfo directoryToSearch, FileInfo fileToFind)
        {
            if (fileToFind.FullName.StartsWith(directoryToSearch.FullName)) { return true; }

            return false;
        }

        public static string GetMimeType(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension)) { return string.Empty; }

            string fileType = fileExtension.ToLower().Replace(".", string.Empty);

            switch (fileType)
            {
                case "doc":
                case "docx":
                    return "application/msword";

                case "xls":
                case "xlsx":
                    return "application/vnd.ms-excel";

                case "exe":
                    return "application/octet-stream";

                case "ppt":
                case "pptx":
                    return "application/vnd.ms-powerpoint";

                case "jpg":
                case "jpeg":
                    return "image/jpeg";

                case "gif":
                    return "image/gif";

                case "png":
                    return "image/png";

                case "bmp":
                    return "image/bmp";

                case "wmv":
                    return "video/x-ms-wmv";

                case "mpg":
                case "mpeg":
                    return "video/mpeg";

                case "mov":
                    return "video/quicktime";

                case "flv":
                    return "video/x-flv";

                case "ico":
                    return "image/x-icon";

                case "htm":
                case "html":
                    return "text/html";

                case "css":
                    return "text/css";

                case "eps":
                    return "application/postscript";

            }

            return "application/" + fileType;
        }

        public static bool IsNonAttacmentFileType(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension)) { return false; }

            string fileType = fileExtension.ToLower().Replace(".", string.Empty);
            if (fileType == "pdf") { return true; }
            return false;
        }

        public static bool HasFolder(string parentPath,string folderName)
        {
           return Directory.Exists(string.Format("{0}\\{1}",parentPath,folderName));
        }
    }
}
