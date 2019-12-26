using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;

namespace HSDbOrm
{
    public abstract class CDbOrm<T> where T : new()
    {
        protected IDbOperator SqlHelper;
        protected bool NeedCreateTable;

        protected CDbOrm()
        {
            this.Init();
        }

        public abstract string GetDbFile();

        public abstract string GetTableName();

        public void Close()
        {
            this.SqlHelper.Close();
        }

        protected virtual void Init()
        {
            string dbFile = this.GetDbFile();
            if (dbFile == null)
                return;
            this.SqlHelper = (IDbOperator)new CSqliteBase(dbFile);
            this.CreateTable();
        }

        public bool bConnected
        {
            get
            {
                return SqlHelper.bConnected;
            }
        }

        protected string GetUpdateCondition(List<Columns> cols)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(" where ");
            bool flag = false;
            foreach (Columns col in cols)
            {
                if (col.ColIsId)
                {
                    flag = true;
                    stringBuilder.Append(string.Format("{0}='{1}' and ", (object)col.ColName, col.ColData));
                }
            }
            if (!flag)
            {
                foreach (Columns col in cols)
                {
                    if (col.ColPorperty.Contains("PRIMARY KEY"))
                    {
                        flag = true;
                        stringBuilder.Append(string.Format("{0}='{1}' and ", (object)col.ColName, col.ColData));
                        break;
                    }
                }
            }
            if (flag)
                stringBuilder.Remove(stringBuilder.Length - 5, 4);
            return flag ? stringBuilder.ToString() : "";
        }

        public bool SaveOrUpdate(T obj)
        {
            List<Columns> customAttribute = AttributeHelper.GetCustomAttribute((object)obj);
            string updateCondition = this.GetUpdateCondition(customAttribute);
            if ((object)this.QuerySingle(updateCondition) != null)
                return this.Update(obj, updateCondition, customAttribute);
            return this.Insert(obj);
        }

        public bool Update(T obj, string condition, List<Columns> cols)
        {
            StringBuilder stringBuilder = new StringBuilder();
            object[] objArray = new object[cols.Count];
            int length = 0;
            foreach (Columns col in cols)
            {
                string colName = col.ColName;
                if (!col.ColPorperty.ToUpper().Contains("PRIMARY KEY"))
                {
                    object colData = col.ColData;
                    if (colData != null)
                    {
                        stringBuilder.Append(colName + "=@" + colName + ",");
                        objArray[length] = colData;
                        ++length;
                    }
                }
            }
            object[] parameters = new object[length];
            for (int index = 0; index < length; ++index)
                parameters[index] = objArray[index];
            return this.SqlHelper.ExecuteNonSql(string.Format("update {0} set {1} {2}", (object)this.GetTableName(), (object)stringBuilder.Remove(stringBuilder.Length - 1, 1), (object)condition), parameters) == 1;
        }

        public bool Update(T obj)
        {
            List<Columns> customAttribute = AttributeHelper.GetCustomAttribute((object)obj);
            return this.Update(obj, this.GetUpdateCondition(customAttribute), customAttribute);
        }

        public bool Insert(T obj)
        {
            List<Columns> customAttribute = AttributeHelper.GetCustomAttribute((object)obj);
            StringBuilder stringBuilder1 = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            object[] parameters = new object[customAttribute.Count];
            int num = 0;
            foreach (Columns columns in customAttribute)
            {
                string colName = columns.ColName;
                string colPorperty = columns.ColPorperty;
                stringBuilder1.Append(colName + ",");
                if (colPorperty.ToUpper().Contains("PRIMARY KEY") && colPorperty.ToUpper().Contains("AUTOINCREMENT"))
                {
                    stringBuilder2.Append("null,");
                    object[] objArray = new object[customAttribute.Count - 1];
                    for (int index = 0; index < objArray.Length; ++index)
                        objArray[index] = parameters[index];
                    parameters = objArray;
                }
                else
                {
                    stringBuilder2.Append("@" + colName + ",");
                    object colData = columns.ColData;
                    object obj1 = colData == null ? (object)"" : colData;
                    parameters[num++] = obj1;
                }
            }
            return this.SqlHelper.ExecuteNonSql(string.Format("INSERT INTO {0}({1}) VALUES({2})", (object)this.GetTableName(), (object)stringBuilder1.Remove(stringBuilder1.Length - 1, 1), (object)stringBuilder2.Remove(stringBuilder2.Length - 1, 1)), parameters) == 1;
        }

        public bool InsertBatch(List<T> objs)
        {
            List<Columns> customAttribute = AttributeHelper.GetCustomAttribute((object)objs[0]);
            StringBuilder stringBuilder1 = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            string[,] strArray = new string[objs.Count, customAttribute.Count];
            int index1 = 0;
            int count = customAttribute.Count;
            foreach (Columns columns in customAttribute)
            {
                string colName = columns.ColName;
                string colPorperty = columns.ColPorperty;
                stringBuilder1.Append(colName + ",");
                if (colPorperty.ToUpper().Contains("PRIMARY KEY") && colPorperty.ToUpper().Contains("AUTOINCREMENT"))
                {
                    --count;
                    stringBuilder2.Append("null,");
                }
                else
                {
                    stringBuilder2.Append("@" + colName + ",");
                    for (int index2 = 0; index2 < objs.Count; ++index2)
                    {
                        PropertyInfo property = objs[index2].GetType().GetProperty(colName);
                        if (property != (PropertyInfo)null)
                        {
                            object obj1 = property.GetValue((object)objs[index2], (object[])null);
                            object obj2 = obj1 == null ? (object)"" : (object)obj1.ToString();
                            strArray[index2, index1] = obj2.ToString();
                        }
                        else
                            strArray[index2, index1] = "";
                    }
                    ++index1;
                }
            }
            string commandText = string.Format("INSERT INTO {0}({1}) VALUES({2})", (object)this.GetTableName(), (object)stringBuilder1.Remove(stringBuilder1.Length - 1, 1), (object)stringBuilder2.Remove(stringBuilder2.Length - 1, 1));
            string[][] dtData = new string[objs.Count][];
            for (int index2 = 0; index2 < objs.Count; ++index2)
            {
                dtData[index2] = new string[count];
                for (int index3 = 0; index3 < count; ++index3)
                    dtData[index2][index3] = strArray[index2, index3];
            }
            return this.SqlHelper.ExecuteMutliQuery(commandText, dtData) == objs.Count;
        }

        public List<T> QueryList(string sSql)
        {
            DataTable dataTable = this.SqlHelper.QueryTable(this.GetQuerySql() + sSql);
            List<T> objList = new List<T>(dataTable.Rows.Count);
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                objList.Add((T)CClazzUtils.SetClazzProperties((object)new T(), row));
            return objList;
        }

        public List<T> QueryList(string sSql, int startPage, int size, string orderColumn)
        {
            string str = sSql;
            string sSql1;
            if (startPage <= 0 || size <= 0)
            {
                sSql1 = sSql;
            }
            else
            {
                if (!string.IsNullOrEmpty(orderColumn))
                    str = str + " order by " + orderColumn;
                sSql1 = str + string.Format(" limit {0} offset {0}*{1}", (object)size, (object)(startPage - 1));
            }
            return this.QueryList(sSql1);
        }

        public int Delete(string strWhere)
        {
            return this.SqlHelper.ExecuteNonSql(string.Format("delete from {0} {1}", (object)this.GetTableName(), (object)strWhere));
        }

        public T QuerySingle(string strWhere)
        {
            return this.QueryList(strWhere).FirstOrDefault<T>();
        }

        public string GetQuerySql()
        {
            return string.Format("select * from {0} ", (object)this.GetTableName());
        }

        public bool UpdateTable(Dictionary<string, string> modifiedData, string strWhere)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in modifiedData)
                stringBuilder.Append(string.Format("{0}='{1}',", (object)keyValuePair.Key, (object)keyValuePair.Value));
            if (stringBuilder.Length <= 0)
                return false;
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return this.SqlHelper.ExecuteSql(string.Format("update {0} set {1} where {2}", (object)this.GetTableName(), (object)stringBuilder, (object)strWhere));
        }

        public bool UpdateTable(Dictionary<string, object> modifiedData, string strWhere)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, object> keyValuePair in modifiedData)
                if (keyValuePair.Value is int)
                    stringBuilder.Append(string.Format("{0}={1},", (object)keyValuePair.Key, keyValuePair.Value));
                else
                    stringBuilder.Append(string.Format("{0}='{1}',", (object)keyValuePair.Key, keyValuePair.Value));
            if (stringBuilder.Length <= 0)
                return false;
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return this.SqlHelper.ExecuteSql(string.Format("update {0} set {1} where {2}", (object)this.GetTableName(), (object)stringBuilder, (object)strWhere));
        }

        public int UpdateTable(Dictionary<string, object> modifiedData, List<T> items)
        {
            if (items == null || items.Count == 0 || modifiedData == null || modifiedData.Count == 0)
                return 0;
            List<string> strSqls = new List<string>();
            StringBuilder stringBuilder1 = new StringBuilder();
            foreach (KeyValuePair<string, object> keyValuePair in modifiedData)
            {
                if (keyValuePair.Value is int)
                    stringBuilder1.Append(string.Format("{0}={1},", (object)keyValuePair.Key, keyValuePair.Value));
                else
                    stringBuilder1.Append(string.Format("{0}='{1}',", (object)keyValuePair.Key, keyValuePair.Value));
            }
            stringBuilder1.Remove(stringBuilder1.Length - 1, 1);
            StringBuilder stringBuilder2 = new StringBuilder();
            int num = 0;
            foreach (T obj in items)
            {
                foreach (Columns columns in AttributeHelper.GetCustomAttribute((object)obj))
                {
                    string colName = columns.ColName;
                    if (columns.ColPorperty.ToUpper().Contains("PRIMARY KEY"))
                    {
                        if (stringBuilder2.Length == 0)
                            stringBuilder2.Append(colName + " in (");
                        stringBuilder2.Append(string.Format("'{0}',", columns.ColData));
                        break;
                    }
                }
                if (num++ >= 900)
                {
                    stringBuilder2.Remove(stringBuilder2.Length - 1, 1).Append(")");
                    string str = string.Format("update {0} set {1} where {2}", (object)this.GetTableName(), (object)stringBuilder1, (object)stringBuilder2);
                    strSqls.Add(str);
                    num = 0;
                    stringBuilder2 = new StringBuilder();
                }
            }
            if (num > 0)
            {
                stringBuilder2.Remove(stringBuilder2.Length - 1, 1).Append(")");
                string str = string.Format("update {0} set {1} where {2}", (object)this.GetTableName(), (object)stringBuilder1, (object)stringBuilder2);
                strSqls.Add(str);
            }
            return this.SqlHelper.ExecuteSql(strSqls);
        }

        public bool CreateTable()
        {
            bool flag = false;
            if (!this.CheckTable(this.GetTableName()))
                flag = this.CreateDataTable(this.GetTableName());
            return flag;
        }

        private bool CheckTable(string table)
        {
            try
            {
                foreach (DataRow row in (InternalDataCollectionBase)this.SqlHelper.QuerySql("select name from sqlite_master where type='table' order by name;").Rows)
                {
                    if (row.ItemArray[0].ToString() == table)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //CLog.GetInstance().Debug(ex.Message + ex.StackTrace);
            }
            return false;
        }

        public bool CreateDataTable(string tableName)
        {
            List<Columns> customAttribute = AttributeHelper.GetCustomAttribute((object)new T());
            string str = "CREATE   TABLE  " + tableName + " (";
            foreach (Columns columns in customAttribute)
                str = str + columns.ColName + " " + columns.ColPorperty + ",";
            string strSql = str.TrimEnd(',') + ")";
            bool flag = false;
            try
            {
                flag = this.SqlHelper.ExecuteSql(strSql);
            }
            catch (Exception ex)
            {
                //CLog.GetInstance().Debug(ex.Message + ex.StackTrace);
            }
            return flag;
        }
    }
}
