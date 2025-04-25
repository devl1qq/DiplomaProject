using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MileageCustomisation.DAC;
using PX.Data;

namespace MileageCustomisation.Graph
{
    // Screen ID: EP502202
    public class ArcMileageTypeMaint : PXGraph<ArcMileageTypeMaint, ArcMileageType>
    {
        public PXSelect<ArcMileageType> MileageType;
    }
}
