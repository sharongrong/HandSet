using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace HSDbOrm
{
    public interface IDbOperator
    {
        void Close();

        DataTable QuerySql(string sSql);

        DataTable QueryTable(string sql);

        bool ExecuteSql(string strSql);

        bool ExecuteSql(string strSql, object[] parameters);

        int ExecuteNonSql(string strSql);

        int ExecuteNonSql(string strSql, object[] parameters);

        object GetObject(string strSql);

        string GetString(string strSql);

        List<string> GetStrList(string sql);

        int ExecuteSql(List<string> strSqls);

        int ExecuteMutliQuery(string commandText, string[][] dtData);

        bool bConnected { get; }
    }
}
