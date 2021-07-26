using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class DBController : ApiController
    {
        public DataTable GetContacts()
        {
            string query = @"select ContactID, ContactEmail, ContactPhoneNumber from dbo.Contacts";
            DataTable table = new DataTable();
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["Messages"].ConnectionString))
            using (var cmd = new SqlCommand(query, con))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.Text;
                da.Fill(table);
            }

            return  table;
        }

        public Boolean InsertIntoMessageTextTable(int messageTopic, int contactID, string text)
        {
            try{
                Console.WriteLine(messageTopic.ToString());
                string query = @"
                    insert into dbo.MessageText values
                    (
                        '" + messageTopic + @"',
                        '" + contactID + @"',
                        '" + text + @"'
                    )";

                DataTable table = new DataTable();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["Messages"].ConnectionString))
                using (var cmd = new SqlCommand(query, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.Text;
                    da.Fill(table);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean InsertIntoContactsTable(String Name, String Email, String PhoneNumber)
        {
            try
            {
                string query = @"
                    insert into dbo.Contacts values
                    (
                        '" + Name + @"',
                        '" + Email + @"',
                        '" + PhoneNumber + @"'
                    )";

                DataTable table = new DataTable();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["Messages"].ConnectionString))
                using (var cmd = new SqlCommand(query, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.Text;
                    da.Fill(table);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string Post(Data data)
        {
            DataTable existingTable = GetContacts();
            DataRow[] rows = existingTable.Select();
            int contactID = 0;
            int lastContactID = (int)rows[rows.Length-1]["ContactID"];
            for (int i = 0; i < rows.Length; i++)
            {
                if ((String.Compare(rows[i]["ContactEmail"].ToString(), data.Email.ToString()) == 0) | (String.Compare(rows[i]["ContactPhoneNumber"].ToString(), data.PhoneNumber.ToString()) == 0))
                {
                    contactID = (int)rows[i]["ContactID"];
                }
            }

            if (contactID != 0)
            {
                if (InsertIntoMessageTextTable(data.MessageTopic, contactID, data.MessageText))
                {
                    string res = data.Name + "\n" + data.Email + "\n" + data.PhoneNumber + "\n" + data.MessageTopic + "\n" + data.MessageText;
                    return res;
                }
                else
                {
                    return "Что-то не так с отправкой вашего сообщения!";
                }
            }
            else
            {

                if (InsertIntoContactsTable(data.Name, data.Email, data.PhoneNumber) & InsertIntoMessageTextTable(data.MessageTopic, lastContactID+1, data.MessageText))
                {
                    string res = data.Name + "\n" + data.Email + "\n" + data.PhoneNumber + "\n" + data.MessageTopic + "\n" + data.MessageText;
                    return res;
                }
                else
                {

                    return  "Что-то не так с отправкой Вашего контакта и сообщения!";
                }
            }
        }
    }
}
