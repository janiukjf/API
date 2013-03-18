using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API.Models;
using System.Web.Services.Protocols;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.IO;

namespace API.Models {
    public class Fedex {

        // Required Fields
        private static Address _origin = new Address();
        private static Address _destination = new Address();
        private static List<int> _parts = new List<int>();
        private static FedExAuthentication _auth = new FedExAuthentication();

        // Optional Fields
        private static string _shipType = "";
        private static string _packageType = "";
        private static bool _insure = false;
        private static decimal _insureAmount = 0;
        private static int _packageCount = 0;
        private static int _customer = 0;
        private static string _environment = "production";

        // Global tools
        private static JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

        public static ShippingResponse GetRate(string dataType = "json") {
            try {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") {
                    ValidateJSON();
                } else {
                    ValidateXML();
                }

                RateRequest standardRequest = CreateRateRequest(_auth);
                RateRequest freightRequest = CreateRateRequest(_auth);
                standardRequest.RequestedShipment.FreightShipmentDetail = null;
                freightRequest.RequestedShipment.RequestedPackageLineItems = null;
                RateService service = new RateService(_environment); // Initializes the service
                try {
                    // Call the web service passing in a RateRequest and returing a RateReply
                    decimal totalweight = standardRequest.RequestedShipment.RequestedPackageLineItems.Sum(x => x.Weight.Value);
                    RateReply reply = new RateReply();
                    if (totalweight < 90) {
                        reply = service.getRates(standardRequest);
                    } else if (totalweight >= 150) {
                        //reply = service.getRates(freightRequest);
                        reply = service.getRates(standardRequest);
                    } else {
                        // make both calls
                        reply = service.getRates(standardRequest);
                        /*RateReply freightreply = service.getRates(freightRequest);
                        
                        List<Notification> allnotifications = new List<Notification>();
                        List<RateReplyDetail> alldetails = new List<RateReplyDetail>();

                        if (standardreply.HighestSeverity == NotificationSeverityType.SUCCESS || standardreply.HighestSeverity == NotificationSeverityType.NOTE || standardreply.HighestSeverity == NotificationSeverityType.WARNING) {
                            reply.HighestSeverity = standardreply.HighestSeverity;
                            allnotifications.AddRange(standardreply.Notifications.ToList<Notification>());
                            alldetails.AddRange(standardreply.RateReplyDetails.ToList<RateReplyDetail>());
                            reply.TransactionDetail = standardreply.TransactionDetail;
                            reply.Version = standardreply.Version;
                        }

                        if (freightreply.HighestSeverity == NotificationSeverityType.SUCCESS || freightreply.HighestSeverity == NotificationSeverityType.NOTE || freightreply.HighestSeverity == NotificationSeverityType.WARNING) {
                            reply.HighestSeverity = freightreply.HighestSeverity;
                            allnotifications.AddRange(freightreply.Notifications.ToList<Notification>());
                            alldetails.AddRange(freightreply.RateReplyDetails.ToList<RateReplyDetail>());
                            reply.TransactionDetail = freightreply.TransactionDetail;
                            reply.Version = freightreply.Version;
                        }
                        reply.Notifications = allnotifications.ToArray<Notification>();
                        reply.RateReplyDetails = alldetails.ToArray<RateReplyDetail>();*/

                    }
                    
                    ShippingResponse resp = new ShippingResponse();


                    // Check if the call was successful
                    if (reply.HighestSeverity == NotificationSeverityType.SUCCESS || reply.HighestSeverity == NotificationSeverityType.NOTE || reply.HighestSeverity == NotificationSeverityType.WARNING) {
                        List<ShipmentRateDetails> details = new List<ShipmentRateDetails>();
                        details = GetRateDetails(reply);
                        resp = new ShippingResponse {
                            Status = "OK",
                            Status_Description = "",
                            Result = details
                        };
                    } else {
                        List<ShipmentRateDetails> details = new List<ShipmentRateDetails>();
                        ShipmentRateDetails detail = new ShipmentRateDetails();
                        detail.Notifications = ShowNotifications(reply);
                        details.Add(detail);
                        resp = new ShippingResponse {
                            Status = "ERROR",
                            Status_Description = "",
                            Result = details
                        };
                    }
                    return resp;
                } catch (SoapException e) {
                    throw new Exception(e.Detail.InnerText);
                } catch (Exception e) {
                    throw new Exception(e.Message);
                }
            } catch (Exception e) {
                ShippingResponse resp2 = new ShippingResponse { 
                    Status = "ERROR",
                    Status_Description = e.Message,
                    Result = null
                };
                return resp2;
            }
        }

        private static RateRequest CreateRateRequest(FedExAuthentication auth) {
            // Build a RateRequest
            RateRequest req = new RateRequest();

            req.WebAuthenticationDetail = new WebAuthenticationDetail();
            req.WebAuthenticationDetail.UserCredential = new WebAuthenticationCredential();
            req.WebAuthenticationDetail.UserCredential.Key = auth.Key;
            req.WebAuthenticationDetail.UserCredential.Password = auth.Password;

            req.ClientDetail = new ClientDetail();
            req.ClientDetail.AccountNumber = auth.AccountNumber.ToString();
            req.ClientDetail.MeterNumber = auth.MeterNumber.ToString();

            req.TransactionDetail = new TransactionDetail();
            req.TransactionDetail.CustomerTransactionId = (auth.CustomerTransactionId != null) ? auth.CustomerTransactionId : "***Rate v10 Request using VC#***";

            req.Version = new VersionId();

            req.ReturnTransitAndCommit = true;
            req.ReturnTransitAndCommitSpecified = true;

            SetShipmentDetails(req);

            return req;
        }

        private static void SetShipmentDetails(RateRequest r) {
            r.RequestedShipment = new RequestedShipment();
            r.RequestedShipment.ShipTimestamp = DateTime.Now;
            r.RequestedShipment.ShipTimestampSpecified = true;
            r.RequestedShipment.DropoffType = DropoffType.REGULAR_PICKUP;
            r.RequestedShipment.DropoffTypeSpecified = true;
            r.RequestedShipment.PackagingType = PackagingType.YOUR_PACKAGING;
            r.RequestedShipment.PackagingTypeSpecified = true;
            if (_packageType != null) {
                foreach (PackagingType type in Enum.GetValues(typeof(PackagingType))) {
                    if (_packageType.ToUpper() == type.ToString()) {
                        r.RequestedShipment.PackagingType = type;
                    }
                }
            }
            r.RequestedShipment.PackagingTypeSpecified = true;
            r.RequestedShipment.FreightShipmentDetail = new FreightShipmentDetail();
            r.RequestedShipment.FreightShipmentDetail.Role = FreightShipmentRoleType.SHIPPER;

            SetOrigin(r);
            SetDestination(r);
            SetPackageLineItems(r);

            //r.RequestedShipment.ServiceType = ServiceType.GROUND_HOME_DELIVERY;
            /*if (_shipType != null && _shipType != "") { // Set the Service Type to a dynamically passed option
                if (Enum.IsDefined(typeof(ServiceType), _shipType)) {
                    r.RequestedShipment.ServiceType = (ServiceType)Enum.Parse(typeof(ServiceType), _shipType, true);
                }
            }
            r.RequestedShipment.ServiceTypeSpecified = true;*/

            if (_insure) {
                r.RequestedShipment.TotalInsuredValue = new Money();
                r.RequestedShipment.TotalInsuredValue.Amount = _insureAmount;
                r.RequestedShipment.TotalInsuredValue.Currency = "USD";
            }

            r.RequestedShipment.RateRequestTypes = new RateRequestType[1];
            r.RequestedShipment.RateRequestTypes[0] = RateRequestType.ACCOUNT;
            //r.RequestedShipment.RateRequestTypes[0] = RateRequestType.LIST;
            r.RequestedShipment.PackageCount = r.RequestedShipment.RequestedPackageLineItems.Count().ToString();
            
        }

        private static void SetOrigin(RateRequest r) {
            r.RequestedShipment.Shipper = new Party();
            //r.RequestedShipment.Shipper.Address = _origin;
            r.RequestedShipment.Shipper.Address = new Address();
            r.RequestedShipment.Shipper.Address.PostalCode = _origin.PostalCode;
            r.RequestedShipment.Shipper.Address.CountryCode = _origin.CountryCode;
            r.RequestedShipment.Shipper.Address.Residential = false;
            r.RequestedShipment.Shipper.Address.ResidentialSpecified = true;
        }

        private static void SetDestination(RateRequest r) {
            r.RequestedShipment.Recipient = new Party();
            //r.RequestedShipment.Recipient.Address = _destination;
            r.RequestedShipment.Recipient.Address = new Address();
            r.RequestedShipment.Recipient.Address.PostalCode = _destination.PostalCode;
            r.RequestedShipment.Recipient.Address.CountryCode = _destination.CountryCode;
            r.RequestedShipment.Recipient.Address.Residential = _destination.Residential;
            r.RequestedShipment.Recipient.Address.ResidentialSpecified = true;
        }

        private static void SetPackageLineItems(RateRequest r) {

            // Get the parts from the database
            List<PartPackage> items = new List<PartPackage>();
            CurtDevDataContext db = new CurtDevDataContext();
            foreach (int part in _parts) {
                // package type ID 1 = shipping box
                List<PartPackage> ppackages = db.PartPackages.Where(x => x.partID.Equals(part) && x.typeID.Equals(1)).ToList<PartPackage>();
                foreach (PartPackage ppackage in ppackages) {
                    for (int p = 0; p < ppackage.quantity; p++) {
                        items.Add(ppackage);
                    }
                }
            }
            List<RequestedPackageLineItem> packageitems = new List<RequestedPackageLineItem>();
            List<FreightShipmentLineItem> freightitems = new List<FreightShipmentLineItem>();
            //Weight TotalWeight = new Weight();
            //TotalWeight.Units = WeightUnits.LB;
            //TotalWeight.Value = 0;
            for (int i = 0; i < items.Count; i++) {
                Weight pweight = null;
                Dimensions pdims = null;
                Volume pvolume = null;

                // package weight
                if (items[i].weight != null && items[i].weight > 0) {
                    pweight = new Weight {
                        Units = WeightUnits.LB,
                        Value = (decimal)items[i].weight
                    };
                }
                if (items[i].height != null && items[i].height > 0 && items[i].width != null && items[i].width > 0 && items[i].length != null && items[i].length > 0) {
                    pdims = new Dimensions {
                        Height = Math.Ceiling((double)items[i].height).ToString(),
                        Width = Math.Ceiling((double)items[i].width).ToString(),
                        Length = Math.Ceiling((double)items[i].length).ToString(),
                        Units = LinearUnits.IN
                    };

                    pvolume = new Volume {
                        Units = VolumeUnits.CUBIC_FT,
                        UnitsSpecified = true,
                        Value = Convert.ToDecimal((items[i].height * items[i].width * items[i].length) / 1728),
                        ValueSpecified = true
                    };

                }
                RequestedPackageLineItem rpackage = new RequestedPackageLineItem {
                    SequenceNumber = (packageitems.Count() + 1).ToString(),
                    GroupPackageCount = "1",
                    Weight = pweight,
                    Dimensions = pdims
                };

                FreightShipmentLineItem rfreight = new FreightShipmentLineItem {
                    Dimensions = pdims,
                    Weight = pweight,
                    Volume = pvolume,
                    Packaging = PhysicalPackagingType.BOX,
                    PackagingSpecified = true,
                    FreightClass = getFreightClass(Convert.ToDouble(pweight.Value / pvolume.Value)),
                    FreightClassSpecified = true
                };
                packageitems.Add(rpackage);
                freightitems.Add(rfreight);
            }
            r.RequestedShipment.RequestedPackageLineItems = packageitems.ToArray<RequestedPackageLineItem>();
            r.RequestedShipment.FreightShipmentDetail.LineItems = freightitems.ToArray<FreightShipmentLineItem>();
            //r.RequestedShipment.TotalWeight = TotalWeight;
        }

        private static FreightClassType getFreightClass(double density) {
            if (density < 1) {
                return FreightClassType.CLASS_500;
            } else if (density >= 1 && density < 2) {
                return FreightClassType.CLASS_400;
            } else if (density >= 2 && density < 3) {
                return FreightClassType.CLASS_300;
            } else if (density >= 3 && density < 4) {
                return FreightClassType.CLASS_250;
            } else if (density >= 4 && density < 5) {
                return FreightClassType.CLASS_200;
            } else if (density >= 5 && density < 6) {
                return FreightClassType.CLASS_175;
            } else if (density >= 6 && density < 7) {
                return FreightClassType.CLASS_150;
            } else if (density >= 7 && density < 8) {
                return FreightClassType.CLASS_125;
            } else if (density >= 8 && density < 9) {
                return FreightClassType.CLASS_110;
            } else if (density >= 9 && density < 10.5) {
                return FreightClassType.CLASS_100;
            } else if (density >= 10.5 && density < 12) {
                return FreightClassType.CLASS_092_5;
            } else if (density >= 12 && density < 13.5) {
                return FreightClassType.CLASS_085;
            } else if (density >= 13.5 && density < 15) {
                return FreightClassType.CLASS_077_5;
            } else if (density >= 15 && density < 22.5) {
                return FreightClassType.CLASS_070;
            } else if (density >= 22.5 && density < 30) {
                return FreightClassType.CLASS_065;
            } else if (density >= 30 && density < 35) {
                return FreightClassType.CLASS_055;
            } else {
                return FreightClassType.CLASS_050;
            }
        }

        private static List<ShipmentRateDetails> GetRateDetails(RateReply reply) {
            List<ShipmentRateDetails> details = new List<ShipmentRateDetails>();
            if (reply.RateReplyDetails != null) {
                foreach (RateReplyDetail replyDetail in reply.RateReplyDetails) {
                    ShipmentRateDetails detail = new ShipmentRateDetails();
                    if (replyDetail.ServiceTypeSpecified) {
                        detail.ServiceType = replyDetail.ServiceType.ToString();
                    }
                    if (replyDetail.PackagingTypeSpecified) {
                        detail.PackagingType = replyDetail.PackagingType.ToString();
                    }
                    List<RateDetail> rateDetails = new List<RateDetail>();
                    foreach (RatedShipmentDetail shipmentDetail in replyDetail.RatedShipmentDetails) {
                        ShipmentRateDetail shipDetail = shipmentDetail.ShipmentRateDetail;
                        RateDetail rateDetail = new RateDetail();
                        rateDetail.RateType = shipDetail.RateType.ToString();
                        if (shipDetail.TotalBillingWeight != null) {
                            rateDetail.TotalBillingWeight = new KeyValuePair<decimal, string>(shipDetail.TotalBillingWeight.Value, shipmentDetail.ShipmentRateDetail.TotalBillingWeight.Units.ToString());
                        }
                        if (shipDetail.TotalBaseCharge != null) {
                            rateDetail.TotalBaseCharge = new KeyValuePair<decimal, string>(shipDetail.TotalBaseCharge.Amount, shipDetail.TotalBaseCharge.Currency);
                        }
                        if (shipDetail.TotalFreightDiscounts != null) {
                            rateDetail.TotalFreightDiscounts = new KeyValuePair<decimal, string>(shipDetail.TotalFreightDiscounts.Amount, shipDetail.TotalFreightDiscounts.Currency);
                        }
                        if (shipDetail.TotalSurcharges != null) {
                            rateDetail.TotalFreightDiscounts = new KeyValuePair<decimal, string>(shipDetail.TotalSurcharges.Amount, shipDetail.TotalSurcharges.Currency);
                        }
                        if (shipDetail.Surcharges != null) {
                            List<ShippingSurcharge> shippingSurcharges = new List<ShippingSurcharge>();
                            foreach (Surcharge detailSurcharge in shipDetail.Surcharges) {
                                ShippingSurcharge surcharge = new ShippingSurcharge {
                                    Type = detailSurcharge.SurchargeType.ToString(),
                                    Charge = new KeyValuePair<decimal, string>(detailSurcharge.Amount.Amount, detailSurcharge.Amount.Currency)
                                };
                                shippingSurcharges.Add(surcharge);
                            }
                            rateDetail.Surcharges = shippingSurcharges;
                        }
                        if (shipDetail.TotalNetCharge != null) {
                            rateDetail.NetCharge = new KeyValuePair<decimal, string>(shipDetail.TotalNetCharge.Amount, shipDetail.TotalNetCharge.Currency);
                        }
                        rateDetails.Add(rateDetail);
                    }
                    detail.Rates = rateDetails;
                    detail.TransitTime = replyDetail.TransitTime.ToString();
                    detail.Notifications = ShowNotifications(reply);
                    details.Add(detail);
                }
            }

            
            return details;
        }

        private static List<ShippingNotification> ShowNotifications(RateReply reply) {
            // Add Notifications
            List<ShippingNotification> notifs = new List<ShippingNotification>();
            for (int i = 0; i < reply.Notifications.Count(); i++) {
                Notification n = reply.Notifications[i];
                ShippingNotification notif = new ShippingNotification {
                    NotificationID = i,
                    Severity = n.Severity.ToString(),
                    Code = n.Code,
                    Message = n.Message,
                    Source = n.Source
                };
                notifs.Add(notif);
            }
            return notifs;
        }

        private static void ValidateJSON() {
            try {
                string origin = HttpContext.Current.Request.Form["origin"];
                string destination = HttpContext.Current.Request.Form["destination"];
                string parts = HttpContext.Current.Request.Form["parts"];
                string auth = HttpContext.Current.Request.Form["auth"];
                string environment = HttpContext.Current.Request.Form["environment"];
                string test = environment;

                // Validate required fields
                if (origin == null || destination == null || parts == null || auth == null) {
                    throw new Exception();
                }
                if (environment != null) {
                    try {
                        _environment = environment;
                    } catch { }
                }
                try {
                    _origin = jsSerializer.Deserialize<Address>(origin);
                    _destination = jsSerializer.Deserialize<Address>(destination);
                    _parts = jsSerializer.Deserialize<List<int>>(parts);
                    _auth = jsSerializer.Deserialize<FedExAuthentication>(auth);
                } catch (Exception) { throw new Exception(); }

                // Parse out optional fields, hopefully without breaking anything
                try {
                    _customer = Convert.ToInt32(HttpContext.Current.Request.Form["customer"]);
                } catch (Exception) { }
                try {
                    _shipType = HttpContext.Current.Request.Form["shipment-type"];
                } catch (Exception) { }
                try {
                    _packageType = HttpContext.Current.Request.Form["package-type"];
                } catch (Exception) { }
                try {
                    _insure = Convert.ToBoolean(HttpContext.Current.Request.Form["insure"]);
                } catch (Exception) { }
                try {
                    _insureAmount = Convert.ToDecimal(HttpContext.Current.Request.Form["insure-amount"]);
                } catch (Exception) { }
                try{
                    _packageCount = Convert.ToInt32(HttpContext.Current.Request.Form["package-count"]);
                }catch(Exception){}

            } catch (Exception) {
                throw new Exception("INVALID_REQUEST");
            }
        }

        private static void ValidateXML() {
            try {
                string origin = HttpContext.Current.Request.Form["origin"];
                string destination = HttpContext.Current.Request.Form["destination"];
                string parts = HttpContext.Current.Request.Form["parts"];
                string auth = HttpContext.Current.Request.Form["auth"];
                string environment = HttpContext.Current.Request.Form["environment"];

                // Validate required fields
                if (origin == null || destination == null || parts == null || auth == null) {
                    throw new Exception();
                }
                if (environment != null) {
                    try {
                        _environment = environment;
                    } catch {  }
                }
                try {
                    XmlSerializer addressSerializer = new XmlSerializer(typeof(Address));
                    XmlSerializer partsSerializer = new XmlSerializer(typeof(List<int>));
                    XmlSerializer authSerializer = new XmlSerializer(typeof(FedExAuthentication));
                    _origin = (Address)addressSerializer.Deserialize(new StringReader(origin));
                    _destination = (Address)addressSerializer.Deserialize(new StringReader(destination));
                    _parts = (List<int>)partsSerializer.Deserialize(new StringReader(parts));
                    _auth = (FedExAuthentication)authSerializer.Deserialize(new StringReader(auth));
                } catch (Exception) { throw new Exception(); }

                // Parse out optional fields, hopefully without breaking anything
                try {
                    _customer = Convert.ToInt32(HttpContext.Current.Request.Form["customer"]);
                } catch (Exception) { }
                try {
                    _shipType = HttpContext.Current.Request.Form["shipment-type"];
                } catch (Exception) { }
                try {
                    _packageType = HttpContext.Current.Request.Form["package-type"];
                } catch (Exception) { }
                try {
                    _insure = Convert.ToBoolean(HttpContext.Current.Request.Form["insure"]);
                } catch (Exception) { }
                try {
                    _insureAmount = Convert.ToDecimal(HttpContext.Current.Request.Form["insure-amount"]);
                } catch (Exception) { }
                try {
                    _packageCount = Convert.ToInt32(HttpContext.Current.Request.Form["package-count"]);
                } catch (Exception) { }
            } catch (Exception) {
                throw new Exception("INVALID_REQUEST");
            }
        }

        public static string SerializeAddress() {
            Address add = new Address();
            string[] lines = { "1120 Sunset Lane", "Apt. 2" };
            add.StreetLines = lines;
            add.City = "Altoona";
            add.StateOrProvinceCode = "WI";
            add.PostalCode = "54720";
            add.CountryCode = "US";
            add.Residential = true;
            add.ResidentialSpecified = true;
            return jsSerializer.Serialize(add);
        }

    }

    public class ShipmentAddress {
        public string[] StreetLines { get; set; } // 1120 Sunset Lane
        public string City { get; set; } // Altoona
        public string StateProvidenceCode { get; set; } // WI
        public string PostalCode { get; set; } // Must be a string for Canadian Postal Codes i.e 54720
        public string CountryCode { get; set; } // US
    }

    public class FedExAuthentication {
        public string Key { get; set; }
        public string Password { get; set; }
        public int AccountNumber { get; set; }
        public int MeterNumber { get; set; }
        public string CustomerTransactionId { get; set; }
    }

    public class ShippingResponse {
        public string Status { get; set; }
        public string Status_Description { get; set; }
        public List<ShipmentRateDetails> Result { get; set; }
    }

    public class ShipmentRateDetails {
        public string ServiceType { get; set; }
        public string PackagingType { get; set; }
        public List<RateDetail> Rates { get; set; }
        public string TransitTime { get; set; }
        public List<ShippingNotification> Notifications { get; set; }
    }

    public class RateDetail {
        public string RateType { get; set; }
        public KeyValuePair<decimal,string> TotalBillingWeight { get; set; }
        public KeyValuePair<decimal,string> TotalBaseCharge { get; set; }
        public KeyValuePair<decimal,string> TotalFreightDiscounts { get; set; }
        public KeyValuePair<decimal, string> TotalSurcharges { get; set; }
        public List<ShippingSurcharge> Surcharges { get; set; }
        public KeyValuePair<decimal,string> NetCharge { get; set; }
    }

    public class ShippingSurcharge {
        public string Type { get; set; }
        public KeyValuePair<decimal,string> Charge { get; set; }
    }

    public class ShippingNotification {
        public int NotificationID { get; set; }
        public string Severity { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }
}