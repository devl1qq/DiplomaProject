using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PM;
using MainProject.DAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.Graph
{
    public class InventoryItemMaintExt : PXGraphExtension<InventoryItemMaint>
    {
        #region Events
        public void _(Events.FieldUpdated<InventoryItem.inventoryCD> e)
        {
            var row = (InventoryItem)e.Row;
            if (row == null)
                return;

            var segmentValue = row.InventoryCD == null ? null : row.InventoryCD.ToString().Substring(0, 4);
            if (segmentValue != row.GetExtension<InventoryItemExt>().UsrManufacturer)
                row.GetExtension<InventoryItemExt>().UsrManufacturer = segmentValue;
        }
        #endregion
    }
}
