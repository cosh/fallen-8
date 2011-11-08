using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BigDataSystems.Fallen8.API.Operator
{
    public sealed class ComperativeOperator : IComperativeOperator, IBinaryOperator
    {
        #region IComperativeOperator Members

        public ComperativeOperation Operator
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IBinaryOperator Members

        public IOperator Left
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IOperator Right
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
