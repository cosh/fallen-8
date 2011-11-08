using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.Model
{
    public sealed class VertexModel : IVertexModel
    {
        #region IVertexModel Members

        public IDictionary<long, IEdgePropertyModel> Edges
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IEdgeModel> GetIncomingEdges(long typeId, long edgeId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphElementModel Members

        public IDictionary<long, IComparable> Properties
        {
            get { throw new NotImplementedException(); }
        }

        public IDictionary<string, object> SchemalessProperties
        {
            get { throw new NotImplementedException(); }
        }

        public long Id
        {
            get { throw new NotImplementedException(); }
        }

        public long TypeID
        {
            get { throw new NotImplementedException(); }
        }

        public string TypeName
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEquatable<IVertexModel> Members

        public bool Equals(IVertexModel other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}
