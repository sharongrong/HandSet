using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HSLog
{
    public class HSLog : ILog
    {
        FileTool _file;

        #region ILog 成员

        public void Init()
        {
            if (_file == null)
            {
                _file = new FileTool();
            }
            string s = "-------------------------------------开始--------------------------------------";
            AsyncWrite(s, LogLevel.Null);
        }

        public void Debug(string msg)
        {
            AsyncWrite(msg, LogLevel.Debug); ;
        }

        public void Error(string msg)
        {
            AsyncWrite(msg,LogLevel.Error);
        }

        public void Error(Exception ex)
        {
            AsyncWrite(ex.Message + "  " + ex.StackTrace, LogLevel.Error);
        }

        private void AsyncWrite(string msg,LogLevel level)
        {
            //Action<string,LogLevel> async = Write;
            //async.BeginInvoke(msg, level, null, null);
            Write(msg, level);
        }

        private void Write(string msg,LogLevel level)
        {
            _file.WriteMsg(msg, level);
        }

        public void Dispose()
        {
            Write("-------------------------------------结束--------------------------------------", LogLevel.Null);
        }

        #endregion
    }
}
