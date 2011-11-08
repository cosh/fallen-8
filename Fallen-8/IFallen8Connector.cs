using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens;

namespace BigDataSystems.Fallen8.API
{
    public interface IFallen8Connector
    {
        IFallen8Session Connect(SecurityToken token);
    }
}
