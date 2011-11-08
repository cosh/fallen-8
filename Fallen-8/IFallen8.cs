using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BigDataSystems.Fallen8.Model;
using BigDataSystems.Fallen8.API.Operator;

namespace BigDataSystems.Fallen8.API
{
    public interface IFallen8
    {
        IGraphModel Graph { get; }

        IVertexModel GetVertex(Int64 id);
        IEnumerable<IVertexModel> GetVertices(IEnumerable<Int64> ids);

        IEdgeModel GetEdge(Int64 id);
        IEnumerable<IEdgeModel> GetEdges(IEnumerable<Int64> ids);

        IFallen8 Calculon(IOperator op);
    }
}
