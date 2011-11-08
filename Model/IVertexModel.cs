using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.Model
{
    public interface IVertexModel : IGraphElementModel, IEquatable<IVertexModel>
    {
        IDictionary<Int64, IEdgePropertyModel> Edges { get; }
        IEnumerable<IEdgeModel> GetIncomingEdges(Int64 typeId, Int64 edgeId);
    }
}
