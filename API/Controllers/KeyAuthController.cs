using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace API.Controllers {
    public class KeyAuthController : Controller {
        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            
            string key = "";
            if (Request.QueryString["key"] != null && Request.QueryString["key"] != "") {
                key = Request.QueryString["key"];
            }
            if (Request.Form["key"] != null && Request.Form["key"] != "") {
                key = Request.Form["key"];
            }

            if (key == null || key.Length == 0) {
                throwError("Invalid API Key");
            } else {
                try {
                    CurtDevDataContext db = new CurtDevDataContext();
                    string cID = db.Customers.Where(x => x.APIKey.Equals(key)).Select(x => x.customerID).ToString();
                    if (cID.Length == 0 || cID == "0") {
                        throwError("Invalid API Key");
                    }
                } catch (Exception e) {
                    throwError(e.Message);
                }
            }

            base.OnActionExecuting(filterContext);
        }

        public void throwError(string msg = "") {
            if (msg.Length == 0) {
                msg = "There was an error while processing your request";
            }

            Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            Response.StatusDescription = msg;
            Response.ContentType = "application/json";
            Response.Write(JsonConvert.SerializeObject(msg, Formatting.Indented));
            Response.End();
        }

    }
}
