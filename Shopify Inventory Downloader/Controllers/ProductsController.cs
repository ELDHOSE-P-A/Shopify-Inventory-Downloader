using RestSharp;
using Shopify_Inventory_Downloader.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Shopify_Inventory_Downloader.Controllers
{
    public class ProductsController : Controller
    {

        private string AccessToken = "";
        private readonly string apiKey = AppSettings.ShopifyApiKey;
        private readonly string secretKey = AppSettings.ShopifySecretKey;
        private readonly string appUrl = AppSettings.AppUrl;



        // GET: Products/Install
        public ActionResult Install(string shop, string signature, string timestamp)
        {

            string r = string.Format("https://{0}/admin/oauth/authorize?client_id={1}&scope=read_products,write_products,read_product_listings&redirect_uri=https://{2}/Products/Auth", shop, apiKey, appUrl);
            return Redirect(r);
        }

        // GET: Products/Auth
        public ActionResult Auth(string shop, string code)
        {
            string u = string.Format("https://{0}/admin/oauth/access_token", shop);

            var client = new RestClient(u);

            var request = new RestRequest(Method.POST);

            request.RequestFormat = DataFormat.Json;
            //request.AddHeader("Content-Type", "application/json");

            
            request.AddParameter("client_id", apiKey);
            request.AddParameter("client_secret", secretKey);
            request.AddParameter("code", code);


            //String parameter = "client_id=" + apiKey + "&client_secret=" + secretKey + "&code=" + code;
            //request.AddParameter("application/x-www-form-urlencoded", parameter, ParameterType.RequestBody);
            var response = client.Execute(request);

            var r = JsonConvert.DeserializeObject<dynamic>(response.Content);
            AccessToken = r.access_token;

            DatabaseHandler.SaveShopToken(AccessToken);// saving the token to the custom function made to handle database requests
            

            //return Content("Access token = " + AccessToken);
            return View();
        }


        public ActionResult Download(string shop, string code)
        {
            return View();
        }


     

       
    }
}