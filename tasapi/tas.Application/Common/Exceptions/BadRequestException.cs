using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Common.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
            ErrorMessage = message;
        }

        public BadRequestException(string[] errors) : base("Multiple errors occurred. See error details.")
        {
           
            Errors = errors;
            ErrorMessage = GenerateErrorMessage(errors);
        }

        public string[] Errors { get; set; }


        private string ErrorMessage { get; }

        public override string Message => ErrorMessage;

        private string GenerateErrorMessage(params string[] messages)
        {
            StringBuilder builder = new StringBuilder();

            foreach (string message in messages)
            {
                builder.AppendLine(message + Environment.NewLine);
            }

            return builder.ToString();
        }
    }

    public class ForBiddenException : Exception
    {

        public ForBiddenException(string error) : base("You do not have access rights. Contact the administrator. Forbidden")
        {
            Error = error;
        }

        public string Error { get; set; }
    }



}
