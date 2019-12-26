using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace HSNetwork
{
    internal class CHttp : IProtocal
    {
        public int Timeout { get; set; }

        protected static string Error;

        protected bool BNetwork = true;

        public string Application { set; get; }


        #region IProtocal 成员

        public string RevResponse(string postDataStr, string url)
        {
            try
            {
                BNetwork = true;
                Error = "";
                GC.Collect();

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = Application;
                req.Timeout = Timeout;
                req.ReadWriteTimeout = Timeout;
                req.SendChunked = true;
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 200;
                req.KeepAlive = false;
                req.ProtocolVersion = HttpVersion.Version11;
                Encoding endcoding = Encoding.UTF8;
                byte[] postData = endcoding.GetBytes(postDataStr);
                req.ContentLength = postData.Length;
                Stream stream = req.GetRequestStream();
                stream.Write(postData, 0, postData.Length);
                stream.Close();

                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
                Stream st = rsp.GetResponseStream();
                if (st == null)
                {
                    Error = "错误:网络异常";
                    BNetwork = false;
                    return Error;
                }

                StreamReader sr = new StreamReader(st, Encoding.GetEncoding("utf-8"));
                string srStr = sr.ReadToEnd();
                st.Close();
                sr.Close();
                return srStr;
            }
            catch (Exception ex)
            {
                BNetwork = false;
                Error = "错误:网络异常";
                HSLog.LogSys.Error(ex.Message + "   " + ex.StackTrace);
                return Error;
            }
        }

        public string RevResponse(string req, string url, int repeat)
        {
            string res = "";
            for (int i = 0; i < repeat; i++)
            {
                res = RevResponse(req, url);
                if (res != "")
                {
                    return res;
                }
            }
            return res;
        }

        public string GetError()
        {
            return Error;
        }

        public bool IsConnected()
        {
            return BNetwork;
        }

        #endregion
    }
}
