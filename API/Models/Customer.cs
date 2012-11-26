using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API {
    partial class Customer {

        public ReturnableCustomer Get() {
            CurtDevDataContext db = new CurtDevDataContext();

            ReturnableCustomer cust = (from c in db.Customers
                             where c.email.Equals(this.email) && c.customerID.Equals(this.customerID)
                             select new ReturnableCustomer {
                                 customerID = (c.customerID != null)? Convert.ToInt32(c.customerID) : 0,
                                 email = c.email,
                                 name = c.name,
                                 key = c.APIKey.ToString(),
                                 website = c.website
                             }).FirstOrDefault<ReturnableCustomer>();

            return cust;
        }

    }

    public class ReturnableCustomer {
        public int customerID { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string key { get; set; }
        public string website { get; set; }
    }
}