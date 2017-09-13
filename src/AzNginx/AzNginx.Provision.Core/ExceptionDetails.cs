using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzNginx.Provision.Core
{
    public class ExceptionDetails
    {
        private const string ExceptionMessageFormatString = "Error: {0}, InnerError: {1}, Code: {2}";

        private ExceptionDetails() { }

        public static ExceptionDetails Create(StorageException storageException)
        {
            ExceptionDetails exceptionDetails = new ExceptionDetails();
            exceptionDetails.ExceptionType = storageException.GetType();
            exceptionDetails.Exception = storageException;
            exceptionDetails.FormattedMessage = String.Format(ExceptionDetails.ExceptionMessageFormatString,
                storageException.Message,
                storageException.RequestInformation.ExtendedErrorInformation != null ?
                    storageException.RequestInformation.ExtendedErrorInformation.ErrorMessage :
                    "",
                storageException.RequestInformation.HttpStatusCode
            );

            exceptionDetails.HttpStatusCode = (HttpStatusCode)storageException.RequestInformation.HttpStatusCode;
            return exceptionDetails;
        }

        public static ExceptionDetails Create(Exception exception)
        {
            ExceptionDetails exceptionDetails = new ExceptionDetails();
            exceptionDetails.ExceptionType = exception.GetType();
            exceptionDetails.Exception = exception;
            exceptionDetails.FormattedMessage = String.Format(ExceptionDetails.ExceptionMessageFormatString,
                exception.Message,
                exception.InnerException != null ?
                    exception.InnerException.Message :
                    "",
                500
            );
            exceptionDetails.HttpStatusCode = default(HttpStatusCode);
            return exceptionDetails;
        }

        public Type ExceptionType
        {
            get;
            internal set;
        }

        public Exception Exception
        {
            get;
            internal set;
        }

        public string FormattedMessage
        {
            get;
            internal set;
        }
        public override string ToString()
        {
            return FormattedMessage;
        }
        public HttpStatusCode HttpStatusCode
        {
            get;
            internal set;
        }
    }
}
