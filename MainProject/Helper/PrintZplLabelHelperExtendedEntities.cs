using GoodsInLabel.Graph;
using GoodsInLabel.Utils;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GoodsInLabel.Utils.PrintZplLabelHelper;
using PX.Objects.SO;
using GoodsInLabel.DAC;
using PX.Objects.PO;
using PX.Objects.IN;
using PX.Objects.CR;
using PX.Objects.AP;
using MainProject.DAC;
using PX.Objects.AR;
using PX.Objects.AM;
using GoodsInLabel.DAC.Virtual;

namespace MainProject.Helper
{
    public class ArcZplTemplateMaintExt : PXGraphExtension<ArcZplTemplateMaint>
    {
        public override void Initialize()
        {
            PrintZplLabelHelperExtendedEntities.Initialize();
            base.Initialize();
        }
    }

    public class ArcPrintedZPLMaintExt : PXGraphExtension<ArcPrintZplLabelMaint>
    {
        public override void Initialize()
        {
            PrintZplLabelHelperExtendedEntities.Initialize();
            base.Initialize();
        }
    }

    public class PrintZplLabelHelperExtendedEntities
    {
        public static void Initialize() { }
        static PrintZplLabelHelperExtendedEntities()
        {
            SupportedEntities.RemoveAll(e => e.PrimaryDac == typeof(POReceiptLineSplit));
            SupportedEntities.Add(new Entity()
            {
                DisplayName = "MainProject PO Receipt Line Split",
                PrimaryDac = typeof(POReceiptLineSplit),
                Select = typeof(
                    Select2<POReceiptLineSplit,

                        LeftJoin<InventoryItem, On<POReceiptLineSplit.inventoryID, Equal<InventoryItem.inventoryID>>,
                        LeftJoin<POReceipt, On<POReceipt.receiptType, Equal<POReceiptLineSplit.receiptType>, And<POReceipt.receiptNbr, Equal<POReceiptLineSplit.receiptNbr>>>,
                        LeftJoin<INSite, On<INSite.siteID, Equal<POReceiptLineSplit.siteID>>,
                        LeftJoin<BAccount, On<BAccount.bAccountID, Equal<POReceipt.vendorID>>,
                        LeftJoin<Vendor, On<Vendor.bAccountID, Equal<POReceipt.vendorID>>,

                        LeftJoin<POReceiptLine, On<POReceiptLine.lineNbr, Equal<POReceiptLineSplit.lineNbr>,
                            And<POReceiptLine.receiptNbr, Equal<POReceiptLineSplit.receiptNbr>,
                                And<POReceiptLine.receiptType, Equal<POReceiptLineSplit.receiptType>>>>,
                        LeftJoin<POLine, On<POLine.lineNbr, Equal<POReceiptLine.pOLineNbr>,
                            And<POLine.orderNbr, Equal<POReceiptLine.pONbr>,
                                And<POLine.orderType, Equal<POReceiptLine.pOType>>>>,
                        LeftJoin<SOLine, On<SOLine.lineNbr, Equal<POLineExt.usrSOLineNbr>,
                            And<SOLine.orderNbr, Equal<POLineExt.usrSONbr>,
                                And<SOLine.orderType, Equal<POLineExt.usrSOType>>>>,
                        LeftJoin<SOOrder, On<SOOrder.orderType, Equal<SOLine.orderType>,
                            And<SOOrder.orderNbr, Equal<SOLine.orderNbr>>>,
                        LeftJoin<Customer, On<Customer.bAccountID, Equal<SOOrder.customerID>>>>>>>>>>>>,

                        Where<POReceiptLineSplitArcExt.noteID, Equal<Required<PrintZplFilter.refNoteID>>>>
                ),
                InnerTypes = new Type[]
                {
                    typeof(POReceiptLineSplit),
                    typeof(POReceipt),
                    typeof(InventoryItem),
                    typeof(INSite),
                    typeof(BAccount),
                    typeof(Vendor),
                    typeof(POReceiptLine),
                    typeof(POLine),
                    typeof(SOLine),
                    typeof(SOOrder),
                    typeof(Customer),
                }
            });
            SupportedEntities.Add(new Entity()
            {
                DisplayName = "Cases SO/PO Details",
                PrimaryDac = typeof(ArcSOPODetails),
                Select = typeof(
                   Select2<ArcSOPODetails,
                   LeftJoin<CRCase, On<CRCase.noteID, Equal<ArcSOPODetails.refNoteID>>,
                   LeftJoin<SOOrder, On<SOOrder.noteID, Equal<ArcSOPODetails.relatedEntityID>>,
                   LeftJoin<POOrder, On<POOrder.noteID, Equal<ArcSOPODetails.relatedEntityID>>,
                   LeftJoin<BAccount, On<BAccount.bAccountID, Equal<POOrder.vendorID>>,
                   LeftJoin<AMProdItem, On<AMProdItem.noteID, Equal<ArcSOPODetails.productionOrderNoteID>>,
                   LeftJoin<InventoryItem, On<InventoryItem.inventoryID, Equal<ArcSOPODetails.inventoryItemID>>,
                   LeftJoin<Contact, On<Contact.contactID, Equal<CRCase.ownerID>>>>>>>>>,
                        
                        Where<ArcSOPODetails.noteID, Equal<Required<PrintZplFilter.refNoteID>>>>
               ),
                InnerTypes = new Type[]
               {
                    typeof(ArcSOPODetails),
                    typeof(CRCase),
                    typeof(SOOrder),
                    typeof(POOrder),
                    typeof(BAccount),
                    typeof(AMProdItem),
                    typeof(InventoryItem),
                    typeof(Contact)
               }
            });
        }
    }
}
