using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API.Models;

namespace API.Controllers
{
    public class IndexController : Controller
    {
        //
        // GET: /Index/

        public void Index()
        {
            HttpContext.Response.Redirect("http://labs.curtmfg.com");
        }

        public string ViewAddress() {
            return Fedex.SerializeAddress();
        }

    }
}
