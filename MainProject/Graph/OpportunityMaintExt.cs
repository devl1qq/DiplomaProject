using PX.Data;
using PX.Objects.CR;
using PX.Objects.CR.Extensions.CRCreateSalesOrder;
using PX.Objects.SO;
using MainProject.DAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Objects.CR.OpportunityMaint;
using static PX.Objects.CR.QuoteMaint;

namespace MainProject.Graph
{
    public class OpportunityMaintExt : PXGraphExtension<OpportunityMaint>
    {
        #region Overrides
        public delegate void CreateNewQuoteDelegate(CROpportunity opportunity, CreateQuotesFilter param, WebDialogResult result);
        [PXOverride]
        public virtual void CreateNewQuote(CROpportunity opportunity, CreateQuotesFilter param, WebDialogResult result, CreateNewQuoteDelegate baseHandler)
        {
            baseHandler(opportunity, param, result);
            var quote = Base.Quotes
                    .Select()
                    .RowCast<CRQuote>()
                    .ToList()
                    .Where(e => e.IsPrimary == true)
                    .FirstOrDefault();
           quote.GetExtension<CRQuoteExt>().UsrSalesPerson = opportunity.GetExtension<CROpportunityExt>().UsrSalesPerson;
           Base.Quotes.Update(quote);
        }
        #endregion
    }
}
