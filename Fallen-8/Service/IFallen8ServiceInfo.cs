using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.API.Service
{
    public interface IFallen8ServiceInfo
    {
        DateTime StartTime { get; }
        Boolean IsRunning { get; }
        IDictionary<String, String> Metadata { get; }
        bool TryStop();
    }
}
