using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }




    public class NotFoundNoDataException : Exception
    {
        public NotFoundNoDataException(string message) : base(message)
        {
        }
    }
}
