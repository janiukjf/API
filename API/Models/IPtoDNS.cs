using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace API {
    partial class IPtoDNS {

        public IPHostEntry Lookup(string address) {
            IPHostEntry hostEntry = new IPHostEntry();
            try {
                IPAddress hostIPAddress = IPAddress.Parse(address);
                hostEntry = Dns.GetHostEntry(hostIPAddress);
            } catch { }
            return hostEntry;
        }

        public void LookupAsync() {
            try {
                AsyncCallback callback = LookupAsyncCompleteCallback;
                ResolveState ioContext = new ResolveState(this.ipaddress, this.ID);
                IPAddress hostIPAddress = IPAddress.Parse(this.ipaddress);
                Dns.BeginGetHostEntry(hostIPAddress, callback, ioContext);
            } catch { }
        }

        /// <summary>
        /// Announce completion of PUT operation
        /// </summary>
        /// <param name="result"></param>
        private void LookupAsyncCompleteCallback(IAsyncResult ar) {
            ResolveState ioContext = (ResolveState)ar.AsyncState;
            loggingDataContext db = new loggingDataContext();
            string hostname = "unknown";
            try {
                ioContext.IPs = Dns.EndGetHostEntry(ar);
                hostname = ioContext.IPs.HostName;
            } catch { };
            IPtoDNS ip = db.IPtoDNS.Where(x => x.ID.Equals(ioContext.DnsID)).FirstOrDefault();
            if (ip != null && ip.ID > 0) {
                ip.dnsentry = hostname;
                db.SubmitChanges();
            }
            GetHostEntryFinished.Set();
        }

        public class ResolveState {
            string hostName;
            IPHostEntry resolvedIPs;
            int id;

            public ResolveState(string host, int dnsid) {
                hostName = host;
                id = dnsid;
            }

            public IPHostEntry IPs {
                get { return resolvedIPs; }
                set { resolvedIPs = value; }
            }
            public string host {
                get { return hostName; }
                set { hostName = value; }
            }

            public int DnsID {
                get { return id; }
                set { id = value; }
            }
        }

        // Signals when the resolve has finished. 
        public static ManualResetEvent GetHostEntryFinished =
            new ManualResetEvent(false);
    }
}