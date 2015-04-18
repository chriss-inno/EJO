//This main contains database functions, 
//each databse dbms will have their own functions 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.IO;
using MySql.Data.MySqlClient;
using System.Net.Mail;

namespace commonServiceMonitoring
{
    class DBGeneral
    {
        //Mysql connect for CMF 

        public MySqlConnection connCMF_FCDB()
        {
            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = "server=localhost;user id=root;password= ;database=bmpl_cmf;";
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                conn.Close();

                SmtpClient smtpClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                smtpClient.Port = 25;
                smtpClient.Host = "192.168.150.22";

                mail.From = new MailAddress("service.delivery@bankm.com");

                mail.To.Add("innocent.christopher@bankm.com");
                //mail.To.Add("support@bankm.com");

                mail.Subject = "DATABASE CONNECTION ERROR NOTIFICATION IN connCMF_FCDB";
                mail.IsBodyHtml = true;
                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said Database failed to be connect with error message [" + ex.Message + "]<br></span></body></html>";
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpClient.Send(mail);

                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "DBLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_connCMF_FCDB: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }

            return conn;

        }

        //connect to MSSQL server 
        public SqlConnection MSSQL_connect()
        {
            SqlConnection conn = conn = new SqlConnection("Data Source=AUTOMAILSRV\\SQLEXPRESS;Initial Catalog=STTM_UPLOAD_FILE;Integrated Security=True"); //Change the connections here 

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                conn.Close();
                conn = null;

                SmtpClient smtpClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                smtpClient.Port = 25;
                smtpClient.Host = "192.168.150.22";
               
                mail.From = new MailAddress("service.delivery@bankm.com");
            
                mail.To.Add("innocent.christopher@bankm.com");
                //mail.To.Add("support@bankm.com");

                mail.Subject = "DATABASE CONNECTION ERROR NOTIFICATION IN MSSQL_connect";
                mail.IsBodyHtml = true;
                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said Database failed to be connect with error message [" + ex.Message + "]<br></span></body></html>";
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpClient.Send(mail);
                    
                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "DBLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_MSSQL_connect: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }
            
            return conn;

        }
        //connect to MSSQL server SWIFT  
        public SqlConnection MSSQL_SWIFT_connect()
        {
            SqlConnection conn = conn = new SqlConnection("Data Source=AUTOMAILSRV\\SQLEXPRESS;Initial Catalog=money_mapato;Integrated Security=True"); //Change the connections here 

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                conn.Close();
                conn = null;

                SmtpClient smtpClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                smtpClient.Port = 25;
                smtpClient.Host = "192.168.150.22";

                mail.From = new MailAddress("service.delivery@bankm.com");

                mail.To.Add("innocent.christopher@bankm.com");
                //mail.To.Add("support@bankm.com");

                mail.Subject = "DATABASE CONNECTION ERROR NOTIFICATION IN MSSQL_SWIFT_connect";
                mail.IsBodyHtml = true;
                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said Database failed to be connect with error message [" + ex.Message + "]<br></span></body></html>";
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpClient.Send(mail);

                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "DBLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_MSSQL_SWIFT_connect: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }

            return conn;

        }
        //connect to MSSQL server SWIFT  
        public SqlConnection MSSQL_SWIFT_Out_connect()
        {
            SqlConnection conn = conn = new SqlConnection("Data Source=AUTOMAILSRV\\SQLEXPRESS;Initial Catalog=money_mapato;Integrated Security=True"); //Change the connections here 
             
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                conn.Close();
                conn = null;

                SmtpClient smtpClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                smtpClient.Port = 25;
                smtpClient.Host = "192.168.150.22";

                mail.From = new MailAddress("service.delivery@bankm.com");

                mail.To.Add("innocent.christopher@bankm.com");
                //mail.To.Add("support@bankm.com");

                mail.Subject = "DATABASE CONNECTION ERROR NOTIFICATION IN MSSQL_SWIFT_Out_connect";
                mail.IsBodyHtml = true;
                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said Database failed to be connect with error message [" + ex.Message + "]<br></span></body></html>";
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpClient.Send(mail);

                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "DBLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_MSSQL_SWIFT_Out_connect: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }

            return conn;

        }

        //connect to MSSQL SWIFT server 
        public SqlConnection MSSQL_opp_connect()
        {
            SqlConnection conn = conn = new SqlConnection("Data Source=BANKMPORTAL\\QLEXPRESS;Initial Catalog=money_mapato;Integrated Security=True"); //Change the connections here 
             
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                conn.Close();
                conn = null;

                SmtpClient smtpClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                smtpClient.Port = 25;
                smtpClient.Host = "192.168.150.22";

                mail.From = new MailAddress("service.delivery@bankm.com");

                mail.To.Add("innocent.christopher@bankm.com");
                //mail.To.Add("support@bankm.com");

                mail.Subject = "DATABASE CONNECTION ERROR NOTIFICATION IN MSSQL_opp_connect";
                mail.IsBodyHtml = true;
                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said Database failed to be connect with error message [" + ex.Message + "]<br></span></body></html>";
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpClient.Send(mail);

                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "DBLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_MSSQL_opp_connect: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }

            return conn;

        }
        //connect to MSSQL SWIFT server 
        public SqlConnection MSSQL_MoneyWireles_Success_connect()
        {
            SqlConnection conn = conn = new SqlConnection("Data Source=AUTOMAILSRV\\SQLEXPRESS;Initial Catalog=TXNFUNDSTRANSFER;Integrated Security=True"); //Change the connections here 

            try 
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                conn.Close();
                conn = null;

                SmtpClient smtpClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                smtpClient.Port = 25;
                smtpClient.Host = "192.168.150.22";

                mail.From = new MailAddress("service.delivery@bankm.com");

                mail.To.Add("innocent.christopher@bankm.com");
                //mail.To.Add("support@bankm.com");

                mail.Subject = "DATABASE CONNECTION ERROR NOTIFICATION IN MSSQL_MoneyWireles_Success_connect";
                mail.IsBodyHtml = true;
                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said Database failed to be connect with error message [" + ex.Message + "]<br></span></body></html>";
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpClient.Send(mail);

                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "DBLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_MSSQL_MoneyWireles_Success_connect: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }

            return conn;

        }

        //Function connection to the oracle dbms 

        public OracleConnection Oracle_connect()
        {

            OracleConnection conn = new OracleConnection("Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.1.10)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = bankmlive)));User ID=fcdblive;Password=fcdblive;Unicode=True");
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {

                conn.Close();

                SmtpClient smtpClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                smtpClient.Port = 25;
                smtpClient.Host = "192.168.150.22";

                mail.From = new MailAddress("service.delivery@bankm.com");

                mail.To.Add("innocent.christopher@bankm.com");
                //mail.To.Add("support@bankm.com");

                mail.Subject = "DATABASE CONNECTION ERROR NOTIFICATION IN Oracle_connect";
                mail.IsBodyHtml = true;
                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said Database failed to be connect with error message [" + ex.Message + "]<br></span></body></html>";
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpClient.Send(mail);

                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "DBLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_Oracle_connect: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }

            return conn;
        }

        //function connection to the MYsql dbms 
        public void MYSQL_connect(string Host, string Userid, string Password, string Database)
        {

            try
            {

            }
            catch (Exception ex)
            {
                SmtpClient smtpClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                smtpClient.Port = 25;
                smtpClient.Host = "192.168.150.22";

                mail.From = new MailAddress("service.delivery@bankm.com");

                mail.To.Add("innocent.christopher@bankm.com");
                //mail.To.Add("support@bankm.com");

                mail.Subject = "DATABASE CONNECTION ERROR NOTIFICATION IN MYSQL_connect";
                mail.IsBodyHtml = true;
                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said Database failed to be connect with error message [" + ex.Message + "]<br></span></body></html>";
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpClient.Send(mail);

                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "DBLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_MYSQL_connect: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }
           

        }


    }
}
