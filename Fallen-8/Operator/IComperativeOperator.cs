using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.API.Operator
{
    public interface IComperativeOperator : IOperator
    {
        ComperativeOperation Operator { get; set; }
    }
}
