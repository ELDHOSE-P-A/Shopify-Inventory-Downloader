using Newtonsoft.Json;
using RestSharp;
using Shopify_Inventory_Downloader.Tools;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            //First, we need to verify if this is freom shopify
            if (IsAuthenticRequest(Request.QueryString,secretKey))
            {
                string u = string.Format("https://{0}/admin/oauth/access_token", shop);

                var client = new RestClient(u);

                var request = new RestRequest(Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Content-Type", "application/json");

                request.AddParameter("application/x-www-form-urlencoded", "client_id=" + apiKey + "&client_secret=" + secretKey + "&code=" + code, ParameterType.RequestBody);

                var response = client.Execute(request);

                var r = JsonConvert.DeserializeObject<dynamic>(response.Content);
                var access = r.access_token;


                DatabaseHandler.SaveShopToken((String)access);

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


            }

            return View();
        }




        public static bool IsAuthenticRequest(NameValueCollection querystring, string shopifySecretKey)
        {
            string hmac = querystring.Get("hmac");

            if (string.IsNullOrEmpty(hmac))
            {
                return false;
            }

            Func<string, bool, string> replaceChars = (string s, bool isKey) =>
            {
                //Important: Replace % before replacing &. Else second replace will replace those %25s.
                string output = (s?.Replace("%", "%25").Replace("&", "%26")) ?? "";

                if (isKey)
                {
                    output = output.Replace("=", "%3D");
                }

                return output;
            };

            var kvps = querystring.Cast<string>()
                .Select(s => new { Key = replaceChars(s, true), Value = replaceChars(querystring[s], false) })
                .Where(kvp => kvp.Key != "signature" && kvp.Key != "hmac")
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => $"{kvp.Key}={kvp.Value}");

            var hmacHasher = new HMACSHA256(Encoding.UTF8.GetBytes(shopifySecretKey));
            var hash = hmacHasher.ComputeHash(Encoding.UTF8.GetBytes(string.Join("&", kvps)));

            //Convert bytes back to string, replacing dashes, to get the final signature.
            var calculatedSignature = BitConverter.ToString(hash).Replace("-", "");

            //Request is valid if the calculated signature matches the signature from the querystring.
            return calculatedSignature.ToUpper() == hmac.ToUpper();
        }


    }
}