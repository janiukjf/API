using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using MoreLinq;

namespace API {
    partial class CustomerPricing {

        public List<SimplePricing> GetAll(string key) {
            Authenticate(key);
            List<int> statuses = new List<int> { 800, 900 };
            CurtDevDataContext db = new CurtDevDataContext();

            List<SimplePricing> allparts = new List<SimplePricing>();
            allparts = (from c in db.Parts
                        join cpr in db.CustomerPricings on c.partID equals cpr.partID into PricingTemp
                        from cp in PricingTemp.Where(x => x.cust_id == this.cust_id).DefaultIfEmpty()
                        join cir in db.CartIntegrations on c.partID equals cir.partID into IntegrationTemp
                        from ci in IntegrationTemp.Where(x => x.custID == this.cust_id).DefaultIfEmpty()
                        where statuses.Contains(c.status)
                        select new SimplePricing {
                            cust_id = this.cust_id,
                            partID = c.partID,
                            custPartID = (ci.custPartID == null) ? 0 : ci.custPartID,
                            price = (cp.price == null) ? 0 : cp.price,
                            isSale = cp.isSale == null ? 0 : cp.isSale,
                            sale_start = cp.sale_start == null ? "" : Convert.ToDateTime(cp.sale_start).ToString(),
                            sale_end = cp.sale_end == null ? "" : Convert.ToDateTime(cp.sale_end).ToString(),
                            msrp = ((c.Prices.Any(x => x.priceType.Equals("List"))) ? c.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault() : 0),
                            map = ((c.Prices.Any(x => x.priceType.Equals("eMap"))) ? c.Prices.Where(x => x.priceType.Equals("eMap")).Select(x => x.price1).FirstOrDefault() : 0),
                        }).AsParallel().ToList();

            return allparts.OrderBy(x => x.partID).ToList<SimplePricing>();
        }

        public SimplePricing Set(string key) {
            Authenticate(key);
            CurtDevDataContext db = new CurtDevDataContext();

            // Validate required fields
            if (this.price == 0) { throw new Exception("Price failed to validate against null or zero."); }
            if (this.partID == 0) { throw new Exception("Part Number failed to validate against null or zero."); }
            if (this.isSale == 1) {
                if (this.sale_start == null || this.sale_start < DateTime.Today.AddDays(-1)) { throw new Exception("If record is going to marked as sale, sale start is required and cannot be in the past."); }
                if (this.sale_end == null || this.sale_end < DateTime.Now || this.sale_end <= this.sale_start) { throw new Exception("If record is going to marked as sale, sale end is required, cannot be in the past, and cannot be sooner or equal to the sale start."); }
            } else {
                this.sale_start = null;
                this.sale_end = null;
            }

            // Make sure the price point isn't set lower than map
            if (!checkMap()) {
                throw new Exception("You may not set the price point lower than map price.");
            }
            CustomerPricing newpricing = new CustomerPricing {
                cust_id = this.cust_id,
                partID = this.partID,
                price = this.price,
                isSale = this.isSale,
                sale_start = this.sale_start,
                sale_end = this.sale_end
            };

            // Attempt to get a CustomerPricing record for this customerID and partID
            List<CustomerPricing> tmpPoints = db.CustomerPricings.Where(x => x.cust_id.Equals(this.cust_id) && x.partID.Equals(this.partID)).ToList<CustomerPricing>();
            bool updated = false;
            List<CustomerPricing> deletables = new List<CustomerPricing>();
            foreach (CustomerPricing tmpPoint in tmpPoints) {
                bool deleted = false;
                if (tmpPoint.sale_end != null && tmpPoint.sale_end < DateTime.Now) {
                    // expired sale - delete
                    deletables.Add(tmpPoint);
                    deleted = true;
                }
                if (!deleted && this.isSale == tmpPoint.isSale) {
                    if (!updated) {
                        tmpPoint.price = this.price;
                        tmpPoint.isSale = this.isSale;
                        tmpPoint.sale_start = this.sale_start;
                        tmpPoint.sale_end = this.sale_end;
                        newpricing.cust_price_id = tmpPoint.cust_price_id;
                        updated = true;
                    } else {
                        deletables.Add(tmpPoint);
                    }
                }
            }
            if (!updated) {
                db.CustomerPricings.InsertOnSubmit(newpricing);
            }
            if (deletables.Count > 0) {
                db.CustomerPricings.DeleteAllOnSubmit(deletables);
            }
            db.SubmitChanges();

            CustomerPricing currentPrice = db.CustomerPricings.Where(x => x.cust_price_id.Equals(newpricing.cust_price_id)).FirstOrDefault<CustomerPricing>();

            SimplePricing pricePoint = new SimplePricing {
                cust_id = currentPrice.cust_id,
                partID = currentPrice.partID,
                price = currentPrice.price,
                isSale = currentPrice.isSale,
                sale_start = currentPrice.sale_start.ToString(),
                sale_end = currentPrice.sale_end.ToString()
            };
            return pricePoint;
        }

        public SimplePricing SetToList(string key) {
            Authenticate(key);

            if (this.partID == 0) { throw new Exception("Invalid reference to part."); }

            this.price = GetList();

            return this.Set(key);
        }

        public void SetAllToMap(string key) {
            Authenticate(key);

            var ctx = new CurtDevDataContext();
            
            // Delete existing records
            ctx.ExecuteCommand(
                "delete from CustomerPricing where cust_id = {0}", this.cust_id);

            // Repopulate from the Price table
            ctx.ExecuteCommand(
                "insert into CustomerPricing(cust_id, partID, price, isSale) select {0},p.partID, p.price,0 from Price p where p.priceType = 'Map'", this.cust_id);

        }

        public SimplePricing SetToMap(string key) {
            Authenticate(key);
            if (this.partID == 0) { throw new Exception("Invalid reference to part."); }

            this.price = GetMap();

            return this.Set(key);
        }

        public void RemoveSale(string key) {
            Authenticate(key);

            if (this.partID == 0) { throw new Exception("Invalid reference to part."); }
            if (this.price == 0) { throw new Exception("Invalid price point."); }

            CurtDevDataContext db = new CurtDevDataContext();

            CustomerPricing point = db.CustomerPricings.Where(x => x.partID.Equals(this.partID) && x.cust_id.Equals(this.cust_id) && x.isSale.Equals(1) && x.price.Equals(this.price)).FirstOrDefault<CustomerPricing>();
            db.CustomerPricings.DeleteOnSubmit(point);

            db.SubmitChanges();
        }

        private bool checkMap() {
            CurtDevDataContext db = new CurtDevDataContext();
            Price map = db.Prices.Where(x => x.priceType.ToUpper().Equals("eMap") && x.partID.Equals(this.partID)).FirstOrDefault();
            if (this.price < (map.price1 / 2) && map.enforced) {
                return false;
            }
            return true;
        }

        internal decimal GetList() {
            CurtDevDataContext db = new CurtDevDataContext();
            return db.Prices.Where(x => x.partID.Equals(this.partID) && x.priceType.ToUpper().Equals("LIST")).Select(x => x.price1).FirstOrDefault<decimal>();
        }

        internal decimal GetMap(){
            var db = new CurtDevDataContext();
            return db.Prices.Where(x => x.partID.Equals(this.partID) && x.priceType.ToUpper().Equals("eMap")).Select(x => x.price1).FirstOrDefault<decimal>();
        }

        /*protected int GetCustomerReference() {
            CurtDevDataContext db = new CurtDevDataContext();
            return db.Customers.Where(x => x.customerID.Equals(this.cust_id)).Select(x => x.cust_id).FirstOrDefault<int>();
        }*/

        internal void Authenticate(string key) {
            if (key == null || key.Length == 0) { throw new Exception("Invalid API key."); }
            if (this.cust_id == 0) { throw new Exception("Invalid customerID"); }

            Guid api_key = Guid.Parse(key);

            CurtDevDataContext db = new CurtDevDataContext();
            Customer cust = db.Customers.Where(x => x.customerID.Equals(this.cust_id) & x.APIKey.Equals(api_key)).FirstOrDefault<Customer>();
            if (cust == null || cust.cust_id == 0) {
                // We need to attempt to auth against a CustomerUsers Key
                var customerID = (from cu in db.CustomerUsers
                            join c in db.Customers on cu.cust_ID equals c.cust_id
                            join ak in db.ApiKeys on cu.id equals ak.user_id
                            join akt in db.ApiKeyTypes on ak.type_id equals akt.id
                            where !akt.type.ToUpper().Equals("AUTHENITCATION") && ak.api_key.Equals(api_key)
                            && c.customerID.Equals(this.cust_id)
                            select c.customerID).FirstOrDefault<int?>();
                if (customerID == null || customerID == 0) {
                    throw new Exception("Denied Access.");
                }
            }
        }

    }

    public class SimplePricing {
        public int cust_id { get; set; }
        public int custPartID { get; set; }
        public int partID { get; set; }
        public decimal price { get; set; }
        public int isSale { get; set; }
        public string sale_start { get; set; }
        public string sale_end { get; set; }
        public decimal msrp { get; set; }
        public decimal map { get; set; }
    }
}