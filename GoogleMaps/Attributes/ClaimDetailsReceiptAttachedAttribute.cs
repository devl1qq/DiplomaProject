using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageCustomisation.Attributes
{
    public class ClaimDetailsReceiptAttachedAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
    {
        public void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
        {
            var row = e.Row;
            if (row == null) return;

            var anyFileAttached = PXNoteAttribute.GetFileNotes(sender, row).Any();
            e.ReturnValue = anyFileAttached;
        }
    }
}
