using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;
using System.Net.Mail;
using System.IO;
using System.Diagnostics;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using SautinSoft;

namespace commonServiceMonitoring
{
    class MOWILES
    {
                    
       
        public MOWILES()
        {
            


        }

        #region Money Wireless Failure
        public void processFailure(OracleConnection db)
        {
            try
            {
                string uPloadRef = "";
                
                OracleCommand cmd = db.CreateCommand();
                cmd.CommandText = "SELECT IDFCATREF	FROM txnfundstransfer WHERE IDFCATREF IN ( SELECT IDFCATREF FROM admintxnunauthdata WHERE IDFCATREF IN (SELECT IDFCATREF FROM txnfundstransfer WHERE CODSTATUS = 'RBH'))  AND DATSEND >= '24-NOV-2014'";

                OracleDataReader dr = cmd.ExecuteReader();
                


                while (dr.Read())
                {
                    uPloadRef = dr["IDFCATREF"].ToString();
                    


                    SqlConnection mDb= new SqlConnection("Data Source=AUTOMAILSRV\\SQLEXPRESS;Initial Catalog=TXNFUNDSTRANSFER;Integrated Security=True");
                    SqlCommand custCMD =mDb.CreateCommand();
                    mDb.Open();
                    custCMD.CommandText = "select * from TXNFUNDSTRANSFER WHERE IDFCATREF = '" + uPloadRef + "'";
                    
                    SqlDataReader customerdetails=custCMD.ExecuteReader(); 


                    customerdetails.Read();

                    if (!customerdetails.HasRows)
                    {
                       // System.Windows.Forms.MessageBox.Show(uPloadRef);
                        //SMTP SERVER  
                        SmtpClient SmtpServer = new SmtpClient();
                        MailMessage mail = new MailMessage();
                        SmtpServer.Port = 25;
                        SmtpServer.Host = "192.168.150.22";

                        mail.From = new MailAddress("Money.Wireless@bankm.com", "Money.Wireless");
                        //mail.To.Add("innocent.christopher@bankm.com");
                        mail.To.Add("products@bankm.com");
                        mail.Bcc.Add("payments@bankm.com,support@bankm.com");
                        mail.Subject = "Money.Wireless FT Failed Transaction";

                        mail.IsBodyHtml = true;
                        mail.Body = "";
                        mail.Body = mail.Body + "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>Product team,<br><br> Please find the below summary table of Funds Transfer transaction(s) got rejected on Money.Wireless for your action. <br><br></span>";


                        mail.Body = mail.Body + "<table width='50%' border='1' cellpadding='0' cellspacing='0' bgcolor='white'>";
                        mail.Body = mail.Body + "<tr>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>BANK REF</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>USER REF</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>BASE #</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>SENDER</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>RECEIVER</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>CCY</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>AMOUNT</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>BEN NAME</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>BEN ADDRESS1</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>BEN ADDRESS2</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>PAYMT DETAILS</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>DESTSORTCODE</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>INPUTER</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>AUTHORIZER</span></strong></td>";
                        mail.Body = mail.Body + "</tr>";

                        //New connection 
                        OracleCommand cmd1= db.CreateCommand();

                        cmd1.CommandText = "SELECT IDFCATREF, IDUSERREFERENCE,IDCORPORATE,IDINITIATOR,DATINITIATION,IDAUTHORIZER1,DATAUTHORIZE1,IDSRCACCT,NUMAMOUNT,CODTRNCURR,TXTDESTACCT,TXTBENNAME,TXTBENADDRESS1,TXTBENADDRESS2,TXTPAYMTDETAILS1,       DESTSORTCODE	FROM txnfundstransfer where (IDFCATREF IN ( SELECT IDFCATREF FROM admintxnunauthdata WHERE IDFCATREF IN (SELECT IDFCATREF FROM txnfundstransfer WHERE CODSTATUS = 'RBH'))) and (idfcatref = '" + uPloadRef + "')  AND DATSEND >= '24-NOV-2014'";
                        OracleDataReader odr = cmd1.ExecuteReader();

                        while (odr.Read())
                        {
                            mail.Body = mail.Body + "<tr>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDFCATREF"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDUSERREFERENCE"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDCORPORATE"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDSRCACCT"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["TXTDESTACCT"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["CODTRNCURR"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["NUMAMOUNT"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["TXTBENNAME"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["TXTBENADDRESS1"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["TXTBENADDRESS2"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["TXTPAYMTDETAILS1"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["DESTSORTCODE"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDINITIATOR"].ToString() + "|" + odr["DATINITIATION"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDAUTHORIZER1"].ToString() + "|" + odr["DATAUTHORIZE1"].ToString() + "</span></td>";

                            mail.Body = mail.Body + "</tr>";

                        }
                        odr.Close();

                        mail.Body = mail.Body + "</table>";


                        mail.Body = mail.Body + "<br>+nbsp;</br>";

                        mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;


                        SmtpServer.Send(mail);


                        //System.Windows.Forms.MessageBox.Show(mail.Body);
                        SqlConnection myconnection = new SqlConnection("Data Source=AUTOMAILSRV\\SQLEXPRESS;Initial Catalog=TXNFUNDSTRANSFER;Integrated Security=True");
                        myconnection.Open();
                        SqlCommand mycommand =myconnection.CreateCommand();
                        mycommand.CommandText="insert into TXNFUNDSTRANSFER([IDFCATREF]) values ('" + dr["IDFCATREF"].ToString() + "')";
                        mycommand.ExecuteNonQuery();
                        myconnection.Close();


                        //Creating file logs 
                        DateTime dt = new DateTime();
                        dt = DateTime.Now;
                        string logfilename = "MOWILESLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                        using (StreamWriter w = File.AppendText(logfilename))
                        {
                            string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_processFailure:Money.Wireless FT Failed Transaction with IDFCATREF = '" + uPloadRef + "'";
                            w.WriteLine(filelog);
                        }

                            
                    }
                    customerdetails.Close();
                    mDb.Close();

                } //End while 

                dr.Close(); //Close reader

            }
            catch(Exception ex )
            {
                SmtpClient smtpClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                smtpClient.Port = 25;
                smtpClient.Host = "192.168.150.22";

                mail.From = new MailAddress("service.delivery@bankm.com");

                mail.To.Add("innocent.christopher@bankm.com");
                //mail.To.Add("support@bankm.com");

                mail.Subject = "CONNECTION ERROR NOTIFICATION IN Money Wireless Failure";
                mail.IsBodyHtml = true;
                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the Money Wireless Failure failed  with error message [" + ex.Message + "]<br></span></body></html>";
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpClient.Send(mail);

                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "MOWILESLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_processFailure: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }
        #endregion 
        #region MONEY WILESS SUCCESS
        public void processMoneyWirelessSuccess(OracleConnection db)
        {
            try
            {
                OracleCommand cmd = db.CreateCommand();
                cmd.CommandText = "SELECT IDFCATREF	FROM txnfundstransfer WHERE IDFCATREF IN ( SELECT IDFCATREF FROM admintxnunauthdata WHERE IDFCATREF IN (SELECT IDFCATREF FROM txnfundstransfer WHERE CODSTATUS = 'ABH'))  AND DATSEND >= '24-NOV-2014'";
                OracleDataReader dr = cmd.ExecuteReader();
                
                string uPloadRef = "";

                while (dr.Read())
                {
                    uPloadRef = dr["IDFCATREF"].ToString();
                    SqlConnection mDb = new SqlConnection("Data Source=AUTOMAILSRV\\SQLEXPRESS;Initial Catalog=TXNFUNDSTRANSFER;Integrated Security=True");
                    mDb.Open();
                    SqlCommand mCmd = mDb.CreateCommand();

                    mCmd.CommandText = "select * from TXNFUNDSTRANSFER WHERE IDFCATREF = '" + dr["IDFCATREF"].ToString() + "'";
                    SqlDataReader customerdetails = mCmd.ExecuteReader();

                    customerdetails.Read();

                    if (!customerdetails.HasRows)
                    {
                        //System.Windows.Forms.MessageBox.Show(uPloadRef);

                        //Initiating smtp 
                        SmtpClient SmtpServer = new SmtpClient();
                        MailMessage mail = new MailMessage();
                        SmtpServer.Port = 25;
                        SmtpServer.Host = "192.168.150.22";

                        mail = new MailMessage();
                        mail.From = new MailAddress("Money.Wireless@bankm.com", "Money.Wireless");
                        //mail.To.Add("innocent.christopher@bankm.com");
                        mail.To.Add("products@bankm.com");
                        mail.Bcc.Add("payments@bankm.com");
                        mail.Bcc.Add("support@bankm.com");
                        mail.Subject = "Successful Money.Wireless FT Transaction";

                        
                        mail.IsBodyHtml = true;
                        mail.Body = "";
                        mail.Body = mail.Body + "<html><head></head><body><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>Product team,<br><br> Please find the below summary table of Funds Transfer transaction(s) accepted by the bank on Money.Wireless for your information. <br> <br></span>";


                        mail.Body = mail.Body + "<table width='50%' border='1' cellpadding='0' cellspacing='0' bgcolor='white'>";
                        mail.Body = mail.Body + "<tr>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>BANK REF</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>USER REF</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>FREXCUBE REF</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>BASE #</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>SENDER</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>RECEIVER</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>CCY</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>AMOUNT</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>BEN NAME</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>BEN ADDRESS1</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>BEN ADDRESS2</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>PAYMT DETAILS</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>DESTSORTCODE</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>INPUTER</span></strong></td>";
                        mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>AUTHORIZER</span></strong></td>";
                        mail.Body = mail.Body + "</tr>";

                         OracleCommand ocmd = db.CreateCommand();
                        ocmd.CommandText = "SELECT IDFCATREF, TXTHOSTREF,IDUSERREFERENCE,IDCORPORATE,IDINITIATOR,DATINITIATION,IDAUTHORIZER1,DATAUTHORIZE1,IDSRCACCT,NUMAMOUNT,CODTRNCURR,TXTDESTACCT,TXTBENNAME,TXTBENADDRESS1,TXTBENADDRESS2,TXTPAYMTDETAILS1,       DESTSORTCODE	FROM txnfundstransfer where (IDFCATREF IN ( SELECT IDFCATREF FROM admintxnunauthdata WHERE IDFCATREF IN (SELECT IDFCATREF FROM txnfundstransfer WHERE CODSTATUS = 'ABH'))) and (idfcatref = '" + uPloadRef + "')  AND DATSEND >= '24-NOV-2014'";

                        OracleDataReader odr = ocmd.ExecuteReader();

                        while (odr.Read())
                        {
                            mail.Body = mail.Body + "<tr>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDFCATREF"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDUSERREFERENCE"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["TXTHOSTREF"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDCORPORATE"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDSRCACCT"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["TXTDESTACCT"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["CODTRNCURR"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["NUMAMOUNT"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["TXTBENNAME"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["TXTBENADDRESS1"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["TXTBENADDRESS2"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["TXTPAYMTDETAILS1"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["DESTSORTCODE"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDINITIATOR"].ToString() + "|" + odr["DATINITIATION"].ToString() + "</span></td>";
                            mail.Body = mail.Body + "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + odr["IDAUTHORIZER1"].ToString() + "|" + odr["DATAUTHORIZE1"].ToString() + "</span></td>";

                            mail.Body = mail.Body + "</tr>";
                        }

                        //close reader 
                        odr.Close();

                        mail.Body = mail.Body + "</table>";


                        mail.Body = mail.Body + "<br>&nbsp;</br>";

                        mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                        SmtpServer.Send(mail);

                        SqlConnection cn= new SqlConnection("Data Source=AUTOMAILSRV\\SQLEXPRESS;Initial Catalog=TXNFUNDSTRANSFER;Integrated Security=True");
                        SqlCommand ocmd2 = cn.CreateCommand();
                        cn.Open();
                        ocmd2.CommandText = "insert into TXNFUNDSTRANSFER([IDFCATREF]) values ('" + dr["IDFCATREF"].ToString() + "')";
                        ocmd2.ExecuteNonQuery();
                        cn.Close();

                        //Creating file logs 
                        DateTime dt = new DateTime();
                        dt = DateTime.Now;
                        string logfilename = "MOWILESLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                        using (StreamWriter w = File.AppendText(logfilename))
                        {
                            string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_Successiful_processMoneyWirelessSuccess:Successful Money.Wireless FT Transactionn with IDFCATREF = '" + uPloadRef + "'";
                            w.WriteLine(filelog);
                        }

                    }
                    customerdetails.Close();
                    mDb.Close();


                }
                dr.Close();
            }
            catch(Exception ex)
            {
                SmtpClient smtpClient = new SmtpClient();
                MailMessage mail = new MailMessage();
                smtpClient.Port = 25;
                smtpClient.Host = "192.168.150.22";

                mail.From = new MailAddress("service.delivery@bankm.com");

                mail.To.Add("innocent.christopher@bankm.com");
                //mail.To.Add("support@bankm.com");

                mail.Subject = "ERROR NOTIFICATION IN MONEY WILESS SUCCESS";
                mail.IsBodyHtml = true;
                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above,MONEY WILESS SUCCESS failed with error message [" + ex.Message + "]<br></span></body></html>";
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpClient.Send(mail);

                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "MOWILESLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_processMoneyWirelessSuccess: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }
        #endregion

    }
}
