using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace HSLog
{
    public class FileTool
    {
        public void WriteMsg(string msg, LogLevel level)
        {
            try
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase.ToString()) + "\\Log";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                string path = dir + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (File.Exists(path))
                {
                    Write(path, msg, level);
                }
                else
                {
                    File.Create(path);
                    Write(path, msg, level);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private string GetHeadMsg(LogLevel level)
        {
            if (level == LogLevel.Null)
            {
                return "";
            }

            return DateTime.Now.ToString("[HH:mm:ss fff]    ") + "[" + level.ToString() + "]:";
        }

        //private string GetLevelStr(LogLevel level)
        //{
        //    switch (level)
        //    {
        //        case LogLevel.Null:
        //            return "";
        //        case LogLevel.Info:
        //            return "Info";
        //        case LogLevel.Debug:
        //            return "Debug";
        //        case LogLevel.Error:
        //            return "Error";
        //        default:
        //            break;
        //    }
        //    return "";
        //}

        private void Write(string path, string msg, LogLevel level)
        {
            StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8);
            string info = GetHeadMsg(level) + msg;
            sw.WriteLine(info);
            sw.Close();
        }
    }
}
