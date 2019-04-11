using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Interface
{
    using CLEM.Resources;
    public partial interface IApsimX
    {
        ICollection<LandData> GetLandData();
    }
}
