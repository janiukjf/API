using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API.Models;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Xml;

namespace API {
    partial class CartIntegration {

        public List<CartIntegration> GetAll(string key) {
            Authenticate(key);
            CurtDevDataContext db = new CurtDevDataContext();

            List<CartIntegration> integrated = (from c in db.Parts
                                                join ci in db.CartIntegrations on c.partID equals ci.partID into IntegrationTemp
                                                from cit in IntegrationTemp.DefaultIfEmpty()
                                                where cit.custID.Equals(this.custID)
                                                select new CartIntegration {
                                                    custID = this.custID,
                                                    partID = c.partID,
                                                    custPartID = cit.custPartID,
                                                    referenceID = cit.referenceID
                                                }).ToList<CartIntegration>();
            List<int> integratedID = integrated.Select(x => x.partID).ToList<int>();

            List<CartIntegration> unintegrated = (from p in db.Parts
                                                where !integratedID.Contains(p.partID)
                                                select new CartIntegration {
                                                    custID = this.custID,
                                                    partID = p.partID,
                                                    custPartID = 0,
                                                    referenceID = 0
                                                }).ToList<CartIntegration>();
            integrated.AddRange(unintegrated);
            return integrated.OrderBy(x => x.partID).ToList<CartIntegration>();

        }

        private List<int> getUnintegratedIDs() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<int> statuslist = new List<int> { 800, 900 };
            List<int> idlist = (from p in db.Parts
                                where statuslist.Contains(p.status) && 
                                    !(from c in db.Parts
                                     join ci in db.CartIntegrations on c.partID equals ci.partID into IntegrationTemp
                                     from cit in IntegrationTemp.DefaultIfEmpty()
                                     where cit.custID.Equals(this.custID) && (cit.custPartID != null || cit.custPartID != 0)
                                     select c.partID).Contains(p.partID)
                                select p.partID).ToList<int>();

            return idlist;
        }

        public string GetUnintegratedPartsJSON() {
            CurtDevDataContext db = new CurtDevDataContext();
            List<APIPart> parts = new List<APIPart>();

            List<int> unintegratedIDs = getUnintegratedIDs();

            while (unintegratedIDs.Count > 0) {
                List<APIPart> tempparts = new List<APIPart>();
                List<int> block = new List<int>();
                if (unintegratedIDs.Count > 2000) {
                    block = unintegratedIDs.Take(2000).ToList<int>();
                } else {
                    block = unintegratedIDs;
                }

                tempparts = (from p in db.Parts
                         join c in db.Classes on p.classID equals c.classID into ClassTemp
                         from c in ClassTemp.DefaultIfEmpty()
                         where block.Contains(p.partID)
                         select new APIPart {
                             partID = p.partID,
                             status = p.status,
                             dateModified = Convert.ToDateTime(p.dateModified).ToString(),
                             dateAdded = Convert.ToDateTime(p.dateAdded).ToString(),
                             shortDesc = "CURT " + p.shortDesc + " #" + p.partID.ToString(),
                             oldPartNumber = p.oldPartNumber,
                             listPrice = String.Format("{0:C}", (from prices in db.Prices
                                                                 where prices.partID.Equals(p.partID) && prices.priceType.Equals("List")
                                                                 select prices.price1 != null ? prices.price1 : (decimal?)0).FirstOrDefault<decimal?>()),
                             pClass = (c != null) ? c.class1 : "",
                             priceCode = p.priceCode,
                             relatedCount = db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count(),
                             attributes = (from pa in db.PartAttributes
                                           where pa.partID.Equals(p.partID)
                                           orderby pa.sort
                                           select new APIAttribute { key = pa.field, value = pa.value }).ToList<APIAttribute>(),
                             content = (from co in p.ContentBridges
                                        orderby co.contentID
                                        select new APIAttribute { key = co.Content.ContentType.type, value = co.Content.text }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             videos = (from pv in p.PartVideos
                                       orderby pv.vTypeID
                                       select new APIVideo { videoID = pv.pVideoID, youTubeVideoID = pv.video, isPrimary = pv.isPrimary, typeID = pv.vTypeID, type = pv.videoType.name, typeicon = pv.videoType.icon }).ToList<APIVideo>(),
                             pricing = (from pr in db.Prices
                                        where pr.partID.Equals(p.partID)
                                        select new APIAttribute { key = pr.priceType, value = pr.price1.ToString() }).OrderBy(x => x.key).ToList<APIAttribute>(),
                             reviews = (from r in db.Reviews
                                        where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                        orderby r.createdDate descending
                                        select new APIReview { reviewID = r.reviewID, partID = (r.partID != null) ? (int)r.partID : 0, rating = r.rating, subject = r.subject, review_text = r.review_text, email = r.email, name = r.name, createdDate = String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate) }).Take(10).ToList<APIReview>(),
                             averageReview = (from r in db.Reviews
                                              where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                              select (double?)r.rating).Average() ?? 0.0,
                             images = (from pi in db.PartImages
                                       where pi.partID.Equals(p.partID)
                                       select new APIImage {
                                           imageID = pi.imageID,
                                           sort = pi.sort,
                                           path = pi.path,
                                           height = pi.height,
                                           width = pi.width,
                                           size = db.PartImageSizes.Where(x => x.sizeID.Equals(pi.sizeID)).Select(x => x.size).FirstOrDefault<string>(),
                                           partID = pi.partID
                                       }).OrderBy(x => x.sort).ToList<APIImage>()
                         }).Distinct().OrderBy(x => x.partID).ToList<APIPart>();
                parts.AddRange(tempparts);
                unintegratedIDs.RemoveRange(0, block.Count);
            }
            return JsonConvert.SerializeObject(parts);
        }

        public XDocument GetUnintegratedPartsXML() {
            XDocument xml = new XDocument();
            XElement parts = new XElement("Parts");

            //List<APIPart> parts = new List<APIPart>();
            CurtDevDataContext db = new CurtDevDataContext();

            List<int> unintegratedIDs = getUnintegratedIDs();

            while (unintegratedIDs.Count > 0) {
                List<int> block = new List<int>();
                List<XElement> xelements = new List<XElement>();

                if (unintegratedIDs.Count > 2000) {
                    block = unintegratedIDs.Take(2000).ToList<int>();
                } else {
                    block = unintegratedIDs;
                }

                List<XElement> tempparts = (from p in db.Parts
                                                join c in db.Classes on p.classID equals c.classID into ClassTemp
                                                from c in ClassTemp.DefaultIfEmpty()
                                                where block.Contains(p.partID)
                                                orderby p.partID
                                                select new XElement("Part",
                                                        new XAttribute("partID", p.partID),
                                                        new XAttribute("status", p.status),
                                                        new XAttribute("dateModified", Convert.ToDateTime(p.dateModified).ToString()),
                                                        new XAttribute("dateAdded", Convert.ToDateTime(p.dateAdded).ToString()),
                                                        new XAttribute("shortDesc", "CURT " + p.shortDesc + " #" + p.partID.ToString()),
                                                        new XAttribute("oldPartNumber", (p.oldPartNumber != null) ? p.oldPartNumber : ""),
                                                        new XAttribute("listPrice", String.Format("{0:C}", (from prices in db.Prices
                                                                                                            where prices.partID.Equals(p.partID) && prices.priceType.Equals("List")
                                                                                                            select prices.price1 != null ? prices.price1 : (decimal?)0).FirstOrDefault<decimal?>())),
                                                        new XAttribute("pClass", (c != null) ? c.class1 : ""),
                                                        new XAttribute("priceCode", (p.priceCode != null) ? p.priceCode : 0),
                                                        new XAttribute("relatedCount", db.RelatedParts.Where(x => x.partID == p.partID).Select(x => new { relatedID = x.relatedID }).Count()),
                                                        new XElement("Attributes", (from pa in db.PartAttributes
                                                                                    where pa.partID.Equals(p.partID)
                                                                                    orderby pa.sort
                                                                                    select new XElement(XmlConvert.EncodeName(pa.field), pa.value)).ToList<XElement>()),
                                                        new XElement("Content", (from co in p.ContentBridges
                                                                                orderby co.Content.ContentType.type, co.contentID
                                                                                select new XElement(XmlConvert.EncodeName(co.Content.ContentType.type), co.Content.text)).ToList<XElement>()),
                                                        new XElement("Videos", (from vp in p.PartVideos
                                                                                orderby vp.vTypeID
                                                                                select new XElement("Video", vp.video,
                                                                                    new XAttribute("videoID", vp.pVideoID),
                                                                                    new XAttribute("isPrimary", vp.isPrimary),
                                                                                    new XAttribute("type", vp.videoType.name),
                                                                                    new XAttribute("icon", vp.videoType.icon ?? ""),
                                                                                    new XAttribute("typeID", vp.vTypeID)
                                                                                )).ToList<XElement>()),
                                                        new XElement("Pricing", (from pr in db.Prices
                                                                                where pr.partID.Equals(p.partID)
                                                                                orderby pr.priceType
                                                                                select new XElement(XmlConvert.EncodeName(pr.priceType), pr.price1.ToString())).ToList<XElement>()),
                                                        new XElement("Reviews",
                                                            new XAttribute("averageReview", (from r in db.Reviews
                                                                                                where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                                select (double?)r.rating).Average() ?? 0.0),
                                                                                (from r in db.Reviews
                                                                                    where r.partID.Equals(p.partID) && r.active.Equals(true) && r.approved.Equals(true)
                                                                                    orderby r.createdDate descending
                                                                                    select new XElement("Review",
                                                                                        new XAttribute("reviewID", r.reviewID),
                                                                                        new XElement("createdDate", String.Format("{0:MM/dd/yyyy hh:mm tt}", r.createdDate)),
                                                                                        new XElement("rating", r.rating),
                                                                                        new XElement("name", r.name),
                                                                                        new XElement("email", r.email),
                                                                                        new XElement("subject", r.subject),
                                                                                        new XElement("review_text", r.review_text))).Take(10).ToList<XElement>()),
                                                        new XElement("Images",
                                                                            (from pin in db.PartImages
                                                                                where pin.partID.Equals(p.partID)
                                                                                group pin by pin.sort into pi
                                                                                orderby pi.Key
                                                                                select new XElement("Index",
                                                                                                    new XAttribute("name", pi.Key.ToString()),
                                                                                                    (from pis in db.PartImages
                                                                                                    join pig in db.PartImageSizes on pis.sizeID equals pig.sizeID
                                                                                                    where pis.partID.Equals(p.partID) && pis.sort.Equals(pi.Key)
                                                                                                    orderby pis.sort, pis.sizeID
                                                                                                    select new XElement(pig.size,
                                                                                                                        new XAttribute("imageID", pis.imageID),
                                                                                                                        new XAttribute("path", pis.path),
                                                                                                                        new XAttribute("height", pis.height),
                                                                                                                        new XAttribute("width", pis.width)
                                                                                                    )).ToList<XElement>()
                                                                            )).ToList<XElement>())
                                                    )).ToList<XElement>();
                parts.Add(tempparts);
                unintegratedIDs.RemoveRange(0, block.Count);
            }
            xml.Add(parts);
            return xml;
        }

        public CartIntegration Set(string key) {
            Authenticate(key);
            CurtDevDataContext db = new CurtDevDataContext();

            // Validate required fields

            // This isn't working because the customer needs to be able to unlink
            // if (this.custPartID == 0) { throw new Exception("Customer Part ID failed to validate against null or zero."); }
            //  - ajn
            if (this.partID == 0) { throw new Exception("Part Number failed to validate against null or zero."); }

            // Attempt to get a CustomerPricing record for this customerID and partID
            List<CartIntegration> tmpIntegrations = db.CartIntegrations.Where(x => x.custID.Equals(this.custID) && x.partID.Equals(this.partID)).ToList<CartIntegration>();
            CartIntegration newintegration = new CartIntegration();
            List<CartIntegration> deleteables = new List<CartIntegration>();
            bool updated = false;
            foreach (CartIntegration tmpIntegration in tmpIntegrations) {
                if (tmpIntegration.custPartID == 0) {
                    deleteables.Add(tmpIntegration);
                } else {
                    if (!updated && this.custPartID > 0) {
                        tmpIntegration.custPartID = this.custPartID;
                        updated = true;
                    } else {
                        deleteables.Add(tmpIntegration);
                    }
                }
            }
            if (!updated && this.custPartID > 0) {
                newintegration = this;
                newintegration.custID = this.custID;
                db.CartIntegrations.InsertOnSubmit(newintegration);
            }
            if (deleteables.Count > 0) {
                db.CartIntegrations.DeleteAllOnSubmit(deleteables);
            }
            db.SubmitChanges();

            CartIntegration cartIntegration = new CartIntegration {
                custID = this.custID,
                partID = newintegration.partID,
                custPartID = newintegration.custPartID,
                referenceID = newintegration.referenceID
            };
            return cartIntegration;
        }


        internal void Authenticate(string key) {
            if (key == null || key.Length == 0) { throw new Exception("Invalid API key."); }
            if (this.custID == 0) { throw new Exception("Invalid customerID"); }
            Guid api_key;
            try {
                api_key = Guid.Parse(key);
            } catch {
                throw new Exception("Invalid API key.");
            }

            CurtDevDataContext db = new CurtDevDataContext();
            Customer cust = db.Customers.Where(x => x.customerID.Equals(this.custID) & x.APIKey.Equals(api_key)).FirstOrDefault<Customer>();
            if (cust == null || cust.cust_id == 0) {
                throw new Exception("Denied Access.");
            }
        }

    }
}