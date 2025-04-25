using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MainProject.DAC.POLineSplitForm;

namespace MainProject.DAC
{
    public class POLineSplitForm : PXBqlTable, IBqlTable
    {
        #region POLineNbr
        [PXInt(IsKey = true)]
        [PXUIField(DisplayName = "PO Line Nbr", IsReadOnly = true)]
        public virtual int? POLineNbr { get; set; }
        public abstract class pOLineNbr : PX.Data.BQL.BqlInt.Field<pOLineNbr> { }
        #endregion
        #region POLineQuantity
        [PXDecimal]
        [PXUIField(DisplayName = "Quantity")]
        public virtual decimal? POLineQuantity { get; set; }
        public abstract class pOLineQuantity : PX.Data.BQL.BqlDecimal.Field<pOLineQuantity> { }
        #endregion
        #region POLineTrackingNbr
        [PXString(IsKey = true)]
        [PXUIField(DisplayName = "Tracking Number")]
        public virtual string POLineTrackingNbr { get; set; }
        public abstract class pOLineTrackingNbr : PX.Data.BQL.BqlString.Field<pOLineTrackingNbr> { }
        #endregion
        #region POLinePromisedDate
        [PXDate]
        [PXDefault]
        [PXUIField(DisplayName = "Promised", Required = true)]
        public virtual DateTime? POLinePromisedDate { get; set; }
        public abstract class pOLinePromisedDate : PX.Data.BQL.BqlDateTime.Field<pOLinePromisedDate> { }
        #endregion
        #region IsButtonEnabled
        [PXBool]
        [PXUIField(Visible = false)]
        public virtual bool? IsButtonEnabled { get; set; }
        public abstract class isButtonEnabled : PX.Data.BQL.BqlBool.Field<isButtonEnabled> { }
        #endregion

    }
}
