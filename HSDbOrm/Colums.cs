using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace HSDbOrm
{
    public class Columns
    {
        public string ColName { set; get; }

        public string ColPorperty { set; get; }

        public bool ColIsId { set; get; }

        public object ColData { set; get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumn : Attribute
    {
        public DbColumn(string colum)
        {
            this.ColumnProperty = colum;
        }

        public string ColumnProperty { get; set; }
    }

    public class IdColumn : Attribute
    { }
}
