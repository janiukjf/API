using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API.Models;
using System.Text;
using API;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace CURT_Docs.Controllers
{
    public class V2Controller : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            base.OnActionExecuting(filterContext);
            new APIAnalytic().Log();
        }

        /* Version 2.0 of CURT Manufacturing eCommerce Data API */

        public ActionResult Index() {
            return View();
        }

        /// <summary>
        /// Get all the years in the database
        /// </summary>
        /// <param name="dataType">Response Format [XML,JSON,JSONP]</param>
        /// <param name="callback">If dataType equals JSONP, we use callback string as function name</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetYear(string mount = "", string dataType = "", string callback = ""){
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") {
                string yearJSON = V2Model.GetYearsJSON(mount);
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + yearJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(yearJSON);
                }
                Response.End();
            } else {
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetYearsXML(mount));
                Response.End();
            }
        }

        /// <summary>
        /// Get all the makes, or if a year is passed in, get all the makes for that year
        /// </summary>
        /// <param name="year">Year to retrieve makes for. [Optional]</param>
        /// <param name="dataType">Response Format [XML,JSON,JSONP]</param>
        /// <param name="callback">If dataType equals JSONP, we will use callback string as function name.</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetMake(string mount = "", double year = 0,string dataType = "", string callback = "") {
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") {
                string makeJSON = V2Model.GetMakesJSON(mount, year);
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + makeJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(makeJSON);
                }
                Response.End();
            } else {
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetMakesXML(mount, year));
                Response.End();
            }
        }

        /// <summary>
        /// Get all the models, or if a year and make are passed in, get the all models for that year/make
        /// </summary>
        /// <param name="year">Year to retrieve models for. [Optional]</param>
        /// <param name="make">Make to retrieve models for. [Optional]</param>
        /// <param name="dataType">Response Format [XML,JSON,JSONP]</param>
        /// <param name="callback">If dataType equals JSONP, we will use callback string as function name.</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetModel(string mount = "", double year = 0, string make = "", string dataType = "", string callback = "") {
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") {
                string modelJSON = V2Model.GetModelsJSON(mount,year,make);
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + modelJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(modelJSON);
                }
                Response.End();
            } else {
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetModelsXML(mount,year,make));
                Response.End();
            }
        }

        /// <summary>
        /// Get all the styles, or if a year, make, model are passed in, get the models for that year/make/model.
        /// </summary>
        /// <param name="year">Year to retrieve styles for. [Optional]</param>
        /// <param name="make">Make to retrieve styles for. [Optional]</param>
        /// <param name="model">Model to retrieve styles for. [Optional]</param>
        /// <param name="dataType">Response Format. [XML,JSON,JSONP]</param>
        /// <param name="callback">If dataType equals JSONP, we will use callback string as function name.</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetStyle(string mount = "", double year = 0, string make = "", string model = "", string dataType = "", string callback = "") {
           if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") {
                string styleJSON = V2Model.GetStylesJSON(mount, year, make, model);
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + styleJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(styleJSON);
                }
                Response.End();
            } else {
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetStylesXML(mount, year, make, model));
                Response.End();
            }
        }

        /// <summary>
        /// Get all the vehicles records, or if a year, make, model, and style are passed in, get the vehicles for that year/make/model/style.
        /// </summary>
        /// <param name="year">Year of the vehicle. [Optional]</param>
        /// <param name="make">Make of the vehicle. [Optional]</param>
        /// <param name="model">Model of the vehicle. [Optional]</param>
        /// <param name="style">Style of the vehicle. [Optional]</param>
        /// <param name="dataType">Response Format. [XML,JSON,JSONP]</param>
        /// <param name="callback">If dataType equals JSONP, we will use callback string as function name.</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetVehicle(double year = 0, string make = "", string model = "", string style = "", string dataType = "", string callback = "") {
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") {
                string vehicleJSON = V2Model.GetVehiclesJSON(year, make, model, style);
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + vehicleJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(vehicleJSON);
                }
                Response.End();
            } else {
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetVehiclesXML(year, make, model, style));
                Response.End();
            }
        }

        /// <summary>
        /// Returns the vehicles that fit the given part ID
        /// </summary>
        /// <param name="partID">ID of the part to find vehicles for.</param>
        /// <param name="dataType">Return data type</param>
        /// <param name="callback">If data type is JSONP, wrap in function</param>
        public void GetPartVehicles(int partID = 0, string dataType = "", string callback = "") {
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                string vehicleJSON = V2Model.GetVehiclesByPartJSON(partID);
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + vehicleJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(vehicleJSON);
                }
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetVehiclesByPartXML(partID));
                Response.End();
            }
        }

        /// <summary>
        /// Get the parts for either the given vehicleID OR the year/make/model/style.
        /// </summary>
        /// <param name="vehicleID">ID of the vehicle to retrieve parts for. [Optional]</param>
        /// <param name="year">Year of the vehicle to retrieve parts for. [Optional]</param>
        /// <param name="make">Make of the vehicle to retrieve parts for. [Optional]</param>
        /// <param name="model">Model of the vehicle to retrieve parts for. [Optional]</param>
        /// <param name="style">Style of the vehicle to retrieve parts for. [Optional]</param>
        /// <param name="integrated">Integrate with customer part number. [Optional]</param>
        /// <param name="cust_id">Customer ID. [Optional]</param>
        /// <param name="dataType">Response Format. [XML,JSON,JSONP]</param>
        /// <param name="callback">IF dataType equals JSONP, we will use callback string as functio name.</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetParts(string mount = "", int vehicleID = 0, double year = 0, string make = "", string model = "", string style = "", string catName = "", string status = "800,900", bool integrated = false, int cust_id = 0, string dataType = "", string callback = "") {
            List<int> statuses = splitList(status);
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") {
                string partJSON = "";
                if (vehicleID != 0) {
                    if (catName.Length > 0){
                        partJSON = V2Model.GetPartsByVehicleIDWithFilterJSON(vehicleID, catName, statuses, integrated, cust_id);
                    } else{
                        partJSON = V2Model.GetPartsByVehicleIDJSON(vehicleID, statuses, integrated, cust_id);
                    }
                } else {
                    if (catName.Length > 0) {
                        partJSON = V2Model.GetPartsWithFilterJSON(year, make, model, style, catName, statuses, integrated, cust_id);
                    } else {
                        partJSON = V2Model.GetPartsJSON(year, make, model, style, statuses, integrated, cust_id);
                    }
                }
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + partJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(partJSON);
                }
                Response.End();
            } else {
                Response.ContentType = "text/xml";
                if (vehicleID != 0) {
                    if (catName.Length > 0) {
                        Response.Write(V2Model.GetPartsByVehicleIDWithFilterXML(vehicleID, catName, statuses, integrated, cust_id));
                    } else {
                        Response.Write(V2Model.GetPartsByVehicleIDXML(vehicleID, statuses, integrated, cust_id));
                    }
                } else {
                    if (catName.Length > 0) {
                        Response.Write(V2Model.GetPartsWithFilterXML(year, make, model, style, catName, statuses, integrated, cust_id));
                    } else {
                        Response.Write(V2Model.GetPartsXML(year, make, model, style, statuses, integrated, cust_id));
                    }
                }
                Response.End();
            }
        }

        /// <summary>
        /// Get the parts in the provided comma separated list.
        /// </summary>
        /// <param name="vehicleID">ID of the vehicle to retrieve parts for. [Optional]</param>
        /// <param name="year">Year of the vehicle to retrieve parts for. [Optional]</param>
        /// <param name="make">Make of the vehicle to retrieve parts for. [Optional]</param>
        /// <param name="model">Model of the vehicle to retrieve parts for. [Optional]</param>
        /// <param name="style">Style of the vehicle to retrieve parts for. [Optional]</param>
        /// <param name="integrated">Integrate with customer part number. [Optional]</param>
        /// <param name="cust_id">Customer ID. [Optional]</param>
        /// <param name="dataType">Response Format. [XML,JSON,JSONP]</param>
        /// <param name="callback">IF dataType equals JSONP, we will use callback string as functio name.</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetPartsByList(int vehicleID = 0, double year = 0, string make = "", string model = "", string style = "", string partlist = "", bool integrated = false, int cust_id = 0, string dataType = "", string callback = "") {
            List<int> partids = new List<int>();
            if (partlist.Trim().Length > 0) {
                try {
                    partids = partlist.Split(',').Select(n => int.Parse(n)).ToList();
                } catch { };
            }

            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") {
                string partJSON = "";
                if (partids.Count > 0) {
                    if (vehicleID != 0) {
                        partJSON = V2Model.GetPartsByListWithVehicleIDJSON(vehicleID, partids, integrated, cust_id);
                    } else if (year != 0 && make != "" && model != "" && style != "") {
                        partJSON = V2Model.GetPartsByListWithFilterJSON(year, make, model, style, partids, integrated, cust_id);
                    } else {
                        partJSON = V2Model.GetPartsByListJSON(partids, integrated, cust_id);
                    }
                }
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + partJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(partJSON);
                }
                Response.End();
            } else {
                Response.ContentType = "text/xml";
                if (partids.Count > 0) {
                    if (vehicleID != 0) {
                        Response.Write(V2Model.GetPartsByListWithVehicleIDXML(vehicleID, partids, integrated, cust_id));
                    } else if (year != 0 && make != "" && model != "" && style != "") {
                        Response.Write(V2Model.GetPartsByListWithFilterXML(year, make, model, style, partids, integrated, cust_id));
                    } else {
                        Response.Write(V2Model.GetPartsByListXML(partids, integrated, cust_id));
                    }
                }
                Response.End();
            }
        }

        /// <summary>
        /// Retrieve part information for a given partID
        /// </summary>
        /// <param name="partID">ID of the part to retrieve</param>
        /// <param name="dataType">Response Format [XML,JSON,JSONP]</param>
        /// <param name="callback">If the dataType is equal to JSONP we will use the callback string as the returned function name.</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetPart(int partID = 0, int vehicleID = 0, double year = 0, string make = "", string model = "", string style = "", bool integrated = false, int cust_id = 0, string dataType = "", string callback = "") {

            // Validate the partID
            if (partID == 0) {
                Response.ContentType = "application/json";
                Response.Write("Invalid partID");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string partJSON = "";
                    if (partID > 0) {
                        if (vehicleID > 0) {
                            partJSON = V2Model.GetPartByVehicleIDJSON(partID, vehicleID, integrated, cust_id);
                        } else if (year > 0 && make != "" && model != "" && style != "") {
                            partJSON = V2Model.GetPartWithFilterJSON(partID, year, make, model, style, integrated, cust_id);
                        } else {
                            partJSON = V2Model.GetPartJSON(partID, integrated, cust_id);
                        }
                    }
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + partJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(partJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    if (partID > 0) {
                        if (vehicleID > 0) {
                            Response.Write(V2Model.GetPartByVehicleIDXML(partID, vehicleID, integrated, cust_id));
                        } else if (year > 0 && make != "" && model != "" && style != "") {
                            Response.Write(V2Model.GetPartWithFilterXML(partID, year, make, model, style, integrated, cust_id));
                        } else {
                            Response.Write(V2Model.GetPartXML(partID, integrated, cust_id));
                        }
                    }
                    Response.End();
                }
            }
        }

        /// <summary>
        /// Retrieve part prices for a given partID
        /// </summary>
        /// <param name="partID">ID of the part to retrieve</param>
        /// <param name="dataType">Response Format [XML,JSON,JSONP]</param>
        /// <param name="callback">If the dataType is equal to JSONP we will use the callback string as the returned function name.</param>
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetPartPrices(int partID = 0, string dataType = "", string callback = "") {

            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                string partJSON = "";
                partJSON = V2Model.GetPartPricesJSON(partID);
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + partJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(partJSON);
                }
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetPartPricesXML(partID));
                Response.End();
            }
        }
        
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetAllParts(string status = "800,900", string dataType = "", string callback = "") {

            List<int> statuses = splitList(status);
            if (dataType.ToUpper() == "JSON") { // Display JSON
                string partJSON = "";
                partJSON = V2Model.GetAllPartsJSON(statuses);
                Response.ContentType = "application/json";
                Response.Write(partJSON);
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetAllPartsXML(statuses));
                Response.End();
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public void GetAllPartID(string status = "800,900", string dataType = "", string callback = "") {
            List<int> statuses = splitList(status);
            if (dataType.ToUpper().Equals("JSON")) {
                Response.ContentType = "application/json";
                Response.Write(V2Model.GetAllPartIDJSON(statuses));
                Response.End();
            } else if (dataType.ToUpper().Equals("JSONP")) {
                Response.ContentType = "application/x-javascript";
                Response.Write(callback + "(" + V2Model.GetAllPartIDJSON(statuses) + ")");
                Response.End();
            } else {
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetAllPartIDXML(statuses));
                Response.End();
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public void GetInstallSheet(int partID = 0, string dataType = "", string callback = "") {
            // Validate the partID
            if (partID == 0) {
                Response.ContentType = "application/json";
                Response.Write("Invalid partID");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string partJSON = "";
                    if (partID > 0) {
                        partJSON = V2Model.GetInstallSheetJSON(partID);
                    }
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + partJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(partJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    if (partID > 0) {
                        Response.Write(V2Model.GetInstallSheetXML(partID));
                    }
                    Response.End();
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public void GetRelatedParts(int partID = 0, string status = "800,900", string dataType = "", string callback = "", bool widget = false, bool integrated = false, int cust_id = 0) {
            List<int> statuses = splitList(status);
            // Validate the partID
            if (partID == 0) {
                Response.ContentType = "application/json";
                Response.Write("Invalid partID");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string partJSON = "";
                    if (partID > 0) {
                        partJSON = V2Model.GetRelatedJSON(partID, statuses, integrated, cust_id);
                    }
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        if (!widget) {
                            Response.Write(callback + "(" + partJSON + ")");
                        } else {
                            Response.Write(callback + "(" + partID + "," + partJSON + ")");
                        }
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(partJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    if (partID > 0) {
                        Response.Write(V2Model.GetRelatedXML(partID, statuses, integrated, cust_id));
                    }
                    Response.End();
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public void GetLatestParts(int count = 5, string status = "800,900", string dataType = "", string callback = "", bool integrated = false, int cust_id = 0) {

            List<int> statuses = splitList(status);
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                string partsJSON = "";
                partsJSON = V2Model.GetLatestPartsJSON(statuses, integrated, cust_id, 6);
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + partsJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(partsJSON);
                }
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetLatestPartsXML(statuses, integrated, cust_id, 6));
                Response.End();
            }
        }

        public void GetSPGridData(int partID = 0, string dataType = "",string callback = "") {
            // Validate the partID
            if (partID == 0) {
                Response.ContentType = "application/json";
                Response.Write("Invalid partID");
                Response.End();
            }

            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                string gridJSON = "";
                if (partID > 0) {
                    gridJSON = V2Model.GetSPGridJSON(partID);
                }
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + gridJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(gridJSON);
                }
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                if (partID > 0) {
                    Response.Write(V2Model.GetSPGridXML(partID));
                }
                Response.End();
            }
        }

        public void GetPartsByDateModified(string date = "", string status = "800,900", string dataType = "", string callback = "") {
            List<int> statuses = splitList(status);
            // Validate the partID
            if (date.Length == 0) {
                Response.ContentType = "application/json";
                Response.Write("You must enter a valid Date");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string gridJSON = V2Model.GetPartsByDateModifiedJSON(date, statuses);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + gridJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(gridJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetPartsByDateModifiedXML(date, statuses));
                    Response.End();
                }
            }
        }

        public void GetAttributes(int partID = 0, string dataType = "", string callback = "") {
            // Validate the partID
            if (partID == 0) {
                Response.ContentType = "application/json";
                Response.Write("Invalid Part #");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string attrJSON = V2Model.GetAttributesJSON(partID);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + attrJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(attrJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetAttributesXML(partID));
                    Response.End();
                }
            }
        }

        public void GetReviewsByPart(int partID = 0, int page = 1, int perPage = 10, string dataType = "", string callback = "", int cust_id = 0) {
            // Validate the partID
            if (partID == 0) {
                Response.ContentType = "application/json";
                Response.Write("Invalid Part #");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string attrJSON = V2Model.GetReviewsByPartJSON(partID, page, perPage, cust_id);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + attrJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(attrJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetReviewsByPartXML(partID, page, perPage, cust_id));
                    Response.End();
                }
            }
        }

        public void SubmitReview(int partID = 0, int cust_id = 0, string name = "", string email = "", int rating = 0, string subject = "", string review_text = "") {
            // Validate the data
            if (partID == 0 || cust_id == 0 || subject == "" || rating == 0 || review_text == "") {
                Response.ContentType = "application/json";
                Response.Write("Invalid Data Submitted");
                Response.Write("partID, cust_id, subject, rating, and review_text are required");
                Response.End();
            } else {
                V2Model.SubmitReview(partID, cust_id, name, email, rating, subject, review_text);
                Response.ContentType = "application/json";
                Response.Write("success");
                Response.End();
            }
        }

        public void GetParentCategories(string dataType = "", string callback = "") {
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                string catJSON = V2Model.GetParentCategoriesJSON();
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + catJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(catJSON);
                }
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetParentCategoriesXML());
                Response.End();
            }
        }

        public void GetFullParentCategories(string dataType = "", string callback = "") {
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                string catJSON = V2Model.GetFullParentCategoriesJSON();
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + catJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(catJSON);
                }
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetFullParentCategoriesXML());
                Response.End();
            }
        }

        public void GetCategories(int parentID = 0, string dataType = "", string callback = "") {
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                string catJSON = V2Model.GetCategoriesJSON(parentID);
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + catJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(catJSON);
                }
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetCategoriesXML(parentID));
                Response.End();
            }
        }

        public void GetPartCategories(int partID = 0, string dataType = "", string callback = "") {
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                string catJSON = V2Model.GetPartCategoriesJSON(partID);
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + catJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(catJSON);
                }
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetPartCategoriesXML(partID));
                Response.End();
            }
        }

        public void GetCategory(int catID = 0, string dataType = "", string callback = "") {
            if (catID == 0) {
                Response.ContentType = "text/plain";
                Response.Write("Invalid category ID.");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string catJSON = V2Model.GetCategoryJSON(catID);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + catJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(catJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetCategoryXML(catID));
                    Response.End();
                }
            }
        }

        public void GetCategoryByName(string catName = "", string dataType = "", string callback = "") {
            if (catName.Length == 0) {
                Response.ContentType = "text/plain";
                Response.Write("Invalid category name.");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string catJSON = V2Model.GetCategoryByNameJSON(catName);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + catJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(catJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetCategoryByNameXML(catName));
                    Response.End();
                }
            }
        }

        public void GetCategoryAttributes(int catID = 0, string dataType = "", string callback = "") {
            if (catID == 0) {
                Response.ContentType = "text/plain";
                Response.Write("Invalid category ID.");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string catJSON = V2Model.GetCategoryAttributesJSON(catID);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + catJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(catJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetCategoryAttributesXML(catID));
                    Response.End();
                }
            }
        }
        
        public void GetCategoryBreadCrumbs(int catId = 0, string dataType = "", string callback = "") {
            if (catId == 0) {
                Response.ContentType = "text/plain";
                Response.Write("Invalid category id.");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string catJSON = V2Model.GetCategoryBreadCrumbsJSON(catId);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + catJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(catJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetCategoryBreadCrumbsXML(catId));
                    Response.End();
                }
            }
        }

        public void GetPartBreadCrumbs(int partID = 0, int catId = 0, string dataType = "", string callback = "") {
            if (partID == 0) {
                Response.ContentType = "text/plain";
                Response.Write("Invalid category id.");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string catJSON = V2Model.GetPartBreadCrumbsJSON(partID, catId);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + catJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(catJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetPartBreadCrumbsXML(partID, catId));
                    Response.End();
                }
            }
        }

        public void GetCategoryParts(int catID = 0, int cust_id = 0, int vehicleID = 0, double year = 0, string make = "", string model = "", string style = "", string status = "800,900", bool integrated = false, string dataType = "", string callback = "", int page = 1, int perpage = 0) {
            List<int> statuses = splitList(status);
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                string partsJSON = "";
                if (vehicleID != 0) {
                    partsJSON = V2Model.GetCategoryPartsWithVehicleIDJSON(catID, vehicleID, statuses, cust_id, integrated, page, perpage);
                } else if (year != 0 && make != "" && model != "" && style != "") {
                    partsJSON = V2Model.GetCategoryPartsWithFilterJSON(catID, year, make, model, style, statuses, cust_id, integrated, page, perpage);
                } else {
                    partsJSON = V2Model.GetCategoryPartsJSON(catID, statuses, cust_id, integrated, page, perpage);
                }
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + partsJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(partsJSON);
                }
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                if (vehicleID != 0) {
                    Response.Write(V2Model.GetCategoryPartsWithVehicleIDXML(catID, vehicleID, statuses, cust_id, integrated, page, perpage));
                } else if (year != 0 && make != "" && model != "" && style != "") {
                    Response.Write(V2Model.GetCategoryPartsWithFilterXML(catID, year, make, model, style, statuses, cust_id, integrated, page, perpage));
                } else {
                    Response.Write(V2Model.GetCategoryPartsXML(catID, statuses, cust_id, integrated, page, perpage));
                }
                Response.End();
            }
        }

        public void GetCategoryPartsCount(int catID = 0, int cust_id = 0, string status = "800,900", bool integrated = false, string dataType = "", string callback = "") {
            string countJSON = "";
            List<int> statuses = splitList(status);
            countJSON = V2Model.GetCategoryPartsCount(catID, statuses, cust_id, integrated);
            if (dataType.ToUpper() == "JSONP") {
                Response.ContentType = "application/x-javascript";
                Response.Write(callback + "(" + countJSON + ")");
            } else {
                Response.ContentType = "application/json";
                Response.Write(countJSON);
            }
            Response.End();
        }

        public void GetCategoryPartsByName(string catName = "", int cust_id = 0, string status = "800,900", bool integrated = false, string dataType = "", string callback = "", int page = 1, int perpage = 0) {
            List<int> statuses = splitList(status);
            if (catName.Length == 0) {
                Response.ContentType = "text/plain";
                Response.Write("Invalid category.");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string partsJSON = V2Model.GetCategoryPartsByNameJSON(catName, statuses, cust_id, integrated, page, perpage);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + partsJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(partsJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetCategoryPartsByNameXML(catName, statuses, cust_id, integrated, page, perpage));
                    Response.End();
                }
            }
        }

        public void GetLifestyles(string dataType = "", string callback = "") {
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                string catJSON = V2Model.GetLifestylesJSON();
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + catJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(catJSON);
                }
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetLifestylesXML());
                Response.End();
            }
        }

        public void GetLifestyle(int lifestyleid = 0, string dataType = "", string callback = "") {
            if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                string catJSON = V2Model.GetLifestyleJSON(lifestyleid);
                if (dataType.ToUpper() == "JSONP") {
                    Response.ContentType = "application/x-javascript";
                    Response.Write(callback + "(" + catJSON + ")");
                } else {
                    Response.ContentType = "application/json";
                    Response.Write(catJSON);
                }
                Response.End();
            } else { // Display XML
                Response.ContentType = "text/xml";
                Response.Write(V2Model.GetLifestyleXML(lifestyleid));
                Response.End();
            }
        }

        public void GetConnector(int vehicleID = 0, double year = 0, string make = "", string model = "", string style = "", string status = "800,900", bool integrated = false, int cust_id = 0, string dataType = "", string callback = "") {
            List<int> statuses = splitList(status);
            if (vehicleID == 0) {
                // We're going to parse out the vehicle record using year make model style...
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string connectorJSON = V2Model.GetConnectorBySpecJSON(year, make, model, style, statuses, integrated, cust_id);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + connectorJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(connectorJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetConnectorBySpecXML(year, make, model, style, statuses, integrated, cust_id));
                    Response.End();
                }
            } else {

                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string connectorJSON = V2Model.GetConnectorJSON(vehicleID, statuses, integrated, cust_id);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + connectorJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(connectorJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetConnectorXML(vehicleID, statuses, integrated, cust_id));
                    Response.End();
                }
            }
        }

        public void Search(string search_term = "", string status = "800,900", string dataType = "", string callback = "") {
            List<int> statuses = splitList(status);
            if (search_term.Length == 0) {
                Response.ContentType = "text/plain";
                Response.Write("");
                Response.End();
            } else {

                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string searchJSON = V2Model.GetSearch_JSON(search_term, statuses);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + searchJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(searchJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetSearch_XML(search_term, statuses));
                    Response.End();
                }
            }
        }

        public void PowerSearch(string search_term = "", string status = "800,900", bool integrated = false, int customerID = 0, string dataType = "", string callback = "") {
            List<int> statuses = splitList(status);
            if (search_term.Length == 0) {
                Response.ContentType = "text/plain";
                Response.Write("");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string searchJSON = V2Model.GetPowerSearch_JSON(search_term, statuses, integrated, customerID);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + searchJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(searchJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetPowerSearch_XML(search_term, statuses, integrated, customerID));
                    Response.End();
                }
            }
        }

        public void GetDefaultPartImages(bool integrated = false, int cust_id = 0) {
            Response.ContentType = "text/xml";
            Response.Write(V2Model.GetPartDefaultImages_XML(integrated,cust_id));
            Response.End();
        }

        public void GetPartImagesByIndex(char index = 'a', bool integrated = false, int cust_id = 0) {
            Response.ContentType = "text/xml";
            Response.Write(V2Model.GetPartImagesByIndex_XML(index, integrated, cust_id));
            Response.End();
        }

        public void GetPartImages(int partID = 0, bool integrated = false, int cust_id = 0) {
            Response.ContentType = "text/xml";
            Response.Write(V2Model.GetPartImages_XML(partID, integrated, cust_id));
            Response.End();
        }

        public void GetPartImage(int partID = 0, char index = 'a', string size = "Grande") {
            Response.ContentType = "text";
            Response.Write(V2Model.GetPartImage(partID, index, size));
            Response.End();
        }

        public void GetFileInfo(string path = "") {
            Response.ContentType = "text/xml";
            Response.Write(V2Model.getFileInfo_XML(path));
            Response.End();
        }


        /********* Customer Interaction *********/
        public void GetPricing(int customerID = 0, string key = "") {
            try {
                CustomerPricing pricing = new CustomerPricing { cust_id = customerID };
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(pricing.GetAll(key)));
                Response.End();
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(e.Message, Formatting.Indented));
                Response.End();
            }
        }

        public void GetUnintegratedParts(int customerID = 0, string dataType = "", string callback = "") {
            // Validate the customerID
            if (customerID == 0) {
                Response.ContentType = "application/json";
                Response.Write("You must provide your customer ID");
                Response.End();
            } else {
                CartIntegration integration = new CartIntegration {
                    custID = customerID
                };
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string gridJSON = integration.GetUnintegratedPartsJSON();
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + gridJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(gridJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(integration.GetUnintegratedPartsXML());
                    Response.End();
                }
            }
        }
        
        /// <summary>
        /// Sets a price point for a given partID to be associated with a customerID and key
        /// </summary>
        /// <param name="customerID">POST variable referencing customer (required)</param>
        /// <param name="key">GET variable to allow API call (required)</param>
        /// <param name="partID">POST variable to refernce part (required)</param>
        /// <param name="price">POST variable to set price to (required)</param>
        /// <param name="isSale">POST variable to mark price point as sale price or not</param>
        /// <param name="sale_start">POST variable for sale start</param>
        /// <param name="sale_end">POST variable for sale end</param>
        [AcceptVerbs(HttpVerbs.Post)]
        public void SetPrice(int customerID = 0, string key = "", int partID = 0, decimal price = 0, int isSale = 0, string sale_start = "", string sale_end = "") {
            try {
                CustomerPricing pricing = new CustomerPricing{
                    cust_id = customerID,
                    partID = partID,
                    price = price,
                    isSale = isSale,
                    sale_start = (sale_start.Length > 0) ? Convert.ToDateTime(sale_start) : (DateTime?)null,
                    sale_end = (sale_end.Length > 0) ? Convert.ToDateTime(sale_end) : (DateTime?)null
                };
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(pricing.Set(key)));
                Response.End();
            } catch (Exception e) {
                //Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                //Response.StatusDescription = e.Message;
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(e.Message,Formatting.Indented));
                Response.End();
            }
        }

        /// <summary>
        /// Sets a customer part ID for a given partID to be associated with a customerID and key
        /// </summary>
        /// <param name="customerID">POST variable referencing customer (required)</param>
        /// <param name="key">GET variable to allow API call (required)</param>
        /// <param name="partID">POST variable to refernce part (required)</param>
        /// <param name="customerPartID">POST variable to customerPartID (required)</param>
        [AcceptVerbs(HttpVerbs.Post)]
        public void SetCustomerPart(int customerID = 0, string key = "", int partID = 0, int customerPartID = 0) {
            try {
                CartIntegration integration = new CartIntegration {
                    custID = customerID,
                    custPartID = customerPartID,
                    partID = partID
                };
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(integration.Set(key)));
                Response.End();
            } catch (Exception e) {
                //Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                //Response.StatusDescription = e.Message;
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(e.Message, Formatting.Indented));
                Response.End();
            }
        }

        /// <summary>
        /// Sets a price point for a given partID to be associated with a customerID and key
        /// </summary>
        /// <param name="customerID">POST variable referencing customer (required)</param>
        /// <param name="key">GET variable to allow API call (required)</param>
        /// <param name="partID">POST variable to reference part (required)</param>
        /// <param name="partID">POST variable to set customer reference part (required)</param>
        /// <param name="price">POST variable to set price to (required)</param>
        /// <param name="isSale">POST variable to mark price point as sale price or not</param>
        /// <param name="sale_start">POST variable for sale start</param>
        /// <param name="sale_end">POST variable for sale end</param>
        [AcceptVerbs(HttpVerbs.Post)]
        public void SetCustomerPartAndPrice(int customerID = 0, string key = "", int partID = 0,  int customerPartID = 0, decimal price = 0, int isSale = 0, string sale_start = "", string sale_end = "") {
            try {
                CartIntegration integration = new CartIntegration {
                    custID = customerID,
                    custPartID = customerPartID,
                    partID = partID
                };
                CartIntegration newintegration = integration.Set(key);
                CustomerPricing pricing = new CustomerPricing {
                    cust_id = customerID,
                    partID = partID,
                    price = price,
                    isSale = isSale,
                    sale_start = (sale_start.Length > 0) ? Convert.ToDateTime(sale_start) : (DateTime?)null,
                    sale_end = (sale_end.Length > 0) ? Convert.ToDateTime(sale_end) : (DateTime?)null
                };
                SimplePricing newpricing = pricing.Set(key);
                List<object> results = new List<object>();
                results.Add(newintegration);
                results.Add(newpricing);

                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(results));
                Response.End();
            } catch (Exception e) {
                //Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                //Response.StatusDescription = e.Message;
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(e.Message, Formatting.Indented));
                Response.End();
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void ResetToList(string key = "", int customerID = 0, int partID = 0) {
            try {
                CustomerPricing pricing = new CustomerPricing {
                    cust_id = customerID,
                    partID = partID
                };
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(pricing.SetToList(key)));
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                Response.StatusDescription = e.Message;
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(e.Message + e.StackTrace,Formatting.Indented));
                Response.End();
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void ResetToMap(string key = "", int customerID = 0, int partID = 0) {
            try {
                CustomerPricing pricing = new CustomerPricing {
                    cust_id = customerID,
                    partID = partID
                };
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(pricing.SetToMap(key)));
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                Response.StatusDescription = e.Message;
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(e.Message + e.StackTrace, Formatting.Indented));
                Response.End();
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void ResetAllToMap(string key = "", int customerID = 0) {
            try {
                CustomerPricing pricing = new CustomerPricing {
                    cust_id = customerID
                };
                pricing.SetAllToMap(key);
                Response.ContentType = "text/plain";
                Response.End();
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                Response.StatusDescription = e.Message;
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(e.Message + e.StackTrace, Formatting.Indented));
                Response.End();
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void RemoveSale(string key = "", int customerID = 0, int partID = 0, decimal price = 0) {
            try {
                CustomerPricing pricing = new CustomerPricing {
                    cust_id = customerID,
                    partID = partID,
                    price = price,
                    isSale = 1
                };
                pricing.RemoveSale(key);
                Response.ContentType = "application/json";
                Response.Write("");
                Response.End();
            } catch (Exception e) {
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(e.Message, Formatting.Indented));
                Response.End();
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public void GetCustomer(int customerID = 0, string email = ""){
            try {
                Customer cust = new Customer { customerID = customerID, email = email };
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(cust.Get()));
                Response.End();
            } catch (Exception e) {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                Response.StatusDescription = e.Message;
                Response.ContentType = "application/json";
                Response.Write(JsonConvert.SerializeObject(e.Message, Formatting.Indented));
                Response.End();
            }
        }

        /****** Kiosk API Calls *********/

        public void GetKioskHitches(int vehicleID = 0, int acctID = 0) {
            string partJSON = "";
            partJSON = V2Model.GetKioskHitches(vehicleID, acctID);
            Response.ContentType = "application/json";
            Response.Write(partJSON);
            Response.End();
        }

        public void GetRelatedKiosk(int partID = 0, int acctID = 0) {
            string partJSON = "";
            partJSON = V2Model.GetRelatedKiosk(partID, acctID);
            Response.ContentType = "application/json";
            Response.Write(partJSON);
            Response.End();
        }

        public void GetConnectorKiosk(int vehicleID = 0, int acctID = 0) {
            string partJSON = "";
            partJSON = V2Model.GetConnectorKiosk(vehicleID, acctID);
            Response.ContentType = "application/json";
            Response.Write(partJSON);
            Response.End();
        }

        public void GetOpenAccessories(int vehicleID = 0, int acctID = 0) {
            string partJSON = "";
            partJSON = V2Model.GetOpenAccessories(vehicleID, acctID);
            Response.ContentType = "application/json";
            Response.Write(partJSON);
            Response.End();
        }

        public void CheckAccount(int acctID = 0) {
            int resp = V2Model.CheckAccount(acctID);
            if (resp == 1) {
                Response.StatusCode = 200;
            } else {
                Response.StatusCode = 400;
                Response.TrySkipIisCustomErrors = true;
            }
        }

        public void GetColor(int partID = 0) {
            Response.ContentType = "application/json";
            Response.Write(V2Model.GetCategoryColor_Kiosk(partID));
            Response.End();
        }

        /****** End Kiosk Calls *********/


        /********** eLocal Calls *********/

        public void Search_eLocal(string search_term = "", string dataType = "", string callback = "") {
            if (search_term.Length == 0) {
                Response.ContentType = "text/plain";
                Response.Write("");
                Response.End();
            } else {
                if (dataType.ToUpper() == "JSON" || dataType.ToUpper() == "JSONP") { // Display JSON
                    string searchJSON = V2Model.GetSearch_eLocalJSON(search_term);
                    if (dataType.ToUpper() == "JSONP") {
                        Response.ContentType = "application/x-javascript";
                        Response.Write(callback + "(" + searchJSON + ")");
                    } else {
                        Response.ContentType = "application/json";
                        Response.Write(searchJSON);
                    }
                    Response.End();
                } else { // Display XML
                    Response.ContentType = "text/xml";
                    Response.Write(V2Model.GetSearch_eLocalXML(search_term));
                    Response.End();
                }
            }
        }

        public void Auth_eLocal(int i = 0, string s = "") {
            Response.ContentType = "text/plain";
            Response.Write(V2Model.CheckAuth_eLocal(i, s));
            Response.End();
        }

        public void Auth_Login(string e, string p, string u) {
            Response.ContentType = "text/plain";
            Response.Write(V2Model.DoLogin_eLocal(e,p,u));
            Response.End();
        }

        public void Setup_eLocal(string e, string p, string url) {
            Response.ContentType = "text/plain";
            Response.Write(V2Model.Setup_eLocal(e, p, url));
            Response.End();
        }

        public void GetPricing_eLocal(int acctID) {
            Response.ContentType = "application/json";
            Response.Write(V2Model.GetPricing_eLocal(acctID));
            Response.End();
        }

        public void GetCustomerID(string email) {
            Response.ContentType = "text/plain";
            Response.Write(V2Model.GetCustomerID(email));
            Response.End();
        }

        /********* End eLocal ************/

        /**** Theisens Integration *****/

        public void TheisensProduct(string date_modified = "") {
            Response.ContentType = "text/xml";
            Response.Write(V2Model.GetTheisensByDate(date_modified));
            Response.End();
        }

        /***** End Theisens *****/

        /*** Hitch Widget Tracker ****/

        public void AddDeployment(string url = "") {
            Response.ContentType = "text/plain";
            Response.Write(V2Model.AddDeployment(url));
            Response.End();
        }

        /***** End Tracker **********/

        /**** Image Submission *********/
        public void AddImage(int partID = 0, char sort = ' ', string path = "", int height = 0, int width = 0, string size = "") {
            try {
                V2Model.AddImage(partID, sort, path, height, width, size);
                Response.StatusCode = 200;
            } catch (Exception) {
                Response.StatusCode = 500;
            }
        }
        /****** End Image ****************/

        private List<int> splitList(string list) {
            List<int> returnlist = new List<int>();
            if (list.Trim().Length > 0) {
                try {
                    returnlist = list.Split(',').Select(n => int.Parse(n)).ToList();
                } catch { };
            }
            return returnlist;
        }

    } // End Class
} // End Namespace
