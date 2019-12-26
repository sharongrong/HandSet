using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Data.SQLite;

namespace HSDbOrm
{
    public class CSqliteBase : IDbOperator
    {
        //protected TSunLog Log = CLog.GetInstance();
        private const string Password = "techsun";
        protected SQLiteHelper SqlHelper;

        public bool bConnected
        {
            get
            {
                return SqlHelper.Conn.State == ConnectionState.Open;
            }
        }

        public CSqliteBase(string dbName)
        {
            this.SqlHelper = new SQLiteHelper(dbName, "techsun");
            if (File.Exists(dbName))
                return;
            this.SqlHelper.CreateDB();
        }

        public void Close()
        {
            this.SqlHelper.CloseDB();
        }

        public DataTable QuerySql(string sSql)
        {
            //this.Log.Debug(sSql);
            return this.SqlHelper.ExecuteDataSet(sSql, (object[])null).Tables[0];
        }

        public DataTable QueryTable(string sql)
        {
            return this.QuerySql(sql);
        }

        public bool ExecuteSql(string strSql)
        {
            return this.ExecuteNonSql(strSql, (object[])null) > 0;
        }

        public bool ExecuteSql(string strSql, object[] parameters)
        {
            return this.ExecuteNonSql(strSql, parameters) > 0;
        }

        public int ExecuteNonSql(string strSql)
        {
            return this.ExecuteNonSql(strSql, (object[])null);
        }

        public int ExecuteNonSql(string strSql, object[] parameters)
        {
            //this.Log.Debug(strSql);
            int num = this.SqlHelper.ExecuteNonQuery(strSql, parameters);
            //this.Log.Debug(string.Concat((object)num));
            return num;
        }

        public object GetObject(string strSql)
        {
            //this.Log.Debug(strSql);
            object obj = this.SqlHelper.ExecuteScalar(strSql, (object[])null);
            //this.Log.Debug(obj.ToString());
            return obj;
        }

        public string GetString(string strSql)
        {
            string sText = this.GetObject(strSql).ToString();
            //this.Log.Debug(sText);
            return sText;
        }

        public List<string> GetStrList(string sql)
        {
            //this.Log.Debug(sql);
            List<string> stringList = new List<string>();
            IDataReader dataReader = this.SqlHelper.ExecuteReader(this.SqlHelper.CreateCommand(sql, (SQLiteParameter[])null), sql, (object[])null);
            while (dataReader.Read())
            {
                string sText = dataReader.GetString(0);
                //this.Log.Debug(sText);
                stringList.Add(sText);
            }
            dataReader.Close();
            return stringList;
        }

        public int ExecuteSql(List<string> strSqls)
        {
            //foreach (string strSql in strSqls)
                //this.Log.Debug(strSql);
            int num = this.SqlHelper.ExecuteMutliQuery(strSqls.ToArray());
            //this.Log.Debug(string.Concat((object)num));
            return num;
        }

        public int ExecuteMutliQuery(string commandText, string[][] dtData)
        {
            //this.Log.Debug(commandText);
            int num = this.SqlHelper.ExecuteMutliQuery(commandText, dtData);
            //this.Log.Debug(string.Concat((object)num));
            return num;
        }
    }
}
