using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

namespace HSDbOrm
{


    public static class CClazzUtils
    {
        public static Object SetClazzProperties(Object model, DataRow row)
        {
            Type t = model.GetType();
            PropertyInfo[] propertyList = t.GetProperties();
            foreach (PropertyInfo item in propertyList)
            {
                try
                {
                    item.SetValue(model, Convert.ChangeType(row[item.Name], item.PropertyType, null), null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return model;
        }

        public static Object SetClazzProperties(Object model, Dictionary<string, object> properties)
        {
            Type t = model.GetType();
            PropertyInfo[] propertyList = t.GetProperties();
            foreach (PropertyInfo item in propertyList)
            {
                item.SetValue(model, properties[item.Name], null);
            }
            return model;
        }
    }
}
