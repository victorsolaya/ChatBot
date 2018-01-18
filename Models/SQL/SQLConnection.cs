using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LuisBot.Models.SQL
{
    public class SQLConnection
    {
        SqlConnection sqlconnection;
        public void GetConnection()
        {
            try
            {
                string connectionString = null;
                
                connectionString = "Data Source=feb-sql2;Initial Catalog=trakdba;User ID=trakdba;Password=aberdeen";
                sqlconnection = new SqlConnection(connectionString);
                sqlconnection.Open();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<string> GetUsers()
        {
            //SqlCommand getCommand;
            List<string> users = new List<string> { "John", "Peter" }; ;
            string sqlStatement = "Here your statement";
            //getCommand = new SqlCommand(sqlStatement, sqlconnection);
            return users;
        }
    }
}