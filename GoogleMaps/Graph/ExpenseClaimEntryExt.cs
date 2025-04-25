using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MileageCustomisation.DAC;
using PX.Data;
using PX.Objects.EP;

namespace MileageCustomisation.Graph
{
    public class ExpenseClaimEntryExt : PXGraphExtension<ExpenseClaimEntry>
    {
        public class ExpenseClaimEntry_ExpenseClaimDeatilsMileageExt : ExpenseClaimDeatilsMileageExt<ExpenseClaimEntry.ExpenseClaimEntryReceiptExt, ExpenseClaimEntry> {}
    }

}
