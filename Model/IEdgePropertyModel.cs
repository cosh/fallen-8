using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.Model
{
    public interface IEdgePropertyModel : IEnumerable<IEdgeModel>
    {
        IVertexModel SourceVertex { get; }
    }
}
