using System;

namespace GestionFormation.App.Core
{
    public static class ExceptionExtention
    {
        public static string LastMessage(this Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            var currentException = exception;
            while (currentException.InnerException != null)
                currentException = currentException.InnerException;
            return currentException.Message;
        }
    }
}