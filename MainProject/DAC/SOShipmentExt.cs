using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.SO;
using static MainProject.DAC.SOOrderExt.usrCustomerDescription;

namespace MainProject.DAC
{
    public class SOShipmentExt : PXCacheExtension<SOShipment>
    {
        #region UsrHold
        [PXBool()]
        [PXUIField(DisplayName = "Customer On Hold")]
        [PXFormula(typeof(Selector<SOShipment.customerID, CustomerExt.usrHold>))]
        public virtual bool? UsrHold { get; set; }
        public abstract class usrHold : PX.Data.BQL.BqlBool.Field<usrHold> { }
        #endregion
        #region UsrCarrierPrinterID
        [PXGuid()]
        [PXFormula(typeof(Selector<SOShipment.shipVia, CarrierExt.usrDefaultPrinterID>))]
        public virtual Guid? UsrCarrierPrinterID { get; set; }
        public abstract class usrCarrierPrinterID : PX.Data.BQL.BqlGuid.Field<usrCarrierPrinterID> { }
        #endregion

    }
}
