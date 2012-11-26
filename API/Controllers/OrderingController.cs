using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API.Models;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace API.Controllers {
    public class OrderingController : Controller {
        //
        // GET: /Ordering/
        private static JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

        public void GetShipping(string dataType = "JSON") {
            ShippingResponse resp = Fedex.GetRate(dataType);
            if (dataType.ToUpper() == "JSON") {
                Response.ContentType = "application/json";
                Response.Write(jsSerializer.Serialize(resp));
                Response.End();
            } else if (dataType.ToUpper() == "JSONP") {
                Response.ContentType = "application/x-javascript";
                Response.Write(jsSerializer.Serialize(resp));
                Response.End();
            } else {
                XmlSerializer ser = new XmlSerializer(typeof(ShippingResponse));
                StringWriter xml = new StringWriter();
                ser.Serialize(xml,resp);
                Response.ContentType = "text/xml";
                Response.Write(xml);
                Response.End();
            }
        }

        #region Object Generators
        public void GenerateXMLAuth() {
            XmlSerializer ser = new XmlSerializer(typeof(FedExAuthentication));
            FedExAuthentication auth = new FedExAuthentication();
            StringWriter xml = new StringWriter();
            ser.Serialize(xml, auth);
            Response.ContentType = "text/xml";
            Response.Write(xml);
            Response.End();
        }

        public string GenerateJSONAuth() {
            return jsSerializer.Serialize(new FedExAuthentication());
        }

        public void GenerateXMLAddress() {
            XmlSerializer ser = new XmlSerializer(typeof(Address));
            string[] lines = { "6208 Industrial Dr." };
            Address add = new Address { 
                City = "Eau Claire",
                CountryCode = "US",
                PostalCode = "54701",
                Residential = true,
                ResidentialSpecified = true,
                StateOrProvinceCode = "WI",
                StreetLines = lines
            };
            
            StringWriter xml = new StringWriter();
            ser.Serialize(xml, add);
            Response.ContentType = "text/xml";
            Response.Write(xml);
            Response.End();
        }

        public string GenerateJSONAddress() {
            string[] lines = { "6208 Industrial Dr." };
            Address add = new Address {
                City = "Eau Claire",
                CountryCode = "US",
                PostalCode = "54701",
                Residential = true,
                ResidentialSpecified = true,
                StateOrProvinceCode = "WI",
                StreetLines = lines
            };
            return jsSerializer.Serialize(add);
        }

        public void GenerateXMLPartList() {
            XmlSerializer ser = new XmlSerializer(typeof(List<int>));
            List<int> list = new List<int>();
            list.Add(11000);
            list.Add(13333);
            StringWriter xml = new StringWriter();
            ser.Serialize(xml, list);
            Response.ContentType = "text/xml";
            Response.Write(xml);
            Response.End();
        }

        public string GenerateJSONPartList() {
            List<int> list = new List<int>();
            list.Add(11000);
            list.Add(13333);
            return jsSerializer.Serialize(list);
        }

        public void GenerateXMLServiceTypes() {
            List<string> types = new List<string>();
            foreach (ServiceType type in Enum.GetValues(typeof(ServiceType))) {
                types.Add(type.ToString());
            }
            XmlSerializer ser = new XmlSerializer(types.GetType());
            StringWriter xml = new StringWriter();
            ser.Serialize(xml, types);
            Response.ContentType = "text/xml";
            Response.Write(xml);
            Response.End();
        }

        public string GenerateJSONServiceTypes() {
            List<string> types = new List<string>();
            foreach (ServiceType type in Enum.GetValues(typeof(ServiceType))) {
                types.Add(type.ToString());
            }
            return jsSerializer.Serialize(types);
        }

        public void GenerateXMLPackageTypes() {
            List<string> types = new List<string>();
            foreach (PackagingType type in Enum.GetValues(typeof(PackagingType))) {
                types.Add(type.ToString());
            }
            XmlSerializer ser = new XmlSerializer(types.GetType());
            StringWriter xml = new StringWriter();
            ser.Serialize(xml, types);
            Response.ContentType = "text/xml";
            Response.Write(xml);
            Response.End();
        }

        public string GenerateJSONPackageTypes() {
            List<string> types = new List<string>();
            foreach (PackagingType type in Enum.GetValues(typeof(PackagingType))) {
                types.Add(type.ToString());
            }
            return jsSerializer.Serialize(types);
        }
        #endregion
    }
}
