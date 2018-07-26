using RestSharp;
using Shopify_Inventory_Downloader.Tools;
using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shopify_Inventory_Downloader.Controllers
{
    public class ProductsController : Controller
    {
        private string AccessToken = "";
        private string ShopUrl = "";
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
            ShopUrl = shop;
            string u = string.Format("https://{0}/admin/oauth/access_token", shop);
            var client = new RestClient(u);
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("client_id", apiKey);
            request.AddParameter("client_secret", secretKey);
            request.AddParameter("code", code);
            var response = client.Execute(request);

            var r = JsonConvert.DeserializeObject<dynamic>(response.Content);
            AccessToken = r.access_token;

            AppSettings.CurrentShop = shop;
            AppSettings.Token = AccessToken;
            try
            {
                DatabaseHandler.SaveShopAndToken(shop, AccessToken);// saving the shiop and token to the custom function made to handle database requests
            }
            catch (Exception e)
            {
                ;
            }
            return View();


        }


        public ActionResult Download()
        {


            ShopUrl = AppSettings.CurrentShop;
            AccessToken = AppSettings.Token;
            String u = string.Format("https://{0}/admin/products.json ", ShopUrl);
            var client = new RestClient(u);
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-Shopify-Access-Token", AccessToken);
            var response = client.Execute(request);
            var r = JsonConvert.DeserializeObject<dynamic>(response.Content);
            string json = JsonConvert.SerializeObject(r);
            string jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);


            // Writing JSON to file
            string filePath = Server.MapPath("/OutPut");
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filePath+"\\Output1.js"))
            {

                // If the line doesn't contain the word 'Second', write the line to the file.
                
                    file.Write(jsonFormatted);

            }

            return Content("YOUR PRODUCT CATALOGUE IN JSON FORMATT IS Downloaded to /OutPut folder in the server :)  ");

        }






    }
}