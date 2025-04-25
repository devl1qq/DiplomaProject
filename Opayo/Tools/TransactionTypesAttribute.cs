using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opayo.Tools
{
    public class TransactionTypesAttribute : PXStringListAttribute
    {
        public TransactionTypesAttribute() : base(Values.Arr, Labels.Arr) { }

        public class Values
        {

            public static readonly string[] Arr = 
            {
                SalesOrder,
                Invoice,
                Statement,
                Customer,
                Test,
            };

            public const string SalesOrder = "O";
            public const string Invoice = "I";
            public const string Statement = "S";
            public const string Customer = "C";
            public const string Test = "T";
        }
        public class Labels
        {
            public static readonly string[] Arr = 
            {
                SalesOrder,
                Invoice,
                Statement,
                Customer,
                Test,
            };

            public const string SalesOrder = "Sales Order";
            public const string Invoice = "Invoice";
            public const string Statement = "Statement";
            public const string Customer = "Customer";
            public const string Test = "Test";
        }
    }
}
