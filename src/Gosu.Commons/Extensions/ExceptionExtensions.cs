using System;
using System.Text;

namespace Gosu.Commons.Extensions
{
    public static class ExceptionExtensions
    {
        private const string Indentation = "  ";

        public static string ToLogMessage(this Exception exception)
         {
             return exception.ToLogMessage("");
         }

         private static string ToLogMessage(this Exception exception, string prefix)
         {
             var message = new StringBuilder();

             message.AppendLine(prefix + "Message: " + exception.Message);
             message.AppendLine(prefix + "DateTime: " + DateTime.Now);
             message.AppendLine(prefix + "StackTrace:");
             message.AppendLine(prefix + exception.StackTrace);

             if (exception.InnerException != null)
             {
                 message.AppendLine(prefix + "Inner exception: --------------------------------------");
                 message.Append(exception.InnerException.ToLogMessage(prefix + Indentation));
             }

             return message.ToString();
         }
    }
}