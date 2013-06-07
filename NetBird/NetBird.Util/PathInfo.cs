using System;
using System.Collections.Generic;
using System.Text;

namespace NetBird.Util
{
    public class PathInfo
    {
        public static string GetInstallPath
        {
            get
            {
                return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }

        public static string GetSysLogDir
        {
            get
            {
                string ret = string.Format("{0}{1}", GetInstallPath, "log\\");
                CheckPath(ret);
                return ret;
            }
        }

        public static string GetErrorDir
        {
            get
            {
                string ret = string.Format("{0}{1}", GetInstallPath, "errorlog\\");
                CheckPath(ret);
                return ret;
            }
        }

        public static string GetUserDir()
        {
            string ret = string.Format("{0}{1}{2}\\", GetInstallPath, "UserDate\\");
            CheckPath(ret);
            return ret;
        }

        public static string GetUserTempDir()
        {
            string ret = string.Format("{0}{1}", GetInstallPath, "Temp\\");
            CheckPath(ret);
            return ret;
        }

        public static string GetUserAttachmentDir()
        {
            string ret = string.Format("{0}{1}", GetInstallPath, "Attachment\\");
            CheckPath(ret);
            return ret;
        }

        public static string GetUserLogDir()
        {
            string ret = string.Format("{0}{1}", GetSysLogDir, "log\\");
            CheckPath(ret);
            return ret;
        }

        public static string GetUserErrorLogDir()
        {
            string ret = string.Format("{0}{1}", GetSysLogDir, "log\\UserErrorLog\\");
            CheckPath(ret);
            return ret;
        }

        private static void CheckPath(string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
        }
    }
}
