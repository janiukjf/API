using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace API.Controllers
{
    public class UserController : KeyAuthController
    {
        /********* User Interaction *********/
        [AcceptVerbs(HttpVerbs.Post)]
        public void GetUser(string username = "", string password = "") {
            user u = new user {
                username = username,
                password = password
            };
            try {
                string json = u.GetUser();
                Response.ContentType = "application/json";
                Response.Write(json);
                Response.End();
            } catch (Exception e) {
                throwError(e.Message);
            }
        }

    }
}
