using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data.BQL;

namespace MainProject.Helper
{
    public class Constants
    {
        public class BQL
        {
            public class HighLevel : BqlType<IBqlString, string>.Constant<HighLevel>
            {
                public HighLevel()
                    : base("H")
                {
                }
            }
        }
        public class Messages
        {
            public const string HigherScoreItem = "There are higher scoring vendors for this item.";
            public const string CustomerHold = "Customer on hold.";
            public const string NCQtyGreaterThanQty = "NC Qty cannot be greater than Qty.";
            public const string CaseForPO = "NC For Purchase Receipt {0}";
            public const string CaseForSO = "NC For Sales Order {0}";
            public const string CaseForProdOrder = "NC For Production Order {0}";
            public const string SplitSumGreaterThanPOLine = "Sum of splitted POLines can't be greater than current POLine.";
            public const string POReceiptDoesNotHavePurchaseOrderLinked = "Current Purchase Receipt does not have reference for Purchase Order.";
            public const string CaseClassCannotBeEmpty = "Case Class can't be empty.";
            public const string CountryOfOriginIsEmptyWarning = "Please Enter Country of Origin.";
        }
    }
}
