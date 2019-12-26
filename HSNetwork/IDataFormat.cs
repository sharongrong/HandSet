using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSNetwork
{
    interface IDataFormat
    {
        string ToJson(object obj);

        T StrToObj<T>(string res);

        string GetApplication();
    }
}
