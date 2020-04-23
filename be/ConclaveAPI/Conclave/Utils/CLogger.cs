using System;
using System.IO;
using System.Text;

namespace Conclave.Utils
{
    /*
     * Logging utility
     */
    /// <summary>
    /// All logs are written to a file whose path is specified in appsettings.json
    /// </summary>
    public class CLogger
    {
        public static string _exceptionFilepath = "/";

        internal static void Log(Exception ex, string msg = "")
        {
            StringBuilder text = new StringBuilder();
            text.Append(DateTime.Now.ToString());
            text.Append("\t" + ex.Message);
            if (!string.IsNullOrEmpty(msg))
            {
                text.Append("\nMessage:" + msg);
            }
            text.Append("\nSource:\n" + ex.Source);
            text.Append("\nStacktrace:\n" + ex.StackTrace + "\n\n");
            File.AppendAllText(Path.Combine(_exceptionFilepath), text.ToString());
        }

        internal static void Log(int code = 0, string msg = "NA")
        {
            StringBuilder text = new StringBuilder();
            text.Append(DateTime.Now.ToString());
            text.Append("\t" + code);
            text.Append("\nMessage:\n" + msg + "\n\n");
            File.AppendAllText(Path.Combine(_exceptionFilepath), text.ToString());
        }
    }
}
