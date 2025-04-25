using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GoodsInLabel.DAC;
using GoodsInLabel.Graph;
using PX.Api;
using PX.Common;
using PX.Data;
using PX.Objects.SO;
using PX.SM;
using MainProject.DAC;
using Constants = MainProject.Helper.Constants;

namespace MainProject.Graph
{
    public class SOShipmentEntryExt : PXGraphExtension<GoodsInLabel.Graph.SOShipmentEntryExt, SOShipmentEntry>
    {
        #region Events
        
        public void _(Events.RowSelected<SOShipment> e)
        {
            var row = e.Row;
            if (row == null) return;

            if(row.GetExtension<SOShipmentExt>().UsrHold == true)
                PXUIFieldAttribute.SetWarning<SOShipmentExt.usrHold>(e.Cache, row, Constants.Messages.CustomerHold);

            Base.releaseFromHold.SetEnabled(row.GetExtension<SOShipmentExt>().UsrHold == false && row.Status == SOShipmentStatus.Hold);
        }
        #endregion

        public delegate System.Threading.Tasks.Task CreatePrintJobsDelegate(PrintPackageFilesArgs printArg,
            CancellationToken cancellationToken);

        [PXOverride]
        public virtual async System.Threading.Tasks.Task CreatePrintJobs(PrintPackageFilesArgs printArg,
            CancellationToken cancellationToken, CreatePrintJobsDelegate baseHandler)
        {
            var customPrinterID = Base.CurrentDocument.Current?.GetExtension<SOShipmentExt>().UsrCarrierPrinterID;
            if (customPrinterID == null)
            {
                customPrinterID = PXSelect<UserPreferences, Where<UserPreferences.userID, Equal<Current<AccessInfo.userID>>>>
                    .Select(Base)
                    .RowCast<UserPreferences>()
                    .FirstOrDefault()?.DefaultPrinterID;
            }

            var oldPrinterParamID = printArg.PrinterToReportsMap.FirstOrDefault().Key;
            var oldPrinterParamValue = printArg.PrinterToReportsMap.FirstOrDefault().Value;
            printArg.PrinterToReportsMap.Clear();
            printArg.PrinterToReportsMap.Add(customPrinterID ?? oldPrinterParamID, oldPrinterParamValue);
            baseHandler(printArg, cancellationToken);
        }
    }
}
