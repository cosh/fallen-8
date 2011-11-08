using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.Model
{
    public interface IGraphElementModel
    {
        /// <summary>
        /// System wide unique id
        /// </summary>
        Int64 Id { get; }


        IDictionary<Int64, IComparable> Properties { get; }
        IDictionary<String, Object> SchemalessProperties { get; }
    }
}
