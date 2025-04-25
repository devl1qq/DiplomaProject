using PX.Data.BQL;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DiscrepancyTypeSeverityAttribute : PXStringListAttribute
    {
        public sealed class low : BqlType<IBqlString, string>.Constant<low>
        {
            public low()
                : base("L")
            {
            }
        }

        public sealed class medium : BqlType<IBqlString, string>.Constant<medium>
        {
            public medium()
                : base("M")
            {
            }
        }

        public sealed class high : BqlType<IBqlString, string>.Constant<high>
        {
            public high()
                : base("H")
            {
            }
        }

        public const string _LOW = "L";

        public const string _MEDIUM = "M";

        public const string _HIGH = "H";


        public DiscrepancyTypeSeverityAttribute()
            : base(new string[3] { "L", "M", "H" }, new string[3] { "Low", "Medium", "High" })
        {
        }
    }
}
