using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.Model
{
    public interface IEdgeModel : IGraphElementModel, IEquatable<IEdgeModel>
    {
        /// <summary>
        /// The target vertex of the edge
        /// </summary>
        IVertexModel TargetVertex { get; }

        /// <summary>
        /// The edge property model that contains the edge
        /// </summary>
        IEdgePropertyModel SourceEdgeProperty { get; }
    }
}
