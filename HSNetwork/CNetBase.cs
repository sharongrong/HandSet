using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace HSNetwork
{
    public abstract class CNetBase<T>
    {
        protected int Timeout { get; set; }

        private readonly IProtocal _protocal;

        private readonly IDataFormat _jsonFormat;

        protected string UrlBase { get; set; }

        public static string Error;


        public const string Success = "0000";
        public const string TokenError = "10008";

        protected CNetBase()
        {
            Timeout = 20000;
            //UrlBase = "http://192.168.1.65:8088/api/";//默认域名前缀
            //UrlBase = "http://101.231.106.162:9103/api/";
            UrlBase = "http://tc.jinshajiuye.com/t/api/";
            //UrlBase = "http://192.168.1.143:8088/api/";//默认域名前缀

            _jsonFormat = new CJsonNetwork();

            _protocal = new CHttp()
            {
                Application = _jsonFormat.GetApplication(),
                Timeout = Timeout,
            };

        }

        protected abstract string GetSubUrl();

        protected virtual T GetRsp(object obj)
        {
            return GetRsp(obj, 1);
        
        }

        protected virtual T GetRsp(object obj, int repeat)
        {
            string reqStr = _jsonFormat.ToJson(obj);
            HSLog.LogSys.Debug(UrlBase + GetSubUrl() + "  " + reqStr);
            string rspStr = _protocal.RevResponse(reqStr, UrlBase + GetSubUrl(), repeat);
            HSLog.LogSys.Debug(UrlBase + GetSubUrl() + "  "+ rspStr);
            return _jsonFormat.StrToObj<T>(rspStr);
        }

        public string GetErrorString()
        {
            Error = _protocal.GetError();
            HSLog.LogSys.Error(Error);
            return Error;
        }       
    }
}
