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
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace commonServiceMonitoring
{
    class FCDB
    {
        //Get list of files in the directory 
        #region FCDB 
        //Get list of files in the directory 

        public void ProcessDirectory(string targetDirectory, string destination, OracleConnection db)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(targetDirectory);
                //Get current date 
                DateTime dtm = new DateTime();
                dtm = DateTime.Now;

                FileInfo[] fileNames = di.GetFiles("*.*");


                //Start processing 

                foreach (System.IO.FileInfo fi in fileNames)
                {
                    //Console.WriteLine("{0}: {1}: {2}", fi.Name, fi.LastAccessTime, fi.Length);

                    //System.Windows.Forms.MessageBox.Show(fi.Name);
                    string[] filearry = fi.Name.ToString().Split('_');
                    string referenceno = filearry[0];
                    string userid = filearry[1];

                    //Get today files //Unprocessed files 
                    string cYear = referenceno.Substring(0, 4);
                    string cMonth = referenceno.Substring(4, 2);
                    string cDay = referenceno.Substring(6, 2);

                    string coppiedFile = destination + fi.Name;
                    string oldfile = targetDirectory + fi.Name;



                    //Filter file by date
                    //Process files only send today.

                    if (cYear.Equals(dtm.Year.ToString()) && cMonth.Equals(dtm.ToString("MM")) && cDay.Equals(dtm.ToString("dd")))
                    {


                        if (!File.Exists(coppiedFile))
                        {

                            //Process Copy file 
                            File.Copy(oldfile, coppiedFile);


                            //Fetch details from the database  and send email 


                            OracleCommand cmd = db.CreateCommand();
                            OracleCommand cmd2 = db.CreateCommand();

                            cmd.CommandText = "SELECT DISTINCT(TXNUPLOAD.FILENAME), VALUESTRING,TXTCORPDESC,NAMUSER,MSTFILEUPLOADDET.FCATREFNO,TXTCORPDESC FROM MSTCORPUSER  INNER JOIN  MSTCORPORATE USING (idcorporate) INNER JOIN MSTFILEUPLOADDET USING (IDUSER) INNER JOIN  TXNUPLOAD ON(MSTFILEUPLOADDET.FCATREFNO=TXNUPLOAD.FCATREFNO) INNER JOIN APPLDATA ON (MSTFILEUPLOADDET.IDAPP=APPLDATA.IDAPP) WHERE IDUSER='" + userid + "' AND MSTFILEUPLOADDET.IDTXN = APPLDATA.DATAVALUE AND  MSTFILEUPLOADDET.FCATREFNO='" + referenceno + "'";

                            cmd2.CommandText = "SELECT A.UDF1, A.UDF2 FROM MSTFILEUPLOADDET A, APPLDATA B WHERE A.IDTXN =B.DATAVALUE AND A.FCATREFNO='" + referenceno + "'";

                            OracleDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {

                                //Process email 
                                SmtpClient smtpClient = new SmtpClient();
                                MailMessage mail = new MailMessage();
                                smtpClient.Port = 25;
                                smtpClient.Host = "192.168.150.22";

                                MailAddress mailAddress = new MailAddress("IB.Attachment@bankm.com", "Internet Banking Uploaded Attachment");

                                mail.From = mailAddress;
                                 mail.To.Add("FCDB@bankm.com");
                                // BCC the support service 
                                 mail.Bcc.Add("support@bankm.com");
                               // mail.To.Add("innocent.christopher@bankm.com");
                                //Mail strings 

                                string subjects = "";
                                string bodyTable1 = "";
                                string bodyTable2 = "";
                                string TXTCORPDESC = "";
                                string VALUESTRING = "";
                                string NAMUSER = "";

                                while (dr.Read())
                                {
                                    subjects = "FCDB Uploaded " + dr["VALUESTRING"].ToString() + " details REF:" + dr["FCATREFNO"].ToString() + "";

                                    ///For table 2 
                                    bodyTable2 += "<tr>";
                                    bodyTable2 += "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + dr["FILENAME"].ToString() + "</span></td>";
                                    bodyTable2 += "</tr>";

                                    TXTCORPDESC = dr["TXTCORPDESC"].ToString();
                                    VALUESTRING = dr["VALUESTRING"].ToString();
                                    NAMUSER = dr["NAMUSER"].ToString();

                                }

                                //Prepare HTML email  formating for first table 

                                OracleDataReader dr2 = cmd2.ExecuteReader();
                                while (dr2.Read())
                                {
                                    bodyTable1 += "<tr>";
                                    bodyTable1 += "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + dr2["UDF1"].ToString() + "</span></td>";
                                    bodyTable1 += "<td><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>" + dr2["UDF2"].ToString() + "</span></td>";
                                    bodyTable1 += "</tr>";
                                }



                                //Close connectiond and readers
                                dr.Close();
                                dr2.Close();


                                mail.Subject = subjects;
                                mail.IsBodyHtml = true;
                                mail.Body = mail.Body + "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>Team,<br><br> Please find the details below of  " + VALUESTRING + "  uploaded on Internet Banking Platform <br>by User :" + NAMUSER + ", Company :" + TXTCORPDESC + " <br><br></span>";

                                //Start of Table 1
                                mail.Body = mail.Body + "<table width='50%' border='1' cellpadding='0' cellspacing='0' bgcolor='white'>";
                                mail.Body = mail.Body + "<tr>";
                                mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>Template details</span></strong></td>";
                                mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>Details</span></strong></td>";
                                mail.Body = mail.Body + "</tr>";

                                mail.Body = mail.Body + bodyTable1; //Include section for table 1 

                                mail.Body = mail.Body + "</table>";

                                //------------------------------end table 1

                                mail.Body = mail.Body + "<br>&nbsp;</br>";

                                //------------------------------table 2
                                mail.Body = mail.Body + "<table width='50%' border='1' cellpadding='0' cellspacing='0' bgcolor='white'>";
                                mail.Body = mail.Body + "<tr>";
                                mail.Body = mail.Body + "<td><strong><span style='color:#4f81bd;font-size: 16px;font-family: tahoma'>File names attached</span></strong></td>";
                                mail.Body = mail.Body + "</tr>";

                                mail.Body = mail.Body + bodyTable2;

                                mail.Body = mail.Body + "</table>";
                                //------------------------------end table 2


                                mail.Body = mail.Body + "</body></html>";
                                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;


                                smtpClient.Send(mail);// send email now 



                                //Process printing the file

                                using (Process P = new Process())
                                {
                                    P.StartInfo.FileName = coppiedFile;
                                    P.StartInfo.Verb = "Print";
                                    P.Start();

                                    //P.WaitForExit()
                                }

                                /*
                                 ProcessStartInfo info = new ProcessStartInfo(coppiedFile);
                                 info.Verb = "Print";
                                 info.CreateNoWindow = true;
                                 info.WindowStyle = ProcessWindowStyle.Hidden;
                                 Process.Start(info);
                              
                                //end printing file 
                                *  */

                                //Creating file logs 
                                DateTime dt = new DateTime();
                                dt = DateTime.Now;
                                string logfilename = "FCDBLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                                using (StreamWriter w = File.AppendText(logfilename))
                                {
                                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_")+"Successfully_processed" + coppiedFile;
                                    w.WriteLine(filelog);
                                }
                                 
                            }


                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        //System.Windows.Forms.MessageBox.Show("Not Matched" + coppiedFile);
                    }
                }

                //Creating file logs 

               
            }
            catch (Exception ex)
            {
                //Creating file logs 
                if (ex.ToString().Equals("Index was outside the bounds of the array"))
                {
                }
                else
                {

                    DateTime dt = new DateTime();
                    dt = DateTime.Now;
                    string logfilename = "FCDBErrorLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                    using (StreamWriter w = File.AppendText(logfilename))
                    {
                        string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_ProcessDirectory: " + ex.Message;
                        w.WriteLine(filelog);
                    }
                }
                 

            }
        }
           

        // Insert logic for processing found files here. 
        public static void ProcessFile(string path)
        {

        }

#endregion
        #region FCBD CMF 

        //FCDB CMF 
        public void processFCDBCMF(OracleConnection oDb, MySqlConnection mDb)
        {
            try
            {

            //
            OracleCommand cmd = oDb.CreateCommand();
            cmd.CommandText = "SELECT C.IDMESG,C.DATMESSAGE,C.FROMID,B.NAMUSER,A.TXTCORPDESC,C.TXTSUBJECT,C.TXTMESSAGE FROM MSTCORPORATE A, MSTCORPUSER B, MAILMESSAGE C WHERE C.FROMID = B.IDUSER AND B.IDCORPORATE = A.IDCORPORATE";
            OracleDataReader dr = cmd.ExecuteReader();
            string msge = "";
            SqlConnection conDatabase  = new SqlConnection("Data Source=AUTOMAILSRV\\SQLEXPRESS;Initial Catalog=mailmessage;Integrated Security=True");
            conDatabase.Open(); //open connections for sql server

            string Label1 = "";

            while (dr.Read())
            {
               
             SqlCommand   cmdDatabase =conDatabase.CreateCommand();
             cmdDatabase.CommandText = "select * from STTM_IDMESG WHERE IDMESG = '" + dr["IDMESG"].ToString() +  "'";
             SqlDataReader customerdetails = cmdDatabase.ExecuteReader();

             if (customerdetails.HasRows)
             {
                   

                    string  cut_at1 = "<faml><message>";
                    string cut_at2 = "</message><ccuserid>";
                    int x  = msge.IndexOf(cut_at1);
                    string string_before  = msge.Substring(0, x - 1);
                    string string_after = msge.Substring(x + cut_at1.Length - 1);

                    int y =string_after.IndexOf( cut_at2);
                    string  string_before1  = string_after.Substring(0, y - 1);
                    string  string_after1  = string_after.Substring(y + cut_at2.Length - 1);

                    char old ,newchar;
                  
                    string  msg_subject = dr["TXTSUBJECT"].ToString();

                    old = '(';
                    newchar = ' ';

                    string  new_string_before1 = string_before1.Replace(old, newchar);
                    string  new_msg_subject  = msg_subject.Replace(old, newchar);


                    old = ')';
                    newchar = ' ';

                    new_string_before1 = string_before1.Replace(old, newchar);
                    new_msg_subject = msg_subject.Replace(old, newchar);


                    old = '\'';
                    newchar = ' ';

                    new_string_before1 = new_string_before1.Replace(old, newchar);
                    new_msg_subject = new_msg_subject.Replace(old, newchar);


                    old = ',';
                    newchar = ' ';

                    new_string_before1 = new_string_before1.Replace(old, newchar);
                    new_msg_subject = new_msg_subject.Replace(old, newchar);

                    SmtpClient SmtpServer = new  SmtpClient();
                    MailMessage mail = new MailMessage();
                    SmtpServer.Port = 25;
                    SmtpServer.Host = "192.168.150.22";

                    mail.From = new MailAddress("internet.banking@bankm.com");
                    mail.To.Add("innocent.christopher@bankm.com");
                   
                    mail.Subject = " IB CUSTOMER REQUESTS ID:" + dr["IDMESG"].ToString() + "";
                    mail.IsBodyHtml = true;
                    mail.Body = "<html><head><style type=text/css>.style1 {color: #4f81bd;font-size: 16px;font-family: tahoma;}.style2 {color: #ff0000;font-weight: bold;}.style3 {color: #4f81bd;font-size: 10px;font-family: tahoma;}</style></head><body><span class=style1>Dear Team,<br><br> Below is the checklist received from customer on internet banking portal <br><br><table width=500 border=1 cellspacing=0 cellpadding=0><tr><td>CUSTOMER APPLICATION </td></tr>  <tr>    <td><table width=500 border=1 cellspacing=0 cellpadding=0> <tr> <td>MESSAGE ID:</td> <td>" + dr["IDMESG"].ToString() + "</td></tr><tr> <td>DATE:</td> <td>" + dr["DATMESSAGE"].ToString()+ "</td></tr> <tr> <td>FROM ID:</td> <td>" + dr["FROMID"].ToString()+ "</td></tr><tr>  <td>NAME:</td>  <td>" + dr["NAMUSER"].ToString() + "</td> </tr><tr>  <td>COMPANY:</td>  <td>" + dr["TXTCORPDESC"].ToString() + "</td> </tr> <tr> <td>SUBJECT</td> <td>" + new_msg_subject + "</td> </tr> <tr> <td>MESSAGE</td> <td>" + new_string_before1 + "</td></tr> </table></td>  </tr></table><br><br></span></body></html>";

                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    SmtpServer.Send(mail);

                    Label1 = "LAST MESSAGE ID: " + dr["IDMESG"].ToString(); ;

                   string MSGID = dr["IDMESG"].ToString() ;
                   string DT_MSG  = String.Format("yyyy-MM-dd",dr["DATMESSAGE"].ToString() );
                   string TM_MSG = String.Format("HH:mm:ss",dr["DATMESSAGE"].ToString());

                   MySqlCommand myCommand4 = mDb.CreateCommand();
                   MySqlDataAdapter myAdapter = new MySqlDataAdapter();
                   MySqlDataReader myData;

                   string SQL = "insert into cmf_details (IDMESG, FROMID, TXTSUBJECT, TMMESSAGE, DTMESSAGE, TXTMESSAGE, TOUSERLIST, COMPANY) values ('" + MSGID + "','" + dr["FROMID"].ToString() + "','" + new_msg_subject + "','" + TM_MSG + "','" + DT_MSG + "','" + new_string_before1 + "','" + dr["NAMUSER"].ToString() + "','" + dr["TXTCORPDESC"].ToString() + "')";
                   myAdapter.SelectCommand = myCommand4;
                   try
                   {
                        myData = myCommand4.ExecuteReader();
                        myData.Read();
                        if (myData.HasRows)
                        {
                            myData.Close();
                        }
                        else
                        {
                            myData.Close();
                        }
                   }
                   catch (Exception ex)
                   {

                       //Creating file logs 
                       using (StreamWriter w = File.AppendText("Logs.fcdb"))
                       {
                           DateTime dt = new DateTime();
                           dt = DateTime.Now;
                           string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_processFCDBCMF: " + ex.Message;
                           w.WriteLine(filelog);
                       }
                   }
             }
            
            } //End while 

            //Close connections 
            dr.Close();
            conDatabase.Close();
            }
            catch (Exception ex)
            {

                //Creating file logs 
                DateTime dt = new DateTime();
                dt = DateTime.Now;
                string logfilename = "FCDBLogs" + dt.Year + dt.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dt.Year + dt.ToString("-MM-dd hh:mm:ss_") + "Error_FCDB CMF General: " + ex.Message;
                    w.WriteLine(filelog);
                } 
            }
        }
        #endregion


    }

}
