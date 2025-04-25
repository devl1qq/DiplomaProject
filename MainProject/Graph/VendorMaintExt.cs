using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CN.Common.Extensions;
using MainProject.DAC;
using System;

namespace MainProject.Graph
{
    public class VendorMaintExt : PXGraphExtension<VendorMaint>
    {
        #region Events
        public void _(Events.RowSelected<VendorR> e)
        {
            var row = e.Row;
            if(row == null) 
                return;
            var rowExt = row.GetExtension<VendorExt>();
            PXUIFieldAttribute.SetEnabled<VendorR.vStatus>(e.Cache, row, rowExt.UsrRestricted != true);

        }
        public void _(Events.FieldUpdated<VendorExt.usrLastSurveyDate> e)
        {
            var row = (VendorR)e.Row;
            if(row == null || e.NewValue == null) 
                return; 

            row.GetExtension<VendorExt>().UsrSurveyExpirationDate = e.NewValue.Cast<DateTime>().AddYears(2);
        }

        public void _(Events.FieldUpdated<VendorExt.usrRestricted> e)
        {
            var row = (VendorR)e.Row;
            if (row == null || e.NewValue == null)
                return;

            if ((bool?)e.NewValue == true)
                row.VStatus = VendorStatus.Hold;
        }
        #endregion
    }
}
