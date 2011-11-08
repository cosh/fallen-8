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

        IGraphModel Get(IOperator op);


    }
}
