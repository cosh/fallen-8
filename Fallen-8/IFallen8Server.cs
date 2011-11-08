using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BigDataSystems.Fallen8.API.Service;

namespace BigDataSystems.Fallen8.API
{
    public interface IFallen8Server
    {
        void Shutdown();
        IDictionary<String, IFallen8ServiceFactory> Services { get; }
    }
}