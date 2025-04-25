using PX.Data;
using PX.Objects.AM;
using MainProject.DAC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.AR;
using static PX.Objects.CN.CRM.CR.DAC.MultipleQuote;

namespace MainProject.Graph
{
    
    public class EstimateMaintExt : PXGraphExtension<EstimateMaint>
    {
        #region Views

        public PXSelect<Customer, Where<Customer.bAccountID, Equal<Current<AMEstimateItemExt.usrBAccountID>>>>
            RelatedCustomer;

        #endregion
        #region Actions
        public PXAction<AMEstimateOper> keepSelected;
        [PXUIField(DisplayName = "Keep Selected", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable KeepSelected(PXAdapter adapter)
        {
            if (ConfirmationPopup("Please confirm keeping the selected operations and deleting the unselected ones.") != WebDialogResult.Yes)
                return adapter.Get();

            var unselectedOpers = Base.EstimateOperRecords
                .Select()
                .RowCast<AMEstimateOper>()
                .Where(oper => oper.GetExtension<AMEstimateOperExt>().UsrSelected != true);
            foreach (var oper in unselectedOpers)
                Base.EstimateOperRecords.Delete(oper);
            Base.Save.Press();

            UpdateOperIDs();
            Base.Save.Press();

            return adapter.Get();
        }
        public PXAction<AMEstimateOper> deleteSelected;
        [PXUIField(DisplayName = "Delete Selected", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable DeleteSelected(PXAdapter adapter)
        {
            if(ConfirmationPopup("Please confirm the deletion of selected operations.") != WebDialogResult.Yes)
                return adapter.Get();

            var selectedOpers = Base.EstimateOperRecords
                .Select()
                .RowCast<AMEstimateOper>()
                .Where(oper => oper.GetExtension<AMEstimateOperExt>().UsrSelected == true);

            foreach (var oper in selectedOpers)
                Base.EstimateOperRecords.Delete(oper);
            Base.Save.Press();

            UpdateOperIDs();
            Base.Save.Press();

            return adapter.Get();
        }
        #endregion
        #region Methods
        private void UpdateOperIDs()
        {
            var operIDCounter = 0;

            var allOpers = Base.EstimateOperRecords.Select().RowCast<AMEstimateOper>().ToList();
            var copies = new List<AMEstimateOper>();

            allOpers.ForEach(original  => 
            { 
                copies.Add(original.CreateCopy());
                Base.EstimateOperRecords.Delete(original);
            });
            Base.Save.Press();

            foreach (var oper in copies)
            {
                operIDCounter += 10;
                oper.OperationID = null;
                oper.OperationCD = operIDCounter.ToString("D4");
                Base.EstimateOperRecords.Insert(oper);
            }
        }
        private WebDialogResult ConfirmationPopup(string message) => 
            Base.EstimateOperRecords.Ask(message, MessageButtons.YesNo);
        #endregion
    }
}
