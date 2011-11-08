using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.API.Operator
{
    public interface ILogicalOperator : IOperator
    {
        LogicalOperation Operator { get; set; }
    }
}
