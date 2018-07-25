using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopify_Inventory_Downloader.Controllers
{
    public class ProductsController : Controller
    {
        // GET: Products/GetProduct
        public ActionResult GetProuct(string shop, string code)
        {

            string u = string.Format("https://{0}/admin/oauth/access_token", shop);



            return View();
        }
    }
}