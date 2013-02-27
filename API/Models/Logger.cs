using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;

namespace API {
    partial class Logger {

        public static void LogMessage(string msg) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                Logger log = new Logger {
                    id = Guid.NewGuid(),
                    Message = msg,
                    Source = GetIp().ToString(),
                    Date = DateTime.Now,
                    loggedType = new Guid("C72B953C-1192-451E-B960-D7730B4F41B3")
                };
                db.Loggers.InsertOnSubmit(log);
                db.SubmitChanges();
            } catch { }
        }
        
        public static IPAddress GetIp() {
            string ipString;
            if (string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"])) {
                ipString = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            } else {
                ipString = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]
                   .Split(",".ToCharArray(),
                   StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            }

            IPAddress result;
            if (!IPAddress.TryParse(ipString, out result)) {
                result = IPAddress.None;
            }

            return result;
        }
    }
}