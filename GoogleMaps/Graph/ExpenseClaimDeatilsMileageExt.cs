using MileageCustomisation.DAC;
using PX.Data;
using PX.Objects.EP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MileageCustomisation.Helpers;
using PX.Api;
using PX.Web.UI;
using System.Web;
using System.Web.UI;
using PX.Objects.AP;
using static PX.Data.BQL.BqlPlaceholder;
using PX.Objects.FA;
using PX.Common;
using System.Text.RegularExpressions;
using static PX.SM.EMailAccount;

namespace MileageCustomisation.Graph
{
    
    public class ExpenseClaimDeatilsMileageExt<TGraphExt, TGraph> : PXGraphExtension<TGraphExt, TGraph> 
        where TGraph : PXGraph 
        where TGraphExt : PXGraphExtension<TGraph>
    {
        public PXSetup<EPSetup> EPSetup;

        #region Actions

        #region CalculateMileage
        public PXAction<EPExpenseClaimDetails> CalculateMileage;
        [PXUIField(DisplayName = "Calculate Mileage")]
        [PXButton(Category = "Mileage")]
        public virtual IEnumerable calculateMileage(PXAdapter adapter)
        {
            var expenseClaimDetailCache = Base.Caches[typeof(EPExpenseClaimDetails)];
            var expenseClaimDetail = expenseClaimDetailCache.Current as EPExpenseClaimDetails;
            var expenseClaimDetailExt = expenseClaimDetail.GetExtension<EPExpenseClaimDetailsExt>();

            var apiKey = EPSetup.Current?.GetExtension<EPSetupExt>()?.UsrGooleMapsAPIKey;
            var originLocation = expenseClaimDetailExt.UsrFromLocation;
            var destinationLocation = expenseClaimDetailExt.UsrToLocation;

            if (string.IsNullOrEmpty(apiKey))
                throw new PXException("Google Maps API Key is empty.");

            if (string.IsNullOrEmpty(originLocation))
                throw new PXException("'From location' cannot be empty.");

            if (string.IsNullOrEmpty(destinationLocation))
                throw new PXException("'To Location' cannot be empty.");

            PXLongOperation.StartOperation(Base.UID, () =>
            {
                var multiplier = expenseClaimDetailExt.UsrRoundTrip == true ? 2m : 1m;
                var distance = GoogleMapsApiHelper.CalculateDistance(originLocation, destinationLocation, apiKey);

                expenseClaimDetailCache.SetValueExt<EPExpenseClaimDetailsExt.usrCalculatedMileage>(expenseClaimDetail, (decimal?)distance * multiplier);
                expenseClaimDetailCache.Update(expenseClaimDetail);

                Base.Actions["Save"].Press();
            });

            return adapter.Get();
        }
        #endregion

        #endregion

        #region CacheAttached

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), nameof(PXUIFieldAttribute.DisplayName), "Claim Amount Exc Tax")]
        protected virtual void _(Events.CacheAttached<EPExpenseClaimDetails.curyTranAmt> e) { }

        #endregion

        #region Events

        protected virtual void _(Events.RowSelected<EPExpenseClaimDetails> e)
        {
            var row = e.Row;
            var rowExt = row?.GetExtension<EPExpenseClaimDetailsExt>();
            if (row == null) return;

            bool isMileAgeItem = IsMileageItem(row);

            // set enabled/disabled
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.curyTranAmt>(e.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetailsExt.usrPaymentTermsID>(e.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetailsExt.usrPLDepartmentID>(e.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetailsExt.usrReceiptAttached>(e.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetailsExt.usrMileageType>(e.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetailsExt.usrCalculatedMileage>(e.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetailsExt.usrTotalRate>(e.Cache, row, false);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetailsExt.usrWearAndTearRate>(e.Cache, row, false);

            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetailsExt.usrFromLocation>(e.Cache, row, isMileAgeItem);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetailsExt.usrToLocation>(e.Cache, row, isMileAgeItem);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetailsExt.usrClaimedMileage>(e.Cache, row, isMileAgeItem);

            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.curyUnitCost>(e.Cache, row, !isMileAgeItem);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.curyExtCost>(e.Cache, row, !isMileAgeItem);
            PXUIFieldAttribute.SetEnabled<EPExpenseClaimDetails.curyTipAmt>(e.Cache, row, !isMileAgeItem);

            // set visibility
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetailsExt.usrFromLocation>(e.Cache, row, isMileAgeItem);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetailsExt.usrToLocation>(e.Cache, row, isMileAgeItem);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetailsExt.usrCalculatedMileage>(e.Cache, row, isMileAgeItem);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetailsExt.usrClaimedMileage>(e.Cache, row, isMileAgeItem);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetailsExt.usrMileageType>(e.Cache, row, isMileAgeItem);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetailsExt.usrReasonForVariance>(e.Cache, row, isMileAgeItem);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetailsExt.usrRoundTrip>(e.Cache, row, isMileAgeItem);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetailsExt.usrTotalRate>(e.Cache, row, isMileAgeItem);
            PXUIFieldAttribute.SetVisible<EPExpenseClaimDetailsExt.usrWearAndTearRate>(e.Cache, row, isMileAgeItem);

            CalculateMileage.SetVisible(isMileAgeItem);
        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetailsExt.usrCalculatedMileage> e)
        {
            var row = e.Row;
            if (row == null) return;

            e.Cache.SetValueExt<EPExpenseClaimDetailsExt.usrClaimedMileage>(row, e.NewValue);
        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetails.qty> e)
        {
            var row = (EPExpenseClaimDetails)e.Row;
            var rowExt = row?.GetExtension<EPExpenseClaimDetailsExt>();
            if (row == null) return;

            if (!IsMileageItem(row))
                return;

            if (rowExt.UsrClaimedMileage != row.Qty)
                e.Cache.SetValueExt<EPExpenseClaimDetailsExt.usrClaimedMileage>(row, e.NewValue);

            PopulateFieldsFromMileageType(e.Cache, row);
        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetailsExt.usrClaimedMileage> e)
        {
            var row = (EPExpenseClaimDetails)e.Row;
            var rowExt = row?.GetExtension<EPExpenseClaimDetailsExt>();
            if (row == null) return;

            if (!IsMileageItem(row))
                return;

            if (rowExt.UsrClaimedMileage != row.Qty)
                e.Cache.SetValueExt<EPExpenseClaimDetails.qty>(row, e.NewValue);
        }
        protected virtual void _(Events.RowPersisting<EPExpenseClaimDetails> e)
        {
            var row = e.Row;
            var rowExt = row?.GetExtension<EPExpenseClaimDetailsExt>();
            if (row == null) return;

            var calculatedMileage = rowExt.UsrCalculatedMileage.GetValueOrDefault();
            var claimedMileage = rowExt.UsrClaimedMileage.GetValueOrDefault();
            var reasonForVarianceIsEmpty = string.IsNullOrEmpty(rowExt.UsrReasonForVariance);

            if (reasonForVarianceIsEmpty && claimedMileage != calculatedMileage)
                    e.Cache.RaiseExceptionHandling<EPExpenseClaimDetailsExt.usrReasonForVariance>(row, null,
                        new PXSetPropertyException<EPExpenseClaimDetailsExt.usrReasonForVariance>(
                            "The Reason for Variance must be provided if the Calculated Mileage and Claimed Mileage values differ."));
        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetailsExt.usrFromLocation> e)
        {
            var row = (EPExpenseClaimDetails)e.Row;
            if (row == null) return;

            RecalculateDistance(e.Cache, row);
        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetailsExt.usrToLocation> e)
        {
            var row = (EPExpenseClaimDetails)e.Row;
            if (row == null) return;

            RecalculateDistance(e.Cache, row);
        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetailsExt.usrRoundTrip> e)
        {
            var row = (EPExpenseClaimDetails)e.Row;
            var rowExt = row?.GetExtension<EPExpenseClaimDetailsExt>();
            if (row == null) return;

            var multiplier = (bool?)e.NewValue == true ? 2m : 0.5m;

            var newCalculatedMileage = rowExt.UsrCalculatedMileage * multiplier;
            var newClaimedMileage= rowExt.UsrClaimedMileage * multiplier;

            e.Cache.SetValueExt<EPExpenseClaimDetailsExt.usrCalculatedMileage>(row, newCalculatedMileage);
            e.Cache.SetValueExt<EPExpenseClaimDetailsExt.usrClaimedMileage>(row, newClaimedMileage);
        }

        // populate fields From ArcMileageType
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetails.inventoryID> e)
        {
            var row = (EPExpenseClaimDetails)e.Row;
            if (row == null) return;

            if (!IsMileageItem(row))
                ClearMileageFields(row);

            PopulateFieldsFromMileageType(e.Cache, row);
        }
        protected virtual void _(Events.FieldUpdated<EPExpenseClaimDetails.employeeID> e)
        {
            var row = (EPExpenseClaimDetails)e.Row;
            if (row == null) return;

            PopulateFieldsFromMileageType(e.Cache, row);
        }
        protected virtual void _(Events.RowInserted<EPExpenseClaimDetails> e)
        {
            var row = e.Row;
            if (row == null) return;

            PopulateFieldsFromMileageType(e.Cache, row);
        }

        // locations Validation & Input Mask
        protected virtual void _(Events.FieldSelecting<EPExpenseClaimDetailsExt.usrFromLocation> e)
        {
            if ((!Base.IsImport || Base.IsMobile) && !Base.IsContractBasedAPI && !Base.IsCopyPasteContext && e.IsAltered)
            {
                e.ReturnState = PXStringState.CreateInstance(
                    e.ReturnState,
                    null,
                    null,
                    nameof(EPExpenseClaimDetailsExt.UsrFromLocation),
                    null,
                    null,
                    GetLocationMask(),
                    null,
                    null,
                    null,
                    null);
            }
        }
        protected virtual void _(Events.FieldSelecting<EPExpenseClaimDetailsExt.usrToLocation> e)
        {
            if ((!Base.IsImport || Base.IsMobile) && !Base.IsContractBasedAPI && !Base.IsCopyPasteContext && e.IsAltered)
            {
                e.ReturnState = PXStringState.CreateInstance(
                    e.ReturnState,
                    null,
                    null,
                    nameof(EPExpenseClaimDetailsExt.UsrToLocation),
                    null,
                    null,
                    GetLocationMask(),
                    null,
                    null,
                    null,
                    null);
            }
        }
        protected virtual void _(Events.FieldVerifying<EPExpenseClaimDetailsExt.usrFromLocation> e)
        {
            if (Base.IsContractBasedAPI || Base.IsCopyPasteContext)
                return;

            var newValue = e.NewValue?.ToString();
            var regExp = GetLocationRegExp();

            if (!ValidateLocation(newValue, regExp))
                throw new PXSetPropertyException("The entered code doesn't match the required expression.");

            e.Cache.RaiseExceptionHandling<EPExpenseClaimDetailsExt.usrFromLocation>(e.Row, e.NewValue, null);
        }
        protected virtual void _(Events.FieldVerifying<EPExpenseClaimDetailsExt.usrToLocation> e)
        {
            if (Base.IsContractBasedAPI || Base.IsCopyPasteContext)
                return;

            var newValue = e.NewValue?.ToString();
            var regExp = GetLocationRegExp();

            if (!ValidateLocation(newValue, regExp))
                throw new PXSetPropertyException("The entered code doesn't match the required expression.");

            e.Cache.RaiseExceptionHandling<EPExpenseClaimDetailsExt.usrToLocation>(e.Row, e.NewValue, null);
        }

        #endregion

        #region Methods

        private bool IsMileageItem(EPExpenseClaimDetails row)
        {
            var epSetup = EPSetup.Current;
            var epSetupExt = epSetup.GetExtension<EPSetupExt>();

            return row != null && row.InventoryID == epSetupExt.UsrMileageItem;
        }
        private void PopulateFieldsFromMileageType(PXCache cache, EPExpenseClaimDetails row)
        {
            if (!IsMileageItem(row)) 
                return;

            var rowExt = row.GetExtension<EPExpenseClaimDetailsExt>();

            var mileageType = ArcMileageType.PK.Find(Base, rowExt.UsrMileageType);
            if (mileageType != null)
            {
                cache.SetValueExt<EPExpenseClaimDetails.curyUnitCost>(row, mileageType.FuelPart);
                cache.SetValueExt<EPExpenseClaimDetails.curyTipAmt>(row, mileageType.WearAndTearPart.GetValueOrDefault() * row.Qty.GetValueOrDefault());
            }
        }
        private void RecalculateDistance(PXCache cache, EPExpenseClaimDetails row)
        {
            var rowExt = row.GetExtension<EPExpenseClaimDetailsExt>();

            var apiKey = EPSetup.Current?.GetExtension<EPSetupExt>()?.UsrGooleMapsAPIKey;
            var fromLocation = rowExt.UsrFromLocation;
            var toLocation = rowExt.UsrToLocation;

            // check for empty
            if (string.IsNullOrEmpty(fromLocation) || string.IsNullOrEmpty(toLocation))
                return;

            // regExp validation
            var fromLocationValid = ValidateLocation(fromLocation, GetLocationRegExp());
            var toLocationValid = ValidateLocation(toLocation, GetLocationRegExp());
            if (!fromLocationValid || !toLocationValid)
                return;

            try
            {
                var multiplier = rowExt.UsrRoundTrip == true ? 2m : 1m;
                var distance = GoogleMapsApiHelper.CalculateDistance(fromLocation, toLocation, apiKey);
                cache.SetValueExt<EPExpenseClaimDetailsExt.usrCalculatedMileage>(row, (decimal?)distance * multiplier);
            }
            catch (Exception e)
            {
                PXTrace.WriteWarning(e.Message);
            }
        }
        private void ClearMileageFields(EPExpenseClaimDetails row)
        {
            var rowExt = row.GetExtension<EPExpenseClaimDetailsExt>();

            rowExt.UsrFromLocation = null;
            rowExt.UsrToLocation = null;
            rowExt.UsrCalculatedMileage = 0m;
            rowExt.UsrClaimedMileage = 0m;
            rowExt.UsrReasonForVariance = null;
            rowExt.UsrRoundTrip = false;
        }
        private string GetLocationMask() => EPSetup.Current.GetExtension<EPSetupExt>().UsrLocationInputMask;
        private string GetLocationRegExp() => EPSetup.Current.GetExtension<EPSetupExt>().UsrLocationValidationRegexp;
        private bool ValidateLocation(string val, string regex)
        {
            if (string.IsNullOrEmpty(val) || string.IsNullOrEmpty(regex))
                return true;

            return new Regex(regex).IsMatch(val);
        }

        #endregion
    }
}
