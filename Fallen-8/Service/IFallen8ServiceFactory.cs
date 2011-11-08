using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.API.Service
{
    public interface IFallen8ServiceFactory
    {
        String Name { get; }
        String Description { get; }
        String Manufacturer { get; }

        IEnumerable<IFallen8ServiceInfo> RunningServices { get; }

        bool TryStart(out IFallen8ServiceInfo info);
    }
}
