using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MoreLinq;

namespace API {
    partial class CustomerPricing {

        public List<SimplePricing> GetAll(string key) {
            Authenticate(key);
            CurtDevDataContext db = new CurtDevDataContext();

            List<SimplePricing> integrated = (from c in db.Parts
                                              join cpr in db.CustomerPricings on c.partID equals cpr.partID into PricingTemp
                                              from cp in PricingTemp.DefaultIfEmpty()
                                              join ci in db.CartIntegrations on c.partID equals ci.partID
                                              where cp.cust_id.Equals(this._cust_id) && ci.custID.Equals(this.cust_id)
                                              select new SimplePricing {
                                                  cust_id = this.cust_id,
                                                  partID = c.partID,
                                                  custPartID = (ci.custPartID == null)? 0 : ci.custPartID,
                                                  price = cp.price,
                                                  isSale = cp.isSale,
                                                  sale_start = Convert.ToDateTime(cp.sale_start).ToString(),
                                                  sale_end = Convert.ToDateTime(cp.sale_end).ToString(),
                                                  msrp = c.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                                                  map = c.Prices.Where(x => x.priceType.Equals("Map")).Select(x => x.price1).FirstOrDefault(),
                                              }).DistinctBy(x => x.partID).ToList<SimplePricing>();
            List<int> integratedID = integrated.Select(x => x.partID).ToList<int>();
            List<int> allParts = db.Parts.Select(x => x.partID).ToList<int>();
            List<int> unintegratedIDs = allParts.Except(integratedID).ToList<int>();

            List<SimplePricing> unintegrated = new List<SimplePricing>();
            if (unintegratedIDs.Count > 2100) {
                while (unintegratedIDs.Count != 0) {
                    List<int> ids = new List<int>();
                    ids = unintegratedIDs.Take(2000).ToList<int>();
                    unintegratedIDs = unintegratedIDs.Skip(2000).ToList();
                    List<SimplePricing> current = this.GetUnintegrated(ids);
                    integrated.AddRange(current);
                }
            } else {
                unintegrated = this.GetUnintegrated(unintegratedIDs);
            }
            
            integrated.AddRange(unintegrated);

            List<SimplePricing> results = integrated.DistinctBy(x => x.partID).ToList<SimplePricing>();

            List<SimplePricing> sales = (from c in db.Parts
                                         join pr in db.CustomerPricings on c.partID equals pr.partID into PricingTemp
                                         from cp in PricingTemp.DefaultIfEmpty()
                                         join ci in db.CartIntegrations on c.partID equals ci.partID into IntegrateTemp
                                         from integ in IntegrateTemp.DefaultIfEmpty()
                                         where cp.cust_id.Equals(this._cust_id) && integ.custID.Equals(this.cust_id) && cp.isSale.Equals(1)
                                         select new SimplePricing {
                                             cust_id = this.cust_id,
                                             partID = c.partID,
                                             custPartID = (integ.custPartID == null) ? 0 : integ.custPartID,
                                             price = cp.price,
                                             isSale = cp.isSale,
                                             sale_start = Convert.ToDateTime(cp.sale_start).ToString(),
                                             sale_end = Convert.ToDateTime(cp.sale_end).ToString(),
                                             msrp = c.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                                             map = c.Prices.Where(x => x.priceType.Equals("Map")).Select(x => x.price1).FirstOrDefault(),
                                         }).ToList<SimplePricing>();
            results.AddRange(sales.DistinctBy(x => x.partID).ToList<SimplePricing>());

            return results.OrderBy(x => x.partID).ToList<SimplePricing>();
        }

        public List<SimplePricing> GetUnintegrated(List<int> unintegratedIDs) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<SimplePricing> unintegrated = (from p in db.Parts
                                                join pr in db.Prices on p.partID equals pr.partID
                                                join cpr in db.CustomerPricings on p.partID equals cpr.partID into PricingTemp
                                                from cp in PricingTemp.DefaultIfEmpty()
                                                where unintegratedIDs.Contains(p.partID) && pr.priceType.ToUpper().Equals("LIST")
                                                select new SimplePricing {
                                                    cust_id = this.cust_id,
                                                    partID = p.partID,
                                                    custPartID = 0,
                                                    price = (cp.price == null) ? pr.price1 : cp.price,
                                                    isSale = (cp.isSale == null) ? 0 : cp.isSale,
                                                    sale_start = Convert.ToDateTime(cp.sale_start).ToString(),
                                                    sale_end = Convert.ToDateTime(cp.sale_end).ToString(),
                                                    msrp = p.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                                                    map = p.Prices.Where(x => x.priceType.Equals("Map")).Select(x => x.price1).FirstOrDefault(),
                                                }).DistinctBy(x => x.partID).ToList<SimplePricing>();
            return unintegrated;
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
            }

            // Make sure the price point isn't set lower than map
            if (!checkMap()) {
                throw new Exception("You may not set the price point lower than map price.");
            }

            // Attempt to get a CustomerPricing record for this customerID and partID
            CustomerPricing tmpPoint = db.CustomerPricings.Where(x => x.cust_id.Equals(this.cust_id) && x.partID.Equals(this.partID) && x.isSale.Equals(this.isSale)).FirstOrDefault<CustomerPricing>();
            if (tmpPoint != null && tmpPoint.cust_price_id > 0) { // Update existing record
                tmpPoint.price = this.price;
                tmpPoint.isSale = this.isSale;
                tmpPoint.sale_start = this.sale_start;
                tmpPoint.sale_end = this.sale_end;
            } else { // Insert new record
                tmpPoint = this;
                tmpPoint.cust_id = this.cust_id;
                db.CustomerPricings.InsertOnSubmit(tmpPoint);
            }
            db.SubmitChanges();

            SimplePricing pricePoint = new SimplePricing {
                cust_id = this.cust_id,
                partID = tmpPoint.partID,
                price = tmpPoint.price,
                isSale = tmpPoint.isSale,
                sale_start = tmpPoint.sale_start.ToString(),
                sale_end = tmpPoint.sale_end.ToString()
            };
            return pricePoint;
        }

        public SimplePricing SetToList(string key) {
            Authenticate(key);

            if (this.partID == 0) { throw new Exception("Invalid reference to part."); }

            decimal listPrice = GetList();

            CurtDevDataContext db = new CurtDevDataContext();

            CustomerPricing pricePoint = db.CustomerPricings.Where(x => x.partID.Equals(this.partID) && x.cust_id.Equals(this.cust_id)).FirstOrDefault<CustomerPricing>();
            if (pricePoint == null) {
                pricePoint = new CustomerPricing {
                    cust_id = this.cust_id,
                    partID = this.partID,
                    isSale = 0
                };
            }
            pricePoint.price = listPrice;
            db.SubmitChanges();

            SimplePricing price = new SimplePricing {
                cust_id = this.cust_id,
                partID = pricePoint.partID,
                price = pricePoint.price,
                isSale = pricePoint.isSale,
                sale_start = ((pricePoint.sale_start != null)?Convert.ToDateTime(pricePoint.sale_start).ToString():""),
                sale_end = ((pricePoint.sale_end != null) ? Convert.ToDateTime(pricePoint.sale_end).ToString() : "")
            };
            return price;
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
            decimal map = db.Prices.Where(x => x.priceType.ToUpper().Equals("MAP") && x.partID.Equals(this.partID)).Select(x => x.price1).FirstOrDefault<decimal>();
            if (this.price < (map / 2)) {
                return false;
            }
            return true;
        }

        internal decimal GetList() {
            CurtDevDataContext db = new CurtDevDataContext();
            return db.Prices.Where(x => x.partID.Equals(this.partID) && x.priceType.ToUpper().Equals("LIST")).Select(x => x.price1).FirstOrDefault<decimal>();
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
                throw new Exception("Denied Access.");
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