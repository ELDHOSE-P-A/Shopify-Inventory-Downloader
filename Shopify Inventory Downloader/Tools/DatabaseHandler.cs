using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Shopify_Inventory_Downloader.Tools
{
    public class DatabaseHandler
    {
        //global variables


        //Connection variables
        static SqlConnection con = new SqlConnection(" Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=\"|DataDirectory|\\Database.mdf\";Integrated Security = True"  );
        static SqlCommand cmd;
        static SqlDataReader dr;

        
        //functions

        //Save a ShopName into the database
        public static void SaveShopAndToken(String shop , String token)
        {
            // String  SQL_Syntax = "INSERT INTO SystemTable (Value)   VALUES( @ShopToken ) WHERE Name= 'AccessToken' ";
            String SQL_Syntax = "INSERT INTO SystemTable (Shop , Token)   VALUES( @Shop , @Token)";
            con.Open();
            cmd = new SqlCommand(SQL_Syntax);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@Shop", shop);
            cmd.Parameters.AddWithValue("@Token", token);
            cmd.ExecuteNonQuery();
            con.Close();
            return;
        }

        private void SaveShopName(String ShopName)
        {
            String SQL_Syntax = "INSERT INTO SystemTable (Value)   VALUES( @ShopName ) WHERE Name = ShopName ";
            con.Open();
            cmd = new SqlCommand(SQL_Syntax);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@ShopName", ShopName);
            cmd.ExecuteNonQuery();
            con.Close();

            Console.WriteLine("DEBUG    :   ShopName is inserted now please manually check the database to ensure correctness: ");

            return;
        }

        //get the shopToken from Database
        private String getShopToken()
        {
            String result, SQL_Syntax = "SELECT Value FROM SystemTable WHERE Name= 'AccessToken' ";
            cmd = new SqlCommand(SQL_Syntax, con);
            dr = cmd.ExecuteReader();
            dr.Read();
            result = dr[0].ToString();
            con.Close();
            
            //Outputs
            Console.WriteLine("DEBUG    :   access token fetched from the database in the getShopToken() is : " + result);
            return result;

        }
        
        //Next function description goes here
    }
}