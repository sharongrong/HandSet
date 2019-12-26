using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HSNetwork
{
    public class CJsonNetwork : IDataFormat
    {

        #region IDataFormat 成员

        public string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T StrToObj<T>(string res)
        {
            if (res.StartsWith("错误"))
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(res);
        }

        public string GetApplication()
        {
            return "application/json";
        }

        #endregion
    }
}
