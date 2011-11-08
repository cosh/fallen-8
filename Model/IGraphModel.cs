using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.Model
{
    public interface IGraphModel : IGraphElementModel
    {
        IDictionary<Int64, IGraphElementModel> Graphelements { get; }

        IEnumerable<IVertexModel> GetVerticesByType(Int64 id);
        IEnumerable<IEdgeModel> GetEdgesByType(Int64 id);
    }
}
