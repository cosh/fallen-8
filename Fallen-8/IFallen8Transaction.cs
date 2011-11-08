using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BigDataSystems.Fallen8.Model;

namespace BigDataSystems.Fallen8.API
{
    public interface IFallen8Transaction
    {
        IFallen8 Fallen8 { get; }

        void Commit();

        void Rollback();
    }
}
