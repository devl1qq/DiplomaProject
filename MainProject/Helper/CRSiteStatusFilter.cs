using PX.Data;
using PX.Objects.Common;
using PX.Objects.Common.Bql;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using System;

namespace MainProject.Helper
{
    [Serializable]
    public partial class CRSiteStatusFilter : INSiteStatusFilter
    {
        #region SiteID
        public new abstract class siteID : PX.Data.BQL.BqlInt.Field<siteID> { }

        [PXUIField(DisplayName = "Warehouse")]
        [SiteAttribute]
        [InterBranchRestrictor(typeof(Where<SameOrganizationBranch<INSite.branchID, Current<CROpportunity.branchID>>>))]
        [PXDefault(typeof(INRegister.siteID), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Int32? SiteID
        {
            get
            {
                return this._SiteID;
            }
            set
            {
                this._SiteID = value;
            }
        }
        #endregion

        #region Inventory
        public new abstract class inventory : PX.Data.BQL.BqlString.Field<inventory> { }
        #endregion

        #region Mode
        public abstract class mode : PX.Data.BQL.BqlInt.Field<mode> { }
        protected int? _Mode;
        [PXDBInt]
        [PXDefault(0)]
        [PXUIField(DisplayName = "Selection Mode")]
        [SOAddItemMode.List]
        public virtual int? Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                _Mode = value;
            }
        }
        #endregion

        #region HistoryDate
        public abstract class historyDate : PX.Data.BQL.BqlDateTime.Field<historyDate> { }
        protected DateTime? _HistoryDate;
        [PXDBDate()]
        [PXUIField(DisplayName = "Sold Since")]
        public virtual DateTime? HistoryDate
        {
            get
            {
                return this._HistoryDate;
            }
            set
            {
                this._HistoryDate = value;
            }
        }
        #endregion

        #region DropShipSales
        public abstract class dropShipSales : PX.Data.BQL.BqlBool.Field<dropShipSales> { }
        [PXDBBool]
        [PXDefault(false)]
        [PXFormula(typeof(Default<mode>))]
        [PXUIField(DisplayName = "Show Drop-Ship Sales")]
        public virtual bool? DropShipSales
        {
            get;
            set;
        }
        #endregion

        #region Behavior
        public abstract class behavior : PX.Data.BQL.BqlString.Field<behavior> { }
        [PXDBString(2, IsFixed = true, InputMask = ">aa")]
        public virtual string Behavior
        {
            get;
            set;
        }
        #endregion

        #region CustomerLocationID
        public abstract class customerLocationID : PX.Data.BQL.BqlInt.Field<customerLocationID> { }
        [LocationActive(typeof(Where<Location.bAccountID, Equal<Current<CROpportunity.bAccountID>>,
            And<MatchWithBranch<Location.cBranchID>>>), DescriptionField = typeof(Location.descr))]
        [PXUIField(DisplayName = "Ship-To Location")]
        public virtual int? CustomerLocationID
        {
            get;
            set;
        }
        #endregion
    }
}
