using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSLog
{
    interface ILog
    {
        void Debug(string msg);

        void Error(string msg);

        void Error(Exception ex);

        void Init();

        void Dispose();
    }

    public enum LogLevel
    {
        Null = 0,
        Info = 1,
        Debug = 2,
        Error = 3,
    }
}
