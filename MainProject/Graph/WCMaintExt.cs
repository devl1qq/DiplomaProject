using PX.Api;
using PX.Data;
using PX.Objects.AM;
using PX.Objects.AP;
using PX.Objects.PO;
using MainProject.DAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.Graph
{
    public class WCMaintExt : PXGraphExtension<WCMaint>
    {
        #region Views
        public PXSelect<ArcAMWCLocations, Where<ArcAMWCLocations.wCID, Equal<Current<AMWC.wcID>>>> ArcWorkLocations;
        #endregion
    }
}
