using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSNetwork
{
    public interface IProtocal
    {
        string RevResponse(string req, string url);

        string RevResponse(string req, string url, int repeat);

        string GetError();

        bool IsConnected();
    }
}
