using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.Model
{
    public sealed class EdgeModel : IEdgeModel
    {
        #region IEdgeModel Members

        public long Id
        {
            get { throw new NotImplementedException(); }
        }

        public IVertexModel TargetVertex
        {
            get { throw new NotImplementedException(); }
        }

        public IEdgePropertyModel SourceEdgeProperty
        {
            get { throw new NotImplementedException(); }
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

        public long TypeID
        {
            get { throw new NotImplementedException(); }
        }

        public string TypeName
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEquatable<IEdgeModel> Members

        public bool Equals(IEdgeModel other)
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
