using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NetBird.Util
{
    public class FileOperator
    {
        public static string FileReader(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Empty path");
            if (!File.Exists(filePath))
                throw new Exception("Wrong path");

            return File.ReadAllText(filePath);
        }

        public static void FileWriter(string filePath,byte[] content)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Empty path");
            if (!File.Exists(filePath))
                throw new Exception("Wrong path");

            FileStream fs = File.OpenWrite(filePath);
            fs.Write(content, 0, content.Length);
            fs.Close();
        }

        public static DateTime FileDate(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Empty path");

            return File.GetLastWriteTime(filePath);
        }
    }
}
