using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Shopify_Inventory_Downloader.Tools
{
    public static class AppSettings
    {
        public static string ShopifySecretKey { get; } = ConfigurationManager.AppSettings.Get("Shopify_Secret_Key");
        public static string ShopifyApiKey { get; } = ConfigurationManager.AppSettings.Get("Shopify_API_Key");
        public static string AppUrl { get; } = ConfigurationManager.AppSettings.Get("Shopify_App_Url");
        public static string CurrentShop;
        public static string Token;
    }
}