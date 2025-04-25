using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.CR;
using PX.SM;

namespace MainProject.Graph
{
    public class CREmailActivityMaintExt : PXGraphExtension<CREmailActivityMaint>
    {
        public void _(Events.RowInserted<CRSMEmail> e)
        {
            var row = e.Row;
            if (row == null)
                return;

            if (row.CreatedByScreenID == "CR306000")
            {
                var relatedCase =
                    PXSelect<CRCase, Where<CRCase.noteID, Equal<Required<CRActivity.refNoteID>>>>.Select(Base,
                        row.RefNoteID);
                if (relatedCase == null) return;

                var owner = CRCase.FK.Owner.FindParent(Base, relatedCase);
                if (owner == null) return;

                row.MailCc = $"\"{owner.DisplayName}\" <{owner?.EMail}>; nc-case@MainProjectcorp.com";
            }

            Base.Message.Update(row);
        }
    }
}
