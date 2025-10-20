using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Common.Exceptions
{

    public class BadRequestWithDataException<T> : Exception
    {
        public BadRequestWithDataException(string message) : base(message)
        {
        }

        public BadRequestWithDataException(string[] errors, T data) : base("Multiple errors occurred. See error details.")
        {
            var errorResponse = new
            {
                message = "The request contains invalid data.",
                errors = errors,
                data = data
            };

            Response = new BadRequestObjectResult(errorResponse);
        }

        public IActionResult Response { get; }




    }
}
