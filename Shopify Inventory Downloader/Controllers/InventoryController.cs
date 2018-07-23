using Newtonsoft.Json;
using RestSharp;
using Shopify_Inventory_Downloader.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopify_Inventory_Downloader.Controllers
{
    

    public class InventoryController : Controller
    {
        private string apiKey = AppSettings.ShopifyApiKey;
        private string secretKey = AppSettings.ShopifySecretKey;
        private string appUrl = AppSettings.AppUrl;

        // GET: Inventory
        public ActionResult install(string shop, string signature, string timestamp)
        {
            string r = string.Format("https://{0}/admin/oauth/authorize?client_id={1}&scope=read_products,write_products,read_product_listings&redirect_uri=https://{2}/Inventory/auth", shop, apiKey, appUrl);
            return Redirect(r);
        }



        public ActionResult auth(string shop, string code)
        {
            string u = string.Format("https://{0}/admin/oauth/access_token", shop);

            var client = new RestClient(u);

            var request = new RestRequest(Method.POST);

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/x-www-form-urlencoded", "client_id=" + apiKey + "&client_secret=" + secretKey + "&code=" + code, ParameterType.RequestBody);

            var response = client.Execute(request);

            var r =JsonConvert.DeserializeObject<dynamic>(response.Content);
            var access = r.access_token;
            /*


            //Part 5
            //create a un-install web hook
            //you want to know when customers delete your app from their shop

            string unInstallUrlCallback = "https://549653d4.ngrok.io/fulfillment/uninstall";

            string shopAdmin = string.Format("https://{0}/admin/", shop);

            var webHook = new WebHookBucket();
            webHook.Whook = new WebHook { Format = "json", Topic = "app/uninstalled", Address = unInstallUrlCallback };

            CreateUninstallHook(shopAdmin, "webhooks.json", Method.POST, (string)accessToken, webHook);

        */

            return View();
        }



    }
}