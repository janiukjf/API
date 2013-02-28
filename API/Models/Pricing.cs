using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MoreLinq;

namespace API {
    partial class CustomerPricing {

        public List<SimplePricing> GetAll(string key) {
            Authenticate(key);
            CurtDevDataContext db = new CurtDevDataContext();

            List<SimplePricing> integrated = new List<SimplePricing>();
            List<SimplePricing> both = new List<SimplePricing>();
            List<SimplePricing> pricing = new List<SimplePricing>();
            List<SimplePricing> cart = new List<SimplePricing>();
            Parallel.Invoke(() => {
                both = FullIntegration();
            }, () => {
                pricing = PricingIntegration();
            }, () => {
                cart = CartIntegrated();
            });

            integrated.AddRange(both);
            List<int> integrated_parts = integrated.Select(x => x.partID).ToList<int>();
            if (integrated_parts.Count > 2100) {
                var part_nums = integrated_parts;
                while (part_nums.Count != 0) {
                    List<int> ids = new List<int>();
                    ids = part_nums.Take(200).ToList<int>();
                    part_nums = part_nums.Skip(2000).ToList<int>();

                    integrated.AddRange((from p in pricing
                                         where !integrated_parts.Contains(p.partID)
                                         select p).ToList<SimplePricing>());
                    integrated_parts = integrated.Select(x => x.partID).ToList<int>();
                    integrated.AddRange((from c in cart
                                         where !integrated_parts.Contains(c.partID)
                                         select c).ToList<SimplePricing>());
                }
            } else {
                integrated.AddRange((from p in pricing
                                     where !integrated_parts.Contains(p.partID)
                                     select p).ToList<SimplePricing>());
                integrated_parts = integrated.Select(x => x.partID).ToList<int>();
                integrated.AddRange((from c in cart
                                     where !integrated_parts.Contains(c.partID)
                                     select c).ToList<SimplePricing>());

            }

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
                                             map = c.Prices.Where(x => x.priceType.Equals("SuggestedMap")).Select(x => x.price1).FirstOrDefault(),
                                         }).ToList<SimplePricing>();
            results.AddRange(sales.DistinctBy(x => x.partID).ToList<SimplePricing>());

            var result_ids = results.OrderBy(x => x.partID).Select(x => x.partID).ToList<int>();
            var part_ids = allParts.OrderBy(x => x).ToList<int>();
            var unassigned = new List<SimplePricing>();
            if (result_ids.Count > 2100) {
                while (result_ids.Count != 0) {
                    var ids = result_ids.Take(2000).ToList<int>();
                    var p_ids = part_ids.Take(2000).ToList<int>();
                    result_ids = result_ids.Skip(2000).ToList<int>();
                    part_ids = part_ids.Skip(2000).ToList<int>();
                    
                    var need = (from pid in p_ids
                                where !ids.Contains(pid)
                                select pid).ToList<int>();

                    results.AddRange((from p in db.Parts
                                      where need.Contains(p.partID)
                                      select new SimplePricing {
                                          cust_id = this.cust_id,
                                          partID = p.partID,
                                          custPartID = 0,
                                          price = p.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                                          isSale = 0,
                                          sale_start = "",
                                          sale_end = "",
                                          msrp = p.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                                          map = p.Prices.Where(x => x.priceType.Equals("SuggestedMap")).Select(x => x.price1).FirstOrDefault(),
                                      }).ToList<SimplePricing>());
                }
            } else {
                unassigned = (from p in db.Parts
                              where !result_ids.Contains(p.partID)
                              select new SimplePricing {
                                  cust_id = this.cust_id,
                                  partID = p.partID,
                                  custPartID = 0,
                                  price = p.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                                  isSale = 0,
                                  sale_start = "",
                                  sale_end = "",
                                  msrp = p.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                                  map = p.Prices.Where(x => x.priceType.Equals("SuggestedMap")).Select(x => x.price1).FirstOrDefault(),
                              }).ToList<SimplePricing>();
            }
            results.AddRange(unassigned);
            return results.OrderBy(x => x.partID).ToList<SimplePricing>();
        }

        public List<SimplePricing> FullIntegration() {
            var db = new CurtDevDataContext();
            return (from c in db.Parts
                    join cpr in db.CustomerPricings on c.partID equals cpr.partID into PricingTemp
                    from cp in PricingTemp.DefaultIfEmpty()
                    join ci in db.CartIntegrations on c.partID equals ci.partID
                    where (cp.cust_id.Equals(this._cust_id) || cp.cust_id.Equals(null)) && (ci.custID.Equals(this.cust_id) || ci.custID.Equals(null))
                    select new SimplePricing {
                        cust_id = this.cust_id,
                        partID = c.partID,
                        custPartID = (ci.custPartID == null) ? 0 : ci.custPartID,
                        price = (cp.price == null) ? 0 : cp.price,
                        isSale = cp.isSale == null ? 0 : cp.isSale,
                        sale_start = cp.sale_start == null ? "" : Convert.ToDateTime(cp.sale_start).ToString(),
                        sale_end = cp.sale_end == null ? "" : Convert.ToDateTime(cp.sale_end).ToString(),
                        msrp = c.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                        map = c.Prices.Where(x => x.priceType.Equals("SuggestedMap")).Select(x => x.price1).FirstOrDefault(),
                    }).DistinctBy(x => x.partID).ToList<SimplePricing>();
        }

        public List<SimplePricing> CartIntegrated() {
            var db = new CurtDevDataContext();
            return (from c in db.Parts
                    join ci in db.CartIntegrations on c.partID equals ci.partID
                    where ci.custID.Equals(this.cust_id)
                    select new SimplePricing {
                        cust_id = this.cust_id,
                        partID = c.partID,
                        custPartID = (ci.custPartID == null) ? 0 : ci.custPartID,
                        price = c.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                        isSale = 0,
                        sale_start = "",
                        sale_end = "",
                        msrp = c.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                        map = c.Prices.Where(x => x.priceType.Equals("SuggestedMap")).Select(x => x.price1).FirstOrDefault(),
                    }).DistinctBy(x => x.partID).ToList<SimplePricing>();
        }

        public List<SimplePricing> PricingIntegration() {
            var db = new CurtDevDataContext();
            return (from c in db.Parts
                    join cp in db.CustomerPricings on c.partID equals cp.partID
                    where cp.cust_id.Equals(this._cust_id)
                    select new SimplePricing {
                        cust_id = this.cust_id,
                        partID = c.partID,
                        custPartID = 0,
                        price = (cp.price == null) ? 0 : cp.price,
                        isSale = cp.isSale == null ? 0 : cp.isSale,
                        sale_start = cp.sale_start == null ? "" : Convert.ToDateTime(cp.sale_start).ToString(),
                        sale_end = cp.sale_end == null ? "" : Convert.ToDateTime(cp.sale_end).ToString(),
                        msrp = c.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                        map = c.Prices.Where(x => x.priceType.Equals("SuggestedMap")).Select(x => x.price1).FirstOrDefault(),
                    }).DistinctBy(x => x.partID).ToList<SimplePricing>();
        }

        public List<SimplePricing> GetUnintegrated(List<int> unintegratedIDs) {
            CurtDevDataContext db = new CurtDevDataContext();
            List<SimplePricing> unintegrated = (from p in db.Parts
                                                join pr in db.Prices on p.partID equals pr.partID
                                                join cpr in db.CustomerPricings on p.partID equals cpr.partID into PricingTemp
                                                from cp in PricingTemp.DefaultIfEmpty()
                                                where unintegratedIDs.Contains(p.partID) && (cp.cust_id.Equals(this.cust_id) || (cp.cust_id == null || cp.cust_id == 0)) && pr.priceType.ToUpper().Equals("LIST")
                                                select new SimplePricing {
                                                    cust_id = this.cust_id,
                                                    partID = p.partID,
                                                    custPartID = 0,
                                                    price = (cp.price == null) ? pr.price1 : cp.price,
                                                    isSale = (cp.isSale == null) ? 0 : cp.isSale,
                                                    sale_start = Convert.ToDateTime(cp.sale_start).ToString(),
                                                    sale_end = Convert.ToDateTime(cp.sale_end).ToString(),
                                                    msrp = p.Prices.Where(x => x.priceType.Equals("List")).Select(x => x.price1).FirstOrDefault(),
                                                    map = p.Prices.Where(x => x.priceType.Equals("SuggestedMap")).Select(x => x.price1).FirstOrDefault(),
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
                    isSale = 0,
                    price = listPrice
                };
                db.CustomerPricings.InsertOnSubmit(pricePoint);
            } else {
                pricePoint.price = listPrice;
            }
            db.SubmitChanges();

            SimplePricing price = new SimplePricing {
                cust_id = this.cust_id,
                partID = pricePoint.partID,
                price = pricePoint.price,
                isSale = pricePoint.isSale,
                sale_start = ((pricePoint.sale_start != null) ? Convert.ToDateTime(pricePoint.sale_start).ToString() : ""),
                sale_end = ((pricePoint.sale_end != null) ? Convert.ToDateTime(pricePoint.sale_end).ToString() : "")
            };
            return price;
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

            decimal mapPrice = GetMap();

            var db = new CurtDevDataContext();
            CustomerPricing pricePoint = db.CustomerPricings.Where(x => x.partID.Equals(this.partID) && x.cust_id.Equals(this.cust_id)).FirstOrDefault<CustomerPricing>();
            if (pricePoint == null) {
                pricePoint = new CustomerPricing {
                    cust_id = this.cust_id,
                    partID = this.partID,
                    price = mapPrice,
                    isSale = 0
                };
                db.CustomerPricings.InsertOnSubmit(pricePoint);
            } else {
                pricePoint.price = mapPrice;
            }
            db.SubmitChanges();

            return new SimplePricing {
                cust_id = this.cust_id,
                partID = pricePoint.partID,
                price = pricePoint.price,
                isSale = pricePoint.isSale,
                sale_start = ((pricePoint.sale_start != null) ? Convert.ToDateTime(pricePoint.sale_start).ToString() : ""),
                sale_end = ((pricePoint.sale_end != null) ? Convert.ToDateTime(pricePoint.sale_end).ToString() : "")
            };
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
            decimal map = db.Prices.Where(x => x.priceType.ToUpper().Equals("SUGGESTEDMAP") && x.partID.Equals(this.partID)).Select(x => x.price1).FirstOrDefault<decimal>();
            if (this.price < map) {
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
            return db.Prices.Where(x => x.partID.Equals(this.partID) && x.priceType.ToUpper().Equals("SuggestedMap")).Select(x => x.price1).FirstOrDefault<decimal>();
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