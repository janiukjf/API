﻿using System;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;

namespace API {
    partial class APIAnalytic {

        public void Log(string url, string method, string qs) {
            try {
                CurtDevDataContext db = new CurtDevDataContext();
                IPtoDNS ip = GetOrCreateIP(GetIp().ToString());
                APIAnalytic entry = new APIAnalytic {
                    ID = Guid.NewGuid(),
                    addressID = ip.ID,
                    url = url,
                    method = method,
                    querystring = qs,
                    date = DateTime.Now
                };
                db.APIAnalytics.InsertOnSubmit(entry);
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

        private IPtoDNS GetOrCreateIP(string ipaddress) {
            CurtDevDataContext db = new CurtDevDataContext();
            IPtoDNS ipdetails = db.IPtoDNS.Where(x => x.ipaddress.Equals(ipaddress.Trim())).FirstOrDefault();
            if (ipdetails == null || ipdetails.ID == 0) {
                ipdetails = new IPtoDNS {
                    ipaddress = ipaddress.Trim(),
                    dateChecked = DateTime.Now
                };
                db.IPtoDNS.InsertOnSubmit(ipdetails);
                db.SubmitChanges();
                ipdetails.LookupAsync();
            }
            return ipdetails;
        }
    }
}