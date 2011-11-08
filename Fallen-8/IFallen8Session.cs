using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BigDataSystems.Fallen8.Model;
using System.Data;
using BigDataSystems.Fallen8.API.Service;

namespace BigDataSystems.Fallen8.API
{
    public interface IFallen8Session
    {
        IFallen8Server Server { get; }
            
        IFallen8Transaction Begin(IsolationLevel level = IsolationLevel.Chaos);

        void Disconnect();
    }
}
