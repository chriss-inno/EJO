/*This class manages the SWIFT incomming and outgin 
 * The class was witten in 12.02.2015
 * Using the reference of the current requirements 
 * 
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;
using System.Data.SqlClient;
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
    class SWIFT
    {
        DBGeneral dblog;
        #region Swiftlogs
        public void writteLogs(string swiftCopy, string category)
        {

            try
            {
                dblog = new DBGeneral();
                SqlCommand cmd = dblog.MSSQL_SWIFT_connect().CreateCommand();

                string reference_no = "";
                string file_path = "";

                string sent_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                string[] arrFile = swiftCopy.Split('_');
                file_path = swiftCopy;

                reference_no = arrFile[1];

                cmd.CommandText = "INSERT INTO swift_logs(reference_no,file_path,sent_at,category)VALUES('" + reference_no + "','" + file_path + "','" + sent_at + "','" + category + "')";
                cmd.ExecuteNonQuery();

                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "SWIFTEmailLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Swift email Logstored " + file_path;
                    w.WriteLine(filelog);
                }

                cmd.Dispose(); //Release all the resources 


            }
            catch (Exception ex)
            {
                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "SWIFTEmailLogsError" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Swift email Log stored error" + ex.Message.ToString();
                    w.WriteLine(filelog);
                }


            }


        }
        #endregion
        //Process incomming swift 
        #region SWIFT incoming
             
            public void processIncomingSwift( string sourceDr, string desinationDr, SqlConnection db )
            {
                string coppiedFile = "";
                string oldfile = "";
               
          
             try
             {
                DirectoryInfo SourceDir = new DirectoryInfo(sourceDr);
                DirectoryInfo DestDir = new DirectoryInfo(desinationDr);
               
                //Copy all files from source to destination 
                FileInfo[] SourceFileNames = SourceDir.GetFiles("*.*");
                foreach (FileInfo fi in SourceFileNames)
                {
                    coppiedFile = desinationDr + fi.Name;
                    oldfile = sourceDr + fi.Name;

                    

                    if (!File.Exists(coppiedFile))
                    {
                        File.Copy(oldfile, coppiedFile);

                        //System.Windows.Forms.MessageBox.Show(oldfile);

                        //Check for account first 
                       PdfFocus p = new PdfFocus();
                        Byte[] pdf = File.ReadAllBytes(coppiedFile);
                        List result = new List();
                        string PdfText = "";
                        p.OpenPdf(pdf);
                        if (p.PageCount > 0)
                        {
                            PdfText = p.ToText(1, 1);

                           //find the account number
                            string startWith = "59:";
                            int postionFound = PdfText.IndexOf(startWith);
                            
                            string ACNO ="";
                            string ommitedstr="Beneficiary Customer-Name & Addr";

                            string strAfter = PdfText.Substring(postionFound + ommitedstr.Length + startWith.Length + 7);
                            ACNO = strAfter.Substring(0, 10);
                           // System.Windows.Forms.MessageBox.Show("Account number is " + ACNO);
                            
                            //Get customer details based on the account 
                            
                            SqlCommand cmd = db.CreateCommand();
                            cmd.CommandText = "SELECT * FROM customer WHERE customeraccount ='" + ACNO + "'";
                            SqlDataReader dr = cmd.ExecuteReader();
                            dr.Read();
                            if (dr.HasRows)
                            {
                                string email = dr["customeremail"].ToString();
                                string customerdetail = dr["customername"].ToString();
                                //send email 

                                dr.Close(); //close datareader 

                                //Get swift users
                                db.Close();
                                db.Open();
                                SqlCommand cmd2 = db.CreateCommand();
                                cmd2.CommandText = "SELECT * FROM swift_Users_success";
                                SqlDataReader dr2 = cmd2.ExecuteReader();

                                string swusers = "";

                                while (dr2.Read())
                                {
                                    swusers = dr2.GetString(1);

                                }
                                dr2.Close();

                                SmtpClient smtpClient = new SmtpClient();
                                MailMessage mail = new MailMessage();
                                smtpClient.Port = 25;
                                smtpClient.Host = "192.168.150.22";

                                MailAddress mailAddress = new MailAddress("service.delivery@bankm.com");

                                Attachment attachment = new Attachment(oldfile);
                                mail.Attachments.Add(attachment);
                               mail.To.Add(email);
                               mail.Bcc.Add(swusers);
                               // mail.Bcc.Add("innocent.christopher@bankm.com");
                            
                                mail.From = mailAddress;
                               
                                mail.Subject = "" + customerdetail + " INCOMING SWIFT COPY";

                                mail.IsBodyHtml = true;
                                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Dear Valued Customer,<br><br>Please find attached the SWIFT copy processed/received on your behalf. We thank you for channeling your business through us.<br>It has been pleasure serving you and we look forward to your continued patronage.<br><br>If there's any query regarding this payment, please do not hesitate to contact me directly and I will be pleased to assist you.<br></span></body></html>";

                                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                               smtpClient.Send(mail);
                                //System.Windows.Forms.MessageBox.Show(mail.Body);
                                attachment.Dispose();

                                //Unecessary connection to incomming logs in the future can be removed 
                                SqlConnection conn = new SqlConnection("Data Source=AUTOMAILSRV;Initial Catalog=incomingsms;Integrated Security=True");
                                conn.Open();
                                SqlCommand cmd4 = conn.CreateCommand();
                                cmd4.CommandText = "insert into incominglog([customername],[mailsentdate],[mailsenttime],[mailstatus]) values ('" + customerdetail + "','" + DateTime.Now.ToString("dd/MM/yyy") + "','" + DateTime.Now.ToString("hh.mm") + "','NOTSENT')";
                                cmd4.ExecuteNonQuery();
                                conn.Close();

                                writteLogs(fi.Name, "incomming swift"); //Write to logs 

                                //Creating file logs 
                                DateTime dtin = new DateTime();
                                dtin = DateTime.Now;
                                string logfilenamein = "SWIFTLogs" + dtin.Year + dtin.ToString("-MM-dd") + ".fcdb";
                                using (StreamWriter w = File.AppendText(logfilenamein))
                                {
                                    string filelog = dtin.Year + dtin.ToString("-MM-dd hh:mm:ss_") + "Processed_Successiful_processIncomingSwift: " + coppiedFile;
                                    w.WriteLine(filelog);
                                }
                            }
                            else
                            {
                                db.Close();
                                db.Open();
                                SqlCommand cmd3 = db.CreateCommand();
                                cmd3.CommandText = "SELECT * FROM swift_Users_failure";
                                SqlDataReader dr3 = cmd3.ExecuteReader();
                                string swusers = "";
                                while (dr3.Read())
                                {
                                    swusers = dr3.GetString(1);
                                }
                                dr3.Close();

                                SmtpClient smtpClient = new SmtpClient();
                                MailMessage mail = new MailMessage();
                                smtpClient.Port = 25;
                                smtpClient.Host = "192.168.150.22";

                                MailAddress mailAddress = new MailAddress("service.delivery@bankm.com");

                                Attachment attachment = new Attachment(oldfile);
                                mail.Attachments.Add(attachment);
                                mail.To.Add(swusers);
                                //mail.To.Add("innocent.christopher@bankm.com");
                                mail.Bcc.Add("service.delivery@bankm.com");
                                mail.From = mailAddress;
                                
                                mail.Subject = "INCOMING SWIFT COPY SENDING FAILURE - CUSTOMER ACCOUNT DOES NOT EXIST";
                                mail.IsBodyHtml = true;

                                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Dear Valued Customer,<br><br>Please find the attached SWIFT copy processed/receive which has not been sent to the intended customer,<br>due to non maintenance of account number in the database.<br></span></body></html>";
                                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                smtpClient.Send(mail);
                                //System.Windows.Forms.MessageBox.Show(mail.Body);
                                attachment.Dispose();


                                //Unecessary connection to incomming logs in the future can be removed 
                                SqlConnection conn = new SqlConnection("Data Source=AUTOMAILSRV;Initial Catalog=incomingsms;Integrated Security=True");
                                conn.Open();
                                SqlCommand cmd4 = conn.CreateCommand();
                                cmd4.CommandText = "insert into incominglog([customername],[mailsentdate],[mailsenttime],[mailstatus]) values ('NILL','" + DateTime.Now.ToString("dd/MM/yyy") + "','" + DateTime.Now.ToString("hh.mm") + "','NOTSENT')";
                                cmd4.ExecuteNonQuery();
                                conn.Close();

                                //Creating file logs 
                                DateTime dtin = new DateTime();
                                dtin = DateTime.Now;
                                string logfilenamein = "SWIFTLogs" + dtin.Year + dtin.ToString("-MM-dd") + ".fcdb";
                                using (StreamWriter w = File.AppendText(logfilenamein))
                                {
                                    string filelog = dtin.Year + dtin.ToString("-MM-dd hh:mm:ss_") + "Processed_Failed_processIncomingSwift: " + coppiedFile;
                                    w.WriteLine(filelog);
                                }

                                writteLogs(fi.Name, "Incomming Swift Failure"); //Write to logs 
                           
                            }
                            dr.Close();
                        }

                       
                    }
                    
                    }
                    
              }
              
               catch(Exception ex )
                {

                    SmtpClient smtpClient = new SmtpClient();
                    MailMessage mail = new MailMessage();
                    smtpClient.Port = 25;
                    smtpClient.Host = "192.168.150.22";
                    Attachment attachment = new Attachment(oldfile);


                    mail.From = new MailAddress("service.delivery@bankm.com");
                    mail.Attachments.Add(attachment);
                    mail.To.Add("innocent.christopher@bankm.com");
                    mail.Bcc.Add("adolph.mwakalinga@bankm.com");

                    mail.Subject = "ERROR NOTIFICATION IN processIncomingSwift";
                    mail.IsBodyHtml = true;
                    mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said document failed to be processed with error message [" + ex.Message + "]<br></span></body></html>";
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    smtpClient.Send(mail);
                    //System.Windows.Forms.MessageBox.Show(mail.Body);
                    attachment.Dispose();

                    //Creating file logs 

                   //Creating file logs 
                    DateTime dt = new DateTime();
                    dt = DateTime.Now;
                    string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                    using (StreamWriter w = File.AppendText(logfilename))
                    {
                        string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_processIncomingSwift: " + ex.Message;
                        w.WriteLine(filelog);
                    }
                }
                 
            }
        #endregion

        //Process Outgoin Swift 
        #region SWIFT outgoin 
         
            public void processOutgoingSwift(string sourceDr, string desinationDr, string desinationDr2, SqlConnection db)
            {

                string coppiedFile = "";
                string oldfile = "";
           try
              {

                
                //Fetch all files in source folder
                DirectoryInfo dis = new DirectoryInfo(sourceDr);
                FileInfo[] fileNames = dis.GetFiles("*.*");

                foreach (System.IO.FileInfo fi in fileNames)
                {
                     coppiedFile = desinationDr + fi.Name;
                     oldfile = sourceDr + fi.Name;
                    if (!File.Exists(coppiedFile))
                    {
                        File.Copy(oldfile, coppiedFile); //Copy file to the first destination folder

                        //Get client details 
                       
                        

                        //Process file 
                        
                        //Check for account first 
                       PdfFocus p = new PdfFocus();
                       Byte[] pdf = File.ReadAllBytes(oldfile);
                        List result = new List();
                        string PdfText = "";
                        p.OpenPdf(pdf);
                        if (p.PageCount > 0)
                        {
                            PdfText = p.ToText(1, 1);

                           //find the account number
                            string startWith = "50K:";
                            int postionFound = PdfText.IndexOf(startWith);
                            
                            string ACNO ="";
                            string ommitedstr="Ordering Customer-Name & Address";

                            string strAfter = PdfText.Substring(postionFound + ommitedstr.Length + startWith.Length + 7);
                            ACNO = strAfter.Substring(0, 10);
                            
                            
                            //Get customer details based on the account 

                            SqlCommand cmd = db.CreateCommand();
                            cmd.CommandText = "SELECT * FROM customer WHERE customeraccount ='" + ACNO + "'";
                            SqlDataReader dr = cmd.ExecuteReader();
                            dr.Read();
                            if (dr.HasRows)
                            {
                                string email = dr["customeremail"].ToString();
                                string customerdetail = dr["customername"].ToString();
                               
                                //Get swift users
                                db.Close();
                                db.Open();

                                SqlCommand cmd2 = db.CreateCommand();
                                cmd2.CommandText = "SELECT * FROM swift_Users_success";
                               
                                SqlDataReader dr2 = cmd2.ExecuteReader();
                                                               
                                string swusers = "";

                                while (dr2.Read())
                                {
                                    swusers = dr2.GetString(1);

                                }
                                dr2.Close();
                               
                                SmtpClient smtpClient = new SmtpClient();
                                MailMessage mail = new MailMessage();
                                smtpClient.Port = 25;
                                smtpClient.Host = "192.168.150.22";

                                MailAddress mailAddress = new MailAddress("service.delivery@bankm.com");

                                Attachment attachment = new Attachment(oldfile);
                                mail.Attachments.Add(attachment);
                                mail.To.Add(email);
                                mail.Bcc.Add(swusers);
                                //mail.To.Add("innocent.christopher@bankm.com");
                                mail.From = mailAddress;
                               
                                mail.Subject = "" + customerdetail + " SWIFT COPY";
                                mail.IsBodyHtml = true;
                                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Dear Valued Customer,<br><br>Please find attached the SWIFT copy processed/received on your behalf. We thank you for channeling your business through us.<br>It has been pleasure serving you and we look forward to your continued patronage.<br><br>If there's any query regarding this payment, please do not hesitate to contact me directly and I will be pleased to assist you.<br></span></body></html>";
                                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                               // System.Windows.Forms.MessageBox.Show(mail.Body);
                               smtpClient.Send(mail);
                                attachment.Dispose();
                               
                               
                                //Unecessary connection to incomming logs in the future can be removed 
                                SqlConnection conn = new SqlConnection("Data Source=AUTOMAILSRV;Initial Catalog=outgoingsms;Integrated Security=True");
                                conn.Open();
                                SqlCommand cmd4 = conn.CreateCommand();
                               cmd4.CommandText = "insert into outgoinglog([customername],[mailsentdate],[mailsenttime],[mailstatus]) values ('"+ customerdetail + "','" + DateTime.Now.ToString("dd/MM/yyy") + "','" + DateTime.Now.ToString("hh.mm") + "','SENT')";
                               cmd4.ExecuteNonQuery(); 
                                conn.Close();

                              

                                //Creating file logs 
                                DateTime dt = new DateTime();
                                dt = DateTime.Now;
                                string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                using (StreamWriter w = File.AppendText(logfilename))
                                {
                                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_processOutgoingSwift: " + coppiedFile;
                                    w.WriteLine(filelog);
                                }
                                writteLogs(fi.Name, "Outgoing Swift"); //Write to logs 

                            }
                            else
                            {
                                string coppiedFileNONCUST = desinationDr2 + fi.Name;
                                

                                if (!File.Exists(coppiedFileNONCUST))
                                {
                                    File.Copy(oldfile, coppiedFileNONCUST); //Copy file for non customers

                                    
                                
                                }
                            }

                            dr.Close();
                        }

                        
                    }
                    

                    }
                }
               
              catch (Exception ex)
                {

                    SmtpClient smtpClient = new SmtpClient();
                    MailMessage mail = new MailMessage();
                    smtpClient.Port = 25;
                    smtpClient.Host = "192.168.150.22";
                    Attachment attachment = new Attachment(oldfile);


                    mail.From = new MailAddress("service.delivery@bankm.com");
                    mail.Attachments.Add(attachment);
                    mail.To.Add("innocent.christopher@bankm.com");
                    mail.To.Add("adolph.mwakalinga@bankm.com");

                    mail.Subject = "ERROR NOTIFICATION IN processOutgoingSwift";
                    mail.IsBodyHtml = true;
                    mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said document failed to be processed with error message [" + ex.Message + "]<br></span></body></html>";
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                   smtpClient.Send(mail);
                    //System.Windows.Forms.MessageBox.Show(mail.Body);
                    attachment.Dispose();

                  //Creating file logs 
                    DateTime dt = new DateTime();
                    dt = DateTime.Now;
                    string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                    using (StreamWriter w = File.AppendText(logfilename))
                    {
                        string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_processOutgoingSwift: " + ex.Message;
                        w.WriteLine(filelog);
                    }
                }
                
                
            }
        #endregion
        #region Swift outgoing non customers
            public void processOutgoingSwiftNonCustomer(string sourceDr, string desinationDr, SqlConnection db)
            {
                string error_in_file = "";
                string coppiedFile = "";
                string oldfile = "";
               try
              {

                    string x, userEmails;
                    userEmails = x = "";
                    string email = "";

                    //Fetch all files in source folder
                    DirectoryInfo dis = new DirectoryInfo(sourceDr);
                    FileInfo[] fileNames = dis.GetFiles("*.*");
                    foreach (System.IO.FileInfo fi in fileNames)
                    {
                        coppiedFile = desinationDr + fi.Name;
                         oldfile = sourceDr + fi.Name;

                        //System.Windows.Forms.MessageBox.Show(oldfile);

                        error_in_file = oldfile;
                        if (!File.Exists(coppiedFile))
                        {
                            //copy the files first 

                           

                            PdfFocus p = new PdfFocus();
                            Byte[] pdf = File.ReadAllBytes(oldfile);
                            List result = new List();
                            string text = "";
                            p.OpenPdf(pdf);
                            if (p.PageCount > 0)
                            {
                                text = p.ToText(1, 1);
                                //show text 
                                if (!text.Equals(""))
                                {
                                    if (text.Contains("RUPEE"))
                                    {
                                        if (text.Contains("72:"))
                                        {
                                           
                                            if (!File.Exists(coppiedFile))
                                            {
                                                File.Copy(oldfile, coppiedFile);
                                            }
                                            //check for mail 
                                            if (text.Contains("/BNF/"))
                                            {

                                                string cut_at1 = "/BNF/";
                                                string cut_at2 = "--";
                                                string address = "Address";
                                                string str53B = "53B";
                                                int x2 = text.IndexOf(cut_at1);

                                                string string_before = text.Substring(0, x2 - 2);
                                                string string_after = text.Substring(x2 + cut_at1.Length);
                                                int y2 = string_after.IndexOf(cut_at2);
                                                string CustomerEmail = string_after.Substring(0, y2 - 1);

                                                //Find address position 

                                                int addressPosition = text.IndexOf(address) + 19;
                                                int position53B = text.IndexOf(str53B) - 1;

                                                string customerAddress = text.Substring(addressPosition, position53B - addressPosition);

                                                // System.Windows.Forms.MessageBox.Show(customerAddress);
                                                //Replace comma to at 
                                                char old = ',';
                                                char newchar = '@';

                                                CustomerEmail = CustomerEmail.Replace(old, newchar);


                                                SqlCommand cmd = db.CreateCommand();
                                                cmd.CommandText = "SELECT * FROM swift_money_msafir";
                                                SqlDataReader usersEmailsDr = cmd.ExecuteReader();
                                                while (usersEmailsDr.Read())
                                                {
                                                    userEmails = usersEmailsDr.GetString(1);
                                                }
                                                usersEmailsDr.Close();

                                                //Process emails 


                                                string pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";

                                                //Validate email to confirm with the general format
                                                CustomerEmail = CustomerEmail.Replace(" ", "");

                                                if (Regex.IsMatch(CustomerEmail, pattern))
                                                {



                                                    SmtpClient smtpClient = new SmtpClient();
                                                    MailMessage mail = new MailMessage();
                                                    smtpClient.Port = 25;
                                                    smtpClient.Host = "192.168.150.22";
                                                    Attachment attachment = new Attachment(oldfile);

                                                    string subject;
                                                    mail.From = new MailAddress("service.delivery@bankm.com");
                                                    mail.Attachments.Add(attachment);

                                                    //mail.To.Add("innocent.christopher@bankm.com");
                                                    mail.To.Add(CustomerEmail);
                                                    mail.Bcc.Add(userEmails);

                                                    subject = (customerAddress.Replace("\n", " ").Replace("\n", " ").Replace("\n", " "));

                                                    mail.Subject = subject + " SWIFT COPY";
                                                    mail.IsBodyHtml = true;
                                                    mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Dear Valued Customer,<br><br>Please find attached the SWIFT copy processed/received on your behalf. We thank you for channeling your business through us.<br>It has been pleasure serving you and we look forward to your continued patronage.<br><br>If there's any query regarding this payment, please do not hesitate to contact me directly and I will be pleased to assist you.<br></span></body></html>";
                                                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                                   smtpClient.Send(mail);
                                                    // System.Windows.Forms.MessageBox.Show(mail.Body);
                                                    attachment.Dispose();

                                                    //Creating file logs 
                                                    DateTime dt = new DateTime();
                                                    dt = DateTime.Now;
                                                    string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                                    using (StreamWriter w = File.AppendText(logfilename))
                                                    {
                                                        string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_processOutgoingSwiftNonCustomer: " + coppiedFile;
                                                        w.WriteLine(filelog);
                                                    }

                                                    writteLogs(fi.Name, "Outgoing Swift"); //Write to logs 
                                                }
                                                else
                                                {
                                                    if (!File.Exists(coppiedFile))
                                                    {
                                                        File.Copy(oldfile, coppiedFile);
                                                    }

                                                    SmtpClient smtpClient = new SmtpClient();
                                                    MailMessage mail = new MailMessage();
                                                    smtpClient.Port = 25;
                                                    smtpClient.Host = "192.168.150.22";
                                                    Attachment attachment = new Attachment(oldfile);


                                                    mail.From = new MailAddress("service.delivery@bankm.com");
                                                    mail.Attachments.Add(attachment);
                                                    //mail.To.Add("innocent.christopher@bankm.com");
                                                    mail.To.Add("service.delivery@bankm.com");
                                                    mail.Bcc.Add("support@bankm.com");
                                                    mail.To.Add(userEmails);


                                                    mail.Subject = "WRONG MAIL ID DETECTED '" + CustomerEmail + "'";
                                                    mail.IsBodyHtml = true;
                                                    mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said email address is not in a valid/allowed mails address format<br><br>Please also find its attachment <br></span></body></html>";
                                                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                                   smtpClient.Send(mail);
                                                    //System.Windows.Forms.MessageBox.Show(mail.Body);
                                                    attachment.Dispose();

                                                    //Creating file logs 
                                                    DateTime dt = new DateTime();
                                                    dt = DateTime.Now;
                                                    string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                                    using (StreamWriter w = File.AppendText(logfilename))
                                                    {
                                                        string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_processOutgoingSwiftNonCustomer_Failure: " + coppiedFile;
                                                        w.WriteLine(filelog);
                                                    }

                                                    writteLogs(fi.Name, "Outgoing Swift Failure"); //Write to logs 
                                                }

                                            }
                                            else if (text.Contains("/MAIL/"))
                                            {

                                                string cut_at1 = "/MAIL/";
                                                string cut_at2 = "--";
                                                string address = "Address";
                                                string str53B = "53B";
                                                int x2 = text.IndexOf(cut_at1);

                                                string string_before = text.Substring(0, x2 - 2);
                                                string string_after = text.Substring(x2 + cut_at1.Length);
                                                int y2 = string_after.IndexOf(cut_at2);
                                                string CustomerEmail = string_after.Substring(0, y2 - 1);

                                                //Find address position 

                                                int addressPosition = text.IndexOf(address) + 19;
                                                int position53B = text.IndexOf(str53B) - 1;

                                                string customerAddress = text.Substring(addressPosition, position53B - addressPosition);

                                                // System.Windows.Forms.MessageBox.Show(customerAddress);
                                                //Replace comma to at 
                                                char old = ',';
                                                char newchar = '@';

                                                CustomerEmail = CustomerEmail.Replace(old, newchar);


                                                SqlCommand cmd = db.CreateCommand();
                                                cmd.CommandText = "SELECT * FROM swift_money_msafir";
                                                SqlDataReader usersEmailsDr = cmd.ExecuteReader();
                                                while (usersEmailsDr.Read())
                                                {
                                                    userEmails = usersEmailsDr.GetString(1);
                                                }
                                                usersEmailsDr.Close();

                                                //Process emails 


                                                string pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";

                                                //Validate email to confirm with the general format
                                                CustomerEmail = CustomerEmail.Replace(" ", "");

                                                if (Regex.IsMatch(CustomerEmail, pattern))
                                                {



                                                    SmtpClient smtpClient = new SmtpClient();
                                                    MailMessage mail = new MailMessage();
                                                    smtpClient.Port = 25;
                                                    smtpClient.Host = "192.168.150.22";
                                                    Attachment attachment = new Attachment(oldfile);

                                                    string subject;
                                                    mail.From = new MailAddress("service.delivery@bankm.com");
                                                    mail.Attachments.Add(attachment);

                                                   // mail.To.Add("innocent.christopher@bankm.com");
                                                    mail.To.Add(CustomerEmail);
                                                    mail.Bcc.Add(userEmails);

                                                    subject = (customerAddress.Replace("\n", " ").Replace("\n", " ").Replace("\n", " "));

                                                    mail.Subject = subject + " SWIFT COPY";
                                                    mail.IsBodyHtml = true;
                                                    mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Dear Valued Customer,<br><br>Please find attached the SWIFT copy processed/received on your behalf. We thank you for channeling your business through us.<br>It has been pleasure serving you and we look forward to your continued patronage.<br><br>If there's any query regarding this payment, please do not hesitate to contact me directly and I will be pleased to assist you.<br></span></body></html>";
                                                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                                    smtpClient.Send(mail);
                                                    // System.Windows.Forms.MessageBox.Show(mail.Body);
                                                    attachment.Dispose();

                                                    //Creating file logs 
                                                    DateTime dt = new DateTime();
                                                    dt = DateTime.Now;
                                                    string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                                    using (StreamWriter w = File.AppendText(logfilename))
                                                    {
                                                        string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_processOutgoingSwiftNonCustomer: " + coppiedFile;
                                                        w.WriteLine(filelog);
                                                    }

                                                    writteLogs(fi.Name, "Outgoing Swift"); //Write to logs 

                                                }
                                                else
                                                {
                                                    if (!File.Exists(coppiedFile))
                                                    {
                                                        File.Copy(oldfile, coppiedFile);
                                                    }
                                                    

                                                    SmtpClient smtpClient = new SmtpClient();
                                                    MailMessage mail = new MailMessage();
                                                    smtpClient.Port = 25;
                                                    smtpClient.Host = "192.168.150.22";
                                                    Attachment attachment = new Attachment(oldfile);


                                                    mail.From = new MailAddress("service.delivery@bankm.com");
                                                    mail.Attachments.Add(attachment);
                                                   // mail.To.Add("innocent.christopher@bankm.com");
                                                    mail.To.Add("support@bankm.com");
                                                    mail.Bcc.Add(userEmails);


                                                    mail.Subject = "WRONG MAIL ID DETECTED '" + CustomerEmail + "'";
                                                    mail.IsBodyHtml = true;
                                                    mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said email address is not in a valid/allowed mails address format<br><br>Please also find its attachment <br></span></body></html>";
                                                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                                    smtpClient.Send(mail);
                                                    //System.Windows.Forms.MessageBox.Show(mail.Body);
                                                    attachment.Dispose();

                                                    //Creating file logs 
                                                    DateTime dt = new DateTime();
                                                    dt = DateTime.Now;
                                                    string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                                    using (StreamWriter w = File.AppendText(logfilename))
                                                    {
                                                        string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_processOutgoingSwiftNonCustomer_Failure: " + coppiedFile;
                                                        w.WriteLine(filelog);
                                                    }

                                                    writteLogs(fi.Name, "Outgoing Swift Failure"); //Write to logs 
                                                }

                                            }
                                            else
                                            {
                                                    SmtpClient smtpClient = new SmtpClient();
                                                    MailMessage mail = new MailMessage();
                                                    smtpClient.Port = 25;
                                                    smtpClient.Host = "192.168.150.22";
                                                    Attachment attachment = new Attachment(oldfile);


                                                    mail.From = new MailAddress("service.delivery@bankm.com");
                                                    mail.Attachments.Add(attachment);
                                                    //mail.To.Add("innocent.christopher@bankm.com");
                                                    mail.To.Add("support@bankm.com");
                                                    mail.Bcc.Add(userEmails);


                                                    mail.Subject = "WRONG FIELD FORMAT DETECTED";
                                                    mail.IsBodyHtml = true;
                                                    mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the file inputed is not in a valid/allowed mails address format<br><br>Please also find its attachment <br></span></body></html>";
                                                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                                   smtpClient.Send(mail);
                                                    //System.Windows.Forms.MessageBox.Show(mail.Body);
                                                    attachment.Dispose();

                                                    //Creating file logs 
                                                    DateTime dt = new DateTime();
                                                    dt = DateTime.Now;
                                                    string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                                    using (StreamWriter w = File.AppendText(logfilename))
                                                    {
                                                        string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_processOutgoingSwiftNonCustomer_Failure: " + coppiedFile;
                                                        w.WriteLine(filelog);
                                                    }
                                                    writteLogs(fi.Name, "Outgoing Swift Failure"); //Write to logs 
                                            }
                                        }
                                    }
                                    else if (text.Contains("/MAIL/"))
                                    {
                                        if (text.Contains("53A:"))
                                        {
                                            string mailPosition = "/MAIL/";
                                            string position53A = "53A:";
                                            string address = "Address";

                                            if (!File.Exists(coppiedFile))
                                            {
                                                File.Copy(oldfile, coppiedFile);
                                            }
                                            //Find customer email and Address

                                            int mailPos = text.IndexOf(mailPosition) + mailPosition.Length;
                                            int pos53A = text.IndexOf(position53A) - 1;
                                            int AddressPosition = text.IndexOf(address) + 19;

                                            string CustomerEmail = text.Substring(mailPos, pos53A - mailPos).Replace(",", "@");
                                            string CustomerAddress = text.Substring(AddressPosition, mailPos - AddressPosition).Replace(mailPosition, "");

                                            // System.Windows.Forms.MessageBox.Show(CustomerEmail);
                                            // System.Windows.Forms.MessageBox.Show(CustomerAddress);

                                            SqlCommand cmd = db.CreateCommand();
                                            cmd.CommandText = "SELECT * FROM swift_money_msafir";
                                            SqlDataReader usersEmailsDr = cmd.ExecuteReader();
                                            while (usersEmailsDr.Read())
                                            {
                                                userEmails = usersEmailsDr.GetString(1);
                                            }
                                            usersEmailsDr.Close();

                                            //Process emails 
                                            char old = ',';
                                            char newchar = '@';

                                            CustomerEmail = CustomerEmail.Replace(old, newchar);

                                            string pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";

                                            //Validate email to confirm with the general format
                                            CustomerEmail = CustomerEmail.Replace(" ", "");

                                            if (Regex.IsMatch(CustomerEmail, pattern))
                                            {
                                                SmtpClient smtpClient = new SmtpClient();
                                                MailMessage mail = new MailMessage();
                                                smtpClient.Port = 25;
                                                smtpClient.Host = "192.168.150.22";
                                                Attachment attachment = new Attachment(oldfile);

                                                string subject;
                                                mail.From = new MailAddress("service.delivery@bankm.com");
                                                mail.Attachments.Add(attachment);

                                                //mail.To.Add("innocent.christopher@bankm.com");
                                                mail.To.Add(CustomerEmail);
                                                mail.Bcc.Add(userEmails);

                                                subject = (CustomerAddress.Replace("\n", " ").Replace("\n", " ").Replace("\n", " "));

                                                mail.Subject = subject + " SWIFT COPY";
                                                mail.IsBodyHtml = true;
                                                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Dear Valued Customer,<br><br>Please find attached the SWIFT copy processed/received on your behalf. We thank you for channeling your business through us.<br>It has been pleasure serving you and we look forward to your continued patronage.<br><br>If there's any query regarding this payment, please do not hesitate to contact me directly and I will be pleased to assist you.<br></span></body></html>";
                                                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                               smtpClient.Send(mail);
                                                // System.Windows.Forms.MessageBox.Show(mail.Body);
                                                attachment.Dispose();

                                                //Creating file logs 
                                                DateTime dt = new DateTime();
                                                dt = DateTime.Now;
                                                string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                                using (StreamWriter w = File.AppendText(logfilename))
                                                {
                                                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_processOutgoingSwiftNonCustomer: " + coppiedFile;
                                                    w.WriteLine(filelog);
                                                }
                                                writteLogs(fi.Name, "Outgoing Swift"); //Write to logs 
                                                
                                            }
                                            else
                                            {
                                                if (!File.Exists(coppiedFile))
                                                {
                                                    File.Copy(oldfile, coppiedFile);

                                                    //System.Windows.Forms.MessageBox.Show("COPIED");
                                                }
                                                //System.Windows.Forms.MessageBox.Show("Not COPIED");
                                                
                                                SmtpClient smtpClient = new SmtpClient();
                                                MailMessage mail = new MailMessage();
                                                smtpClient.Port = 25;
                                                smtpClient.Host = "192.168.150.22";
                                                Attachment attachment = new Attachment(oldfile);


                                                mail.From = new MailAddress("service.delivery@bankm.com");
                                                mail.Attachments.Add(attachment);

                                                mail.To.Add("support@bankm.com");
                                                mail.Bcc.Add("service.delivery@bankm.com");
                                                mail.Bcc.Add(userEmails);
                                                //mail.To.Add("innocent.christopher@bankm.com");

                                                mail.Subject = "WRONG MAIL ID DETECTED '" + CustomerEmail + "'";
                                                mail.IsBodyHtml = true;
                                                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said email address is not in a valid/allowed mails address format<br><br>Please also find its attachment <br></span></body></html>";
                                                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                                smtpClient.Send(mail);
                                                // System.Windows.Forms.MessageBox.Show(mail.Body);
                                                attachment.Dispose();


                                                //Creating file logs 
                                                DateTime dt = new DateTime();
                                                dt = DateTime.Now;
                                                string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                                using (StreamWriter w = File.AppendText(logfilename))
                                                {
                                                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_processOutgoingSwiftNonCustomer: " + coppiedFile;
                                                    w.WriteLine(filelog);
                                                }

                                                writteLogs(fi.Name, "Outgoing Swift Failure"); //Write to logs 

                                            }
                                        }
                                        else if (text.Contains("53B:"))
                                        {
                                            string mailPosition = "/MAIL/";
                                            string position53B = "53B:";
                                            string address = "Address";

                                            //Find customer email and Address 
                                            if (!File.Exists(coppiedFile))
                                            {
                                                File.Copy(oldfile, coppiedFile);
                                            }
                                            //get postion of mail 
                                            int mailpos = text.IndexOf(mailPosition) + mailPosition.Length;
                                            //Since mail is between mailposition and character 53B then find the postision of 53B

                                            int pos53B = text.IndexOf(position53B);
                                            //To retrieve customer email we need to obtain the substring of the file between mail position and 53B postion 
                                            //Of a lengght of 53B position - last character of mail postion 
                                            //The email from file contains comma insteady of @ we then relace comma with @ 
                                            int substringLenght = pos53B - (mailpos - mailPosition.Length);
                                            string customerEmail = text.Substring(mailpos, substringLenght).Replace(",", "@").Replace(position53B, "").Replace("\n", "");

                                            //Get customer Address
                                            //The customer address is between the Ordering Customer-Name & Address string and 
                                            //MAIL/ character to retrieve the address first find the position of Address keyword 
                                            //Beneth address there is a new line and / plus ten number for the account then the actual start of address is plus these lenth
                                            int addressPos = text.IndexOf(address) + 19;

                                            //Since the address is between the Address position and /MAIL/ then retrieve the substring between these two postions 

                                            int addressSubstringLengh = mailpos - addressPos;

                                            string CustomerAddress = text.Substring(addressPos, addressSubstringLengh).Replace(mailPosition, "").Replace("\n", "");

                                            //System.Windows.Forms.MessageBox.Show(customerEmail);
                                            //System.Windows.Forms.MessageBox.Show(CustomerAddress);

                                            SqlCommand cmd = db.CreateCommand();
                                            cmd.CommandText = "SELECT * FROM swift_money_msafir";
                                            SqlDataReader usersEmailsDr = cmd.ExecuteReader();
                                            while (usersEmailsDr.Read())
                                            {
                                                userEmails = usersEmailsDr.GetString(1);
                                            }
                                            usersEmailsDr.Close();

                                            //Process emails 
                                            char old = ',';
                                            char newchar = '@';

                                            customerEmail = customerEmail.Replace(old, newchar);

                                            string pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";

                                            //Validate email to confirm with the general format
                                            customerEmail = customerEmail.Replace(" ", "");

                                            if (Regex.IsMatch(customerEmail, pattern))
                                            {
                                                SmtpClient smtpClient = new SmtpClient();
                                                MailMessage mail = new MailMessage();
                                                smtpClient.Port = 25;
                                                smtpClient.Host = "192.168.150.22";
                                                Attachment attachment = new Attachment(oldfile);

                                                string subject;
                                                mail.From = new MailAddress("service.delivery@bankm.com");
                                                mail.Attachments.Add(attachment);


                                                mail.To.Add(customerEmail);
                                                mail.Bcc.Add(userEmails);
                                                //mail.To.Add("innocent.christopher@bankm.com");
                                                subject = (CustomerAddress.Replace("\n", " ").Replace("\n", " ").Replace("\n", " "));

                                                mail.Subject = subject + " SWIFT COPY";
                                                mail.IsBodyHtml = true;
                                                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Dear Valued Customer,<br><br>Please find attached the SWIFT copy processed/received on your behalf. We thank you for channeling your business through us.<br>It has been pleasure serving you and we look forward to your continued patronage.<br><br>If there's any query regarding this payment, please do not hesitate to contact me directly and I will be pleased to assist you.<br></span></body></html>";
                                                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                                smtpClient.Send(mail);
                                                //System.Windows.Forms.MessageBox.Show(mail.Body);
                                                attachment.Dispose();


                                                //Creating file logs 
                                                DateTime dt = new DateTime();
                                                dt = DateTime.Now;
                                                string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                                using (StreamWriter w = File.AppendText(logfilename))
                                                {
                                                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_processOutgoingSwiftNonCustomer: " + coppiedFile;
                                                    w.WriteLine(filelog);
                                                }
                                                writteLogs(fi.Name, "Outgoing Swift"); //Write to logs 
                                               
                                            }
                                            else
                                            {
                                                if (!File.Exists(coppiedFile))
                                                {
                                                    File.Copy(oldfile, coppiedFile);
                                                }
                                               

                                                SmtpClient smtpClient = new SmtpClient();
                                                MailMessage mail = new MailMessage();
                                                smtpClient.Port = 25;
                                                smtpClient.Host = "192.168.150.22";
                                                Attachment attachment = new Attachment(oldfile);


                                                mail.From = new MailAddress("service.delivery@bankm.com");
                                                mail.Attachments.Add(attachment);

                                                mail.To.Add("support@bankm.com");
                                                mail.Bcc.Add(userEmails);
                                                //mail.To.Add("innocent.christopher@bankm.com");

                                                mail.Subject = "WRONG MAIL ID DETECTED '" + customerEmail + "'";
                                                mail.IsBodyHtml = true;
                                                mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said email address is not in a valid/allowed mails address format<br><br>Please also find its attachment <br></span></body></html>";
                                                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                               smtpClient.Send(mail);
                                                // System.Windows.Forms.MessageBox.Show(mail.Body);
                                                attachment.Dispose();


                                                //Creating file logs 
                                                DateTime dt = new DateTime();
                                                dt = DateTime.Now;
                                                string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                                using (StreamWriter w = File.AppendText(logfilename))
                                                {
                                                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_processOutgoingSwiftNonCustomer: " + coppiedFile;
                                                    w.WriteLine(filelog);
                                                }

                                                writteLogs(fi.Name, "Outgoing Swift Failure"); //Write to logs 
                                            }

                                        }
                                    }
                                    else
                                    {
                                         //Check for account first 
                                       //System.Windows.Forms.MessageBox.Show("File found: = " + oldfile);
                                       PdfFocus p2 = new PdfFocus();
                                       Byte[] pdf2 = File.ReadAllBytes(oldfile);
                                        List resul2t = new List();
                                        string PdfText2 = "";
                                        p2.OpenPdf(pdf2);
                                        if (p2.PageCount > 0)
                                        {
                                                    PdfText2 = p.ToText(1, 1);
                                                   
                                                    if (!File.Exists(coppiedFile))
                                                    {
                                                        File.Copy(oldfile, coppiedFile);
                                                    }
                                                    //find the account number
                                                    string startWith = "50K:";
                                                    int postionFound = PdfText2.IndexOf(startWith);

                                                    string ACNO = "";
                                                    string ommitedstr2 = "Ordering Customer-Name & Address";

                                                    string strAfter = PdfText2.Substring(postionFound + ommitedstr2.Length + startWith.Length + 7);
                                                    ACNO = strAfter.Substring(0, 10);


                                                    //Get customer details based on the account 

                                                    SqlCommand cmd = db.CreateCommand();
                                                    cmd.CommandText = "SELECT * FROM customer WHERE customeraccount ='" + ACNO + "'";
                                                    SqlDataReader dr = cmd.ExecuteReader();
                                                    dr.Read();
                                                    //If customer is not exist send as failed outgoing swift 
                                                    if (!dr.HasRows)
                                                    {
                                           
                                        
                                                        SmtpClient smtpClient = new SmtpClient();
                                                        MailMessage mail = new MailMessage();
                                                        smtpClient.Port = 25;
                                                        smtpClient.Host = "192.168.150.22";
                                                        MailAddress mailAddress = new MailAddress("service.delivery@bankm.com");
                                                        Attachment attachment = new Attachment(oldfile);
                                                        mail.From = new MailAddress("service.delivery@bankm.com");
                                                        mail.Attachments.Add(attachment);
                                                                //Get swift users
                                                        db.Close();
                                                        db.Open();

                                                        SqlCommand cmd2 = db.CreateCommand();
                                                        cmd2.CommandText = "SELECT * FROM swift_Users_failure";

                                                        SqlDataReader dr2 = cmd2.ExecuteReader();

                                                        string swusers = "";

                                                        while (dr2.Read())
                                                        {
                                                            swusers = dr2.GetString(1);

                                                        }
                                                        dr2.Close();


                                                        mail.To.Add(swusers);
                                                        mail.Bcc.Add("service.delivery@bankm.com");
                                                       // mail.To.Add("innocent.christopher@bankm.com");

                                                        mail.Subject = "SWIFT COPY SENDING FAILURE - CUSTOMER ACCOUNT DOES NOT EXIST";
                                                        mail.IsBodyHtml = true;
                                                        mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Dear Valued Customer,<br><br>Please find the attached SWIFT copy processed/receive which has not been sent to the intended customer,<br>due to non maintenance of account number in the database.<br></span></body></html>";
                                                        mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                                                       smtpClient.Send(mail);
                                                        attachment.Dispose();

                                                        DateTime dtfr = new DateTime();
                                                        dtfr = DateTime.Now;

                                                        SqlConnection myconnection = new SqlConnection();
                                                        SqlCommand mycommand = new SqlCommand();
                                                        myconnection = new SqlConnection("Data Source=AUTOMAILSRV;Initial Catalog=outgoingsms;Integrated Security=True");
                                                        myconnection.Open();
                                                        mycommand = new SqlCommand("insert into outgoinglog([customername],[mailsentdate],[mailsenttime],[mailstatus]) values ('NILL','" + dtfr.ToString("dd/MM/yyy") + "','" + dtfr.ToString("hh.mm") + "','NOTSENT')", myconnection);
                                                        mycommand.ExecuteNonQuery();
                                                        myconnection.Close();

                                                        //Creating file logs 
                                                        DateTime dt = new DateTime();
                                                        dt = DateTime.Now;
                                                        string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                                        using (StreamWriter w = File.AppendText(logfilename))
                                                        {
                                                            string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Processed_Noncustomer_Account_Failed_processOutgoingSwift: " + oldfile;
                                                            w.WriteLine(filelog);
                                                        }
                                                        writteLogs(fi.Name, "Outgoing Swift Failure"); //Write to logs 
                                                        
                                                            }
                                                        }
                                    }


                                }
                            }

                        }
                        

                    }
                  }
              
               catch(Exception ex)
                   {
                       SmtpClient smtpClient = new SmtpClient();
                       MailMessage mail = new MailMessage();
                       smtpClient.Port = 25;
                       smtpClient.Host = "192.168.150.22";
                       Attachment attachment = new Attachment(oldfile);


                       mail.From = new MailAddress("service.delivery@bankm.com");
                       mail.Attachments.Add(attachment);
                       mail.To.Add("innocent.christopher@bankm.com");
                       mail.To.Add("adolph.mwakalinga@bankm.com");

                       mail.Subject = "ERROR NOTIFICATION IN processOutgoingSwiftNonCustomer";
                       mail.IsBodyHtml = true;
                       mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Team,<br><br>Please refer to the subject above, the said document failed to be processed with error message [" + ex.Message + "]<br></span></body></html>";
                       mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                       smtpClient.Send(mail);
                       //System.Windows.Forms.MessageBox.Show(mail.Body);
                       attachment.Dispose();
    
                   //Creating file logs 
                       DateTime dt = new DateTime();
                       dt = DateTime.Now;
                       string logfilename = "SWIFTLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                       using (StreamWriter w = File.AppendText(logfilename))
                       {
                           string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_processOutgoingSwiftNonCustomer: " + ex.Message + " occured for " + error_in_file;
                           w.WriteLine(filelog);
                       }
                   }
                  
              
               
            }
        #endregion
    }

        


}
   


