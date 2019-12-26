using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace HSLog
{
    public static class LogSys
    {
       private static ILog _log;

       private static ILog Log
       {
           get
           {
               if (_log == null)
               {
                   _log = new HSLog();
                   _log.Init();
               }
               return _log;
           }
       }

       public static void Init()
       {
           if (_log == null)
           {
               _log = new HSLog();
               _log.Init();
           }
       }

       public static void Debug(string msg)
       {
           Log.Debug(msg);
       }

       public static void Error(string msg)
       {
           Log.Error(msg);
       }

       public static void Error(Exception ex)
       {
           Log.Error(ex);
       }

       public static void Dispose()
       {
           Log.Dispose();
       }
    }
}
