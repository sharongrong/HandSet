using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace HSDbOrm
{
    public class AttributeHelper
    {
        public static List<Columns> GetCustomAttribute(object obj)
        {
            List<Columns> columnsList = new List<Columns>();
            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                if (property.IsDefined(typeof(DbColumn), true))
                {
                    Columns columns = new Columns();
                    columns.ColIsId = property.IsDefined(typeof(IdColumn), true);
                    columns.ColName = property.Name;
                    columns.ColData = property.GetValue(obj, (object[])null);
                    foreach (object customAttribute in property.GetCustomAttributes(true))
                    {
                        if (customAttribute is DbColumn)
                        {
                            columns.ColPorperty = ((DbColumn)customAttribute).ColumnProperty;
                            break;
                        }
                    }
                    columnsList.Add(columns);
                }
            }
            return columnsList;
        }
    }
}
