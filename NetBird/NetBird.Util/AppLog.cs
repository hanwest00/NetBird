using System;
using System.Collections.Generic;
using System.Text;

namespace NetBird.Util
{
    public class AppLog
    {
        private static string _Separated = "\r\n-----------------------\r\n";
        /// <summary>
        /// 获取或设置每条log之间的分隔字符串
        /// </summary>
        public static string Separated
        {
            get { return _Separated; }
            set { _Separated = value; }
        }

        public static void CreateLog(string path, string logContent)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (string.IsNullOrEmpty(logContent))
                throw new ArgumentNullException("logContent");
            try
            {
                FileExcute.FolderCreate(PathInfo.GetErrorDir);
                FileExcute.FolderCreate(PathInfo.GetSysLogDir);
                if (!logContent.EndsWith("\r\n"))
                    logContent = string.Format("{0}{1}{2}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss,"), logContent, Separated);
                FileExcute.FileAdd(path, logContent);
            }
            catch (Exception ex)
            {
                FileExcute.FileAdd(PathInfo.GetErrorDir + "logerror.log", ex.ToString());
                throw ex;
            }
        }

        public static void SysLog(string logContent)
        {
            CreateLog(string.Format("{0}{1}{2}", PathInfo.GetSysLogDir, DateTime.Now.ToString("yyyy-MM-dd"), ".log"), logContent);
        }

        public static void UserLog(string EmailAddress, string logContent)
        {
            CreateLog(string.Format("{0}{1}_{2}{3}", PathInfo.GetUserLogDir(), EmailAddress, DateTime.Now.ToString("yyyy-MM-dd"), ".log"), logContent);
        }

        public static void UserErrorLog(string EmailAddress, string logContent)
        {
            CreateLog(string.Format("{0}{1}_{2}{3}", PathInfo.GetUserErrorLogDir(), EmailAddress, DateTime.Now.ToString("yyyy-MM-dd"), ".log"), logContent);
        }

        public static IList<string> GetLogString(DateTime date)
        {
            return GetLogString(LogType.SysLog, date, "");
        }

        public static IList<string> GetLogString(LogType logType, DateTime date, string EmailAddress)
        {
            IList<string> retList = new List<string>();
            switch (logType)
            {
                case LogType.SysLog:
                    _GetLogString(retList, FileExcute.ReadFile(string.Format("{0}{1}{3}", PathInfo.GetSysLogDir, date.ToString("yyyy-MM-dd"), ".log")));
                    break;
                case LogType.ErrorLog:
                    _GetLogString(retList, FileExcute.ReadFile(string.Format("{0}{1}_{2}{3}", PathInfo.GetUserErrorLogDir(), EmailAddress, date.ToString("yyyy-MM-dd"), ".log")));
                    break;
                case LogType.UserLog:
                    _GetLogString(retList, FileExcute.ReadFile(string.Format("{0}{1}_{2}{3}", PathInfo.GetUserLogDir(), EmailAddress, date.ToString("yyyy-MM-dd"), ".log")));
                    break;
            }
            return retList;
        }

        private static void _GetLogString(IList<string> arrList, string logString)
        {
            if (logString == "Corresponding directory does not exist")
                arrList = null;
            string[] arrStr = System.Text.RegularExpressions.Regex.Split(logString, Separated);
            foreach (string s in arrStr)
                arrList.Add(s);
        }

        public enum LogType
        {
            SysLog,
            UserLog,
            ErrorLog
        }
    }
}
