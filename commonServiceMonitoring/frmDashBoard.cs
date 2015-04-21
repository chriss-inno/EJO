using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.OracleClient;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
namespace commonServiceMonitoring
{
    public partial class frmDashBoard : Form
    {
        //General initialization 
        
        DBGeneral dbg = new DBGeneral();
        OracleConnection db;
        
        
        //End  General initialization
        //xxxxxxxxxxxxxxxxxxxx................xxxxxxxxxxxxxxxxxxx............


        //FCBD Initialization 
        System.Timers.Timer timerFCDB = new System.Timers.Timer();
        string sourceFolder;      //The source folder for FCDB Upload
        string destinationFolder; // The destination folder for FCDB 

        System.Timers.Timer timerFCDB_CMF  = new System.Timers.Timer();
        MySqlConnection mDb;
        //xxxxxxxxxxxxxxxxxxxxx..............xxxxxxxxxxxx..............xxxxxxxxxx


        //Starting swift incomming  initialization 
        SqlConnection dbSwift; //MSQSL SERVER CONNECTION FOR SWIFT 
        System.Timers.Timer timerSWIFTINCO = new System.Timers.Timer();  //TIMER FOR SWIFT INCOMMING 

        
       string dFolderSwift = "C:\\Swift Receipts\\OUTPUT\\2012\\103\\";
       string sFoldeSwiftr = "\\\\192.168.1.127\\c$\\KENEX\\BANK\\DESTINATION\\PRT\\OUTPUT\\103\\";
        

        //Swift outgoing

        //Starting swift  outgoin initialization for customer
 
        SqlConnection dbSwiftOt; //MSQSL SERVER CONNECTION FOR SWIFT 
        System.Timers.Timer timerSWIFTOT = new System.Timers.Timer();  //TIMER FOR SWIFT INNOMMING 

        string sFoldeSwiftrOt="\\\\192.168.1.127\\c$\\KENEX\\BANK\\DESTINATION\\PRT\\INPUT\\103\\";  //The source folder for FCDB Upload
        string dFolderSwiftOt1="C:\\Swift Receipts\\INPUT\\2012\\103\\"; // The destination folder for SWIFT 
        string dFolderSwiftOt2 = "C:\\Swift Receipts\\A\\"; // The destination folder for SWIFT 

       

        //Swift outgoin non customer
        string sFolderSwiftNonCust = "C:\\Swift Receipts\\A\\";
        string dFolderSwiftNonCust = "C:\\Swift Receipts\\B\\";

       

        System.Timers.Timer timerSWIFTOTNONCU = new System.Timers.Timer();  //TIMER FOR SWIFT INNOMMING 
        SqlConnection dbSwiftNonCust;
        //Ending swift initialization

        //Money wireless FAILURE 
        System.Timers.Timer timerMoWFAILURE = new System.Timers.Timer();  //TIMER FOR SWIFT INNOMMING  
        SqlConnection dbMoWFAILURE;
        OracleConnection dbOrMoWFAILURE; 

        //Money wireless success 
        System.Timers.Timer timerMoSUCCESS = new System.Timers.Timer();  //TIMER FOR SWIFT INNOMMING  
        SqlConnection dbMoSUCCESS;
         

        public frmDashBoard()
        {
            InitializeComponent();
        }

        

        private void btnS4start_Click(object sender, EventArgs e)
        {
            try
            {
                //Starting up the FCDB service 
                DateTime dt = new DateTime();
                dt = DateTime.Now;

                sourceFolder = "\\\\192.168.110.192\\d$\\BANKM\\Download\\";
                destinationFolder = "Z:\\FCDB\\" + dt.Year + "\\" + dt.ToString("MMMM") + "\\" + dt.Day + "\\";

                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }
                 //check for source files if they do exist 
                if (!Directory.Exists(sourceFolder))
                {
                    MessageBox.Show("Service will not start source folder is missing or is not accessible,\n please check if the " + sourceFolder + " is Mapped to this machine", "Service Panel says \"Source directory not found\"", MessageBoxButtons.OK, MessageBoxIcon.Error);

                   //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                        string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SService will not start source folder is missing or is not accessible,\n please check if the " + sourceFolder + "is Mapped to this machine";
                        w.WriteLine(filelog);
                    }
                }
                else
                {
                    //Start database session 
                   

                    //Start timer service for FCDB  
                    timerFCDB = new System.Timers.Timer();
                    timerFCDB.Interval = 30000; // 30 seconds
                    timerFCDB.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerFCDB);
                    timerFCDB.Start();


                    //Additional control 
                    btnS4start.Enabled = false;
                    btnS4Restart.Enabled = true;
                    btnS4Stop.Enabled = true;

                    //Command status 

                    lblS4Display.ForeColor = Color.Green;
                    lblS4Display.Text = "Service is Running";

                            //Creating file logs 
                        DateTime dttm = new DateTime();
                        dttm = DateTime.Now;
                        string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                        using (StreamWriter w = File.AppendText(logfilename))
                        {
                                string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Service Started";
                                 w.WriteLine(filelog);
                        }
                }
            }
            catch (Exception ex )
            {

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Error:" + ex.Message;
                    w.WriteLine(filelog);
                }
            }
            

        }

        //TIMER SERVICE FOR FCDB 
        public void OnTimerFCDB(object sender, System.Timers.ElapsedEventArgs args)
        {
            try
            {
                // TODO: Insert monitoring activities here.

                FCDB c = new FCDB();
                //timerFCDB.Enabled = false;
                c.ProcessDirectory(sourceFolder, destinationFolder, db);
                //Create logs file 
                //timerFCDB.Enabled = true;
            }
            catch (Exception ex)
            {

              
            }


        }

        private void btnS4Restart_Click(object sender, EventArgs e)
        {
            try
            {
                //Command status 

                lblS4Display.ForeColor = Color.Blue;
                lblS4Display.Text = "Service is Restated";


                //Starting up the FCDB service 
                DateTime dt = new DateTime();
                dt = DateTime.Now;

               
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                //Start database session 
                db = dbg.Oracle_connect();

                //Stop the timer
                timerFCDB.Enabled = false;
                //Start timer service for FCDB  
                timerFCDB.Interval = 30000; // 30 seconds
                timerFCDB.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerFCDB);
                timerFCDB.Start();


                //Additional control 
                btnS4start.Enabled = false;
                btnS4Restart.Enabled = true;
                btnS4Stop.Enabled = true;

                lblS4Display.ForeColor = Color.Green;
                lblS4Display.Text = "Service is Running";

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Service Restarted";
                    w.WriteLine(filelog);
                }

                //Command status 

                

            }
            catch (Exception ex)
            {

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Error_btnS4Restart_Click:" + ex.Message;
                    w.WriteLine(filelog);
                }
            }
            
        }

        private void btnS4Stop_Click(object sender, EventArgs e)
        {
            try
            {
                timerFCDB.Enabled = false;
                timerFCDB.Close();

                btnS4start.Enabled = true;
                btnS4Stop.Enabled = false;
                btnS4Restart.Enabled = false;

                //Command status 

                lblS4Display.ForeColor = Color.Red;
                lblS4Display.Text = "Service is Stoped";

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Service Stopped";
                    w.WriteLine(filelog);
                }
            }
            catch (Exception ex)
            {
               //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss_") + "Processed_processOutgoingSwiftNonCustomer_Failure: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }

        }
        #region SWIFT Incoming 
        //Process SWIFT 
        private void btnS1start_Click(object sender, EventArgs e)
        {
           

           
            try
            {
                

                //Check for destination folder 
                if (!Directory.Exists(dFolderSwift))
                {
                    Directory.CreateDirectory(dFolderSwift);

                }

                //check for source files if they do exist 
                if (!Directory.Exists(sFoldeSwiftr))
                {
                    MessageBox.Show("Service will not start source folder is missing or is not accessible,\n please check if the " + sFoldeSwiftr + " is Mapped to this machine","Service Panel says \"Source directory not found\"",MessageBoxButtons.OK,MessageBoxIcon.Error);

                   //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                        string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT: Incoming Messages Service will not start source folder is missing or is not accessible";
                        w.WriteLine(filelog);
                    }
                }
                else
                {
                    //Buttons controls 
                    btnS1Restart.Enabled = true;
                    btnS1start.Enabled = false;
                    btnS1Stop.Enabled = true;

                    lblS1Display.ForeColor = Color.Green;
                    lblS1Display.Text = "Service is Running";
                    //Start connection 
                    dbSwift = dbg.MSSQL_SWIFT_connect();

                    //Start timer service for swift 
                    timerSWIFTINCO = new System.Timers.Timer();
                    timerSWIFTINCO.Interval = 30000; // 30 seconds
                    timerSWIFTINCO.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerSWIFTIN);
                    timerSWIFTINCO.Start();

                    //Creating file logs 
                    DateTime dttm = new DateTime();
                    dttm = DateTime.Now;
                    string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                    using (StreamWriter w = File.AppendText(logfilename))
                    {
                        string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT: Incoming Messages Service Started";
                        w.WriteLine(filelog);
                    }
                }
            }
            catch (Exception ex)
            {
               //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Error_btnS1start_Click:" + ex.Message;
                    w.WriteLine(filelog);
                }
            }
            
            
        }
        //TIMER SERVICE FOR FCDB 
        public void OnTimerSWIFTIN(object sender, System.Timers.ElapsedEventArgs args)
        {
           try 
          {
                // TODO: Insert monitoring activities here.

                //SWIFT instance
                SWIFT sw = new SWIFT();
                //timerSWIFTINCO.Enabled = false; //Stop further time service until one operation is completed 
                
               
                sw.processIncomingSwift(sFoldeSwiftr, dFolderSwift, dbSwift);
               //sw.processIncomingSwift("C:\\snocuout\\", "C:\\snocuout2\\", dbSwift);
               
               
               // timerSWIFTINCO.Enabled = true;   //Start again the timer to proces new incomming 
           }
            catch (Exception ex)
          {

            
           }


        }
        

        private void btnS1Restart_Click(object sender, EventArgs e)
        {

            try
            {
                //Buttons controls 
                btnS1Restart.Enabled = true;
                btnS1start.Enabled = false;
                btnS1Stop.Enabled = true;

                lblS1Display.ForeColor = Color.Green;
                lblS1Display.Text = "Service is Running";

                //Close connection
                dbSwift.Close();
                //Start connection 
                dbSwift.Open();

                //Start timer service for swift 

                timerSWIFTINCO.Interval = 30000; // 30 seconds
                timerSWIFTINCO.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerSWIFTIN);
                timerSWIFTINCO.Start();

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT: Incoming Messages Service Restarted";
                    w.WriteLine(filelog);
                }
            }
            catch (Exception ex)
            {

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Error_btnS1Restart_Click"+ ex.Message ;
                    w.WriteLine(filelog);
                }
            }
        }

        private void btnS1Stop_Click(object sender, EventArgs e)
        {
            //Additional control 
            btnS1start.Enabled = true;
            btnS1Restart.Enabled =false;
            btnS1Stop.Enabled = false;

            //Command status 

            lblS1Display.ForeColor = Color.Red;
            lblS1Display.Text = "Service Stoped";

            try
            {
                timerSWIFTINCO.Enabled = false;   //Start again the timer to proces new incomming
                //Close connection
                dbSwift.Close();



                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT: Incoming Messages Service Stopped";
                    w.WriteLine(filelog);
                }
            }
            catch (Exception ex)
            {

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Error_btnS1Stop_Click:" + ex.Message;
                    w.WriteLine(filelog);
                } 
            }
           
        }

        
        #endregion

        #region SWIFT Outgoin

        private void btnS2start_Click(object sender, EventArgs e)
        {
            try
           {
               //
                //Check for destination folder 

               

                if (!Directory.Exists(dFolderSwiftOt1))
                {
                    Directory.CreateDirectory(dFolderSwiftOt1);

                }
                if (!Directory.Exists(dFolderSwiftOt2))
                {
                    Directory.CreateDirectory(dFolderSwiftOt2);

                }

                //check for source files if they do exist 
                if (!Directory.Exists(sFoldeSwiftrOt))
                {
                    MessageBox.Show("Service will not start source folder is missing or is not accessible,\n please check if the " + sFoldeSwiftrOt + " is Mapped to this machine", "Service Panel says \"Source directory not found\"", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //Creating file logs 
                    DateTime dttm = new DateTime();
                    dttm = DateTime.Now;
                    string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                    using (StreamWriter w = File.AppendText(logfilename))
                    {
                        string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Service will not start source folder " + sFoldeSwiftrOt + " is missing or is not accessible";
                        w.WriteLine(filelog);
                    }
                }
                else
                {
                    dbSwiftOt = dbg.MSSQL_SWIFT_Out_connect();
                    //Start timer service for swift 
                    timerSWIFTOT = new System.Timers.Timer();
                    timerSWIFTOT.Interval = 30000; // 30 seconds
                    timerSWIFTOT.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimertimerSWIFTOT);
                    timerSWIFTOT.Start();

                    //Button control 

                    lblS2Display.ForeColor = Color.Green;
                    lblS2Display.Text = "Service is Running";
                    btnS2start.Enabled = false;
                    btnS2Restart.Enabled = true;
                    btnS2Stop.Enabled = true;


                    //Creating file logs 
                    DateTime dttm = new DateTime();
                    dttm = DateTime.Now;
                    string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                    using (StreamWriter w = File.AppendText(logfilename))
                    {
                        string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT Outgoing Service Started";
                        w.WriteLine(filelog);
                    }
                }
            }
            catch (Exception ex )
            {
                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "Error:_btnS2start_Click" + ex.Message;
                w.WriteLine(filelog);
            } 
                
            }
            
        }
        //TIMER SERVICE FOR FCDB 
        public void OnTimertimerSWIFTOT(object sender, System.Timers.ElapsedEventArgs args)
        {
          try
           {
                // TODO: Insert monitoring activities here.

                //SWIFT instance
                SWIFT sw = new SWIFT();
               //timerSWIFTOT.Enabled = false; //Stop further time service until one operation is completed 
                
                sw.processOutgoingSwift(sFoldeSwiftrOt, dFolderSwiftOt1, dFolderSwiftOt2, dbSwiftOt);
               // sw.processOutgoingSwift("C:\\snocustin\\", "C:\\snocuout\\", "C:\\snocuout2\\", dbSwiftOt);
                
                 
              //  timerSWIFTOT.Enabled = true;   //Start again the timer to proces new incomming 
            }
          catch (Exception ex)
         {

             
            }


        }
        
       

        private void btnS2Restart_Click(object sender, EventArgs e)
        {
            try
            {
                //Buttons controls 
                btnS2Restart.Enabled = true;
                btnS2start.Enabled = false;
                btnS2Stop.Enabled = true;
                lblS2Display.Text = "Service is Running";
                //Close connection
                dbSwiftOt.Close();
                //Start connection 
                dbSwiftOt.Open();

                //Start timer service for swift 

                timerSWIFTOT.Interval = 30000; // 30 seconds
                timerSWIFTOT.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimertimerSWIFTOT);
                timerSWIFTOT.Start();

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT Outgoing Service Restarted";
                    w.WriteLine(filelog);
                }
            }
            catch( Exception ex)
            {
                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Error_btnS2Restart_Click" + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }
        private void btnS2Stop_Click(object sender, EventArgs e)
        {
            try
            {
                timerSWIFTOT.Enabled = false;   //Start again the timer to proces new incomming
                //Close connection
                dbSwiftOt.Close();
                //Button control 

                lblS2Display.ForeColor = Color.Red;
                lblS2Display.Text = "Service Stopped";
                btnS2start.Enabled = true;
                btnS2Restart.Enabled = false;
                btnS2Stop.Enabled = false;


                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT Outgoin Service Stopped";
                    w.WriteLine(filelog);
                }
            }
            catch (Exception ex)
            {

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Error_ btnS2Stop_Click" + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }

        #endregion

        
        #region SWIFT NON CUSTOMER
            private void btnS3start_Click(object sender, EventArgs e)
            {
                try
                {
                    //
                    //Check for destination folder 
                    if (!Directory.Exists(dFolderSwiftNonCust))
                    {
                        Directory.CreateDirectory(dFolderSwiftNonCust);

                    }


                    //check for source files if they do exist 
                    if (!Directory.Exists(sFolderSwiftNonCust))
                    {
                        MessageBox.Show("Service will not start source folder is missing or is not accessible,\n please check if the " + sFolderSwiftNonCust + " is Mapped to this machine", "Service Panel says \"Source directory not found\"", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        //Creating file logs 
                        DateTime dttm = new DateTime();
                        dttm = DateTime.Now;
                        string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                        using (StreamWriter w = File.AppendText(logfilename))
                        {
                            string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SService will not start source folder is missing or is not accessible,\n please check if the " + sFolderSwiftNonCust + "is Mapped to this machine";
                            w.WriteLine(filelog);
                        }
                    }
                    else
                    {
                        //Button control 

                        lblS3Display.ForeColor = Color.Green;
                        lblS3Display.Text = "Service is Running";
                        btnS3start.Enabled = false;
                        btnS3Restart.Enabled = true;
                        btnS3Stop.Enabled = true;

                        //Start timer service for swift 
                        timerSWIFTOTNONCU = new System.Timers.Timer();
                        timerSWIFTOTNONCU.Interval = 30000; // 30 seconds
                        timerSWIFTOTNONCU.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimertimerSWIFTOTNONCU);
                        timerSWIFTOTNONCU.Start();

                        //Creating file logs 
                        DateTime dttm = new DateTime();
                        dttm = DateTime.Now;
                        string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                        using (StreamWriter w = File.AppendText(logfilename))
                        {
                            string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Service Swift non Customer Started";
                            w.WriteLine(filelog);
                        }

                    }
                }
                catch (Exception ex)
                {
                    //Creating file logs 
                    DateTime dttm = new DateTime();
                    dttm = DateTime.Now;
                    string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                    using (StreamWriter w = File.AppendText(logfilename))
                    {
                        string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Errro:_btnS3start_Click: "+ ex.Message;
                        w.WriteLine(filelog);
                    }
                }
            }
            //TIMER SERVICE FOR FCDB 
            public void OnTimertimerSWIFTOTNONCU(object sender, System.Timers.ElapsedEventArgs args)
            {
              
                    // TODO: Insert monitoring activities here.

                    //SWIFT instance
                    SWIFT sw = new SWIFT();
                    //timerSWIFTOTNONCU.Enabled = false; //Stop further time service until one operation is completed 
                    dbSwiftNonCust = dbg.MSSQL_SWIFT_Out_connect();
                    sw.processOutgoingSwiftNonCustomer(sFolderSwiftNonCust, dFolderSwiftNonCust, dbSwiftNonCust); 
                    //sw.processOutgoingSwiftNonCustomer("C:\\snocuout\\", "C:\\snocuout2\\", dbSwiftNonCust); 
                    

                    //timerSWIFTOTNONCU.Enabled = true;   //Start again the timer to proces new incomming 
               


            }
      
        private void frmDashBoard_Load(object sender, EventArgs e)
        {
            db =dbg.Oracle_connect();
        }

        private void btnS3Stop_Click(object sender, EventArgs e)
        {
            try
            {
                timerSWIFTOTNONCU.Enabled = false;   //Start again the timer to proces new incomming
                timerSWIFTOTNONCU.Close();

                lblS3Display.ForeColor = Color.Red;
                lblS3Display.Text = "Service Stopped";
                btnS3start.Enabled = true;
                btnS3Restart.Enabled = false;
                btnS3Stop.Enabled = false;


              //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Service Swift non Customer Restarted";
                    w.WriteLine(filelog);
                }
            }
            catch (Exception ex)
            {
                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Error_btnS3Stop_Click" + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }

        private void btnS3Restart_Click(object sender, EventArgs e)
        {
            try
            {
                //Check for destination folder 
                if (!Directory.Exists(dFolderSwiftNonCust))
                {
                    Directory.CreateDirectory(dFolderSwiftNonCust);

                }


                //check for source files if they do exist 
                if (!Directory.Exists(sFolderSwiftNonCust))
                {
                    MessageBox.Show("Service will not start source folder is missing or is not accessible,\n please check if the " + sFolderSwiftNonCust + " is Mapped to this machine", "Service Panel says \"Source directory not found\"", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //Creating file logs 
                 //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                        string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SService will not start source folder " + sFolderSwiftNonCust + " is missing or is not accessible";
                        w.WriteLine(filelog);
                    }
                }
                else
                {
                    //Button control 

                    lblS3Display.ForeColor = Color.Green;
                    lblS3Display.Text = "Service is Running";
                    btnS3start.Enabled = false;
                    btnS3Restart.Enabled = true;
                    btnS3Stop.Enabled = true;

                    //Start timer service for swift 
                    timerSWIFTOTNONCU.Interval = 30000; // 30 seconds
                    timerSWIFTOTNONCU.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimertimerSWIFTOTNONCU);
                    timerSWIFTOTNONCU.Start();

                }
            }
            catch (Exception ex)
            {
                //Creating file logs 
                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Error_btnS3Restart_Click" + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }
        #endregion

        private void btnS6start_Click(object sender, EventArgs e)
        {
            try
            {
                //Buttons controls 
                btnS6Restart.Enabled = true;
                btnS6start.Enabled = false;
                btnS6Stop.Enabled = true;

                lblS6Display.ForeColor = Color.Green;
                lblS6Display.Text = "Service is Running";
                //Start timer service for swift 
                timerMoWFAILURE = new System.Timers.Timer();

                timerMoWFAILURE.Interval = 30000; // 30 seconds
                timerMoWFAILURE.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerMoWFAILURE);
                timerMoWFAILURE.Start();

                //Creating file logs 
               //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Failed Service Started";
                    w.WriteLine(filelog);
                }
            }
            catch (Exception ex)
            {
               
            }
        }

        //TIMER SERVICE FOR FCDB 
        public void OnTimerMoWFAILURE(object sender, System.Timers.ElapsedEventArgs args)
        {
            try 
           {
                // TODO: Insert monitoring activities here.

                
                //timerMoWFAILURE.Enabled = false; //Stop further time service until one operation is completed 

                MOWILES mw = new MOWILES();
                mw.processFailure(db);
               // timerMoWFAILURE.Enabled = true;   //Start again the timer to proces new incomming 
           }
            catch (Exception ex)
            {

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Error_timerMoWFAILURE:" +ex.Message;
                    w.WriteLine(filelog);
                }
            }


        }
        

        private void btnS5start_Click(object sender, EventArgs e)
        {
            try
            {

            

                 //Start timer service for FCDB  
                    timerFCDB_CMF = new System.Timers.Timer();
                    timerFCDB_CMF.Interval = 30000; // 30 seconds
                    timerFCDB_CMF.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerFCDB_CMF);
                    timerFCDB_CMF.Start();


                    //Additional control 
                    btnS5start.Enabled = false;
                    btnS5Restart.Enabled = true;
                    btnS5Stop.Enabled = true;

                    //Command status 

                    lblS5Display.ForeColor = Color.Green;
                    lblS5Display.Text = "Service is Running";

                   //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                        string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ FCDB CMF Service Started";
                        w.WriteLine(filelog);
                    }
            }
            catch (Exception ex)
            {

               //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Error_btnS5start_Click:" + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }

        public void OnTimerFCDB_CMF(object sender, System.Timers.ElapsedEventArgs args)
        {
            try
            {
                // TODO: Insert monitoring activities here.

                FCDB c = new FCDB();
                //timerFCDB_CMF.Enabled = false;
                mDb=dbg.connCMF_FCDB();

                c.processFCDBCMF(db, mDb); 
                //Create logs file 
                //timerFCDB_CMF.Enabled = true;
            }
            catch (Exception ex)
            {

                
            }


        }

        private void btnS5Restart_Click(object sender, EventArgs e)
        {
            try
            {
                timerFCDB_CMF.Enabled = false;

                timerFCDB_CMF.Enabled = true;
                //Additional control 
                btnS5start.Enabled = false;
                btnS5Restart.Enabled = true;
                btnS5Stop.Enabled = true;

                //Command status 

                lblS5Display.ForeColor = Color.Green;
                lblS5Display.Text = "Service is Running";

               //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ FCDB CMF Service Restarted";
                    w.WriteLine(filelog);
                }
            }
            catch(Exception ex)
            {
                
                    //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Error_btnS5Restart_Click" + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }

        private void btnS5Stop_Click(object sender, EventArgs e)
        {
            try
            {

            
            timerFCDB_CMF.Enabled = false;
            timerFCDB_CMF.Close();

            //Additional control 
            btnS5start.Enabled = true;
            btnS5Restart.Enabled = false;
            btnS5Stop.Enabled = false;

            //Command status 

            lblS5Display.ForeColor = Color.Red;
            lblS5Display.Text = "Service Stoped";

            //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ FCDB CMF Service stopped";
                w.WriteLine(filelog);
            }
                }
            catch (Exception ex )
            {
                
                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Error_btnS5Stop_Click: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }

        }

        private void btnS6Restart_Click(object sender, EventArgs e)
        {
            try
            {

            
            timerMoWFAILURE.Enabled = false;
            timerMoWFAILURE.Enabled = true;

            //Buttons controls 
            btnS6Restart.Enabled = false;
            btnS6start.Enabled =false;
            btnS6Stop.Enabled = true;


            lblS6Display.ForeColor = Color.Green;
            lblS6Display.Text = "Service is Running";

           //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Failed Service Restarted";
                w.WriteLine(filelog);
            }

            }
            catch (Exception ex)
            {

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss_") + "Error_btnS6Restart_Click: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }

        private void btnS6Stop_Click(object sender, EventArgs e)
        {
            try
            {
                timerMoWFAILURE.Enabled = false;
                timerMoWFAILURE.Close();

                //Buttons controls 
                btnS6Restart.Enabled = false;
                btnS6start.Enabled = true;
                btnS6Stop.Enabled = false;

                lblS6Display.ForeColor = Color.Red;
                lblS6Display.Text = "Service Stopped";

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Failed Service Stopped";
                    w.WriteLine(filelog);
                }
            }
            catch (Exception ex)
            {
               //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Error_btnS6Stop_Click" + ex.Message;
                    w.WriteLine(filelog);
                }
            }

        }

        private void btnS7Stop_Click(object sender, EventArgs e)
        {
            try
            {

                timerMoSUCCESS.Enabled = false;
                timerMoSUCCESS.Close();

                //Buttons controls 
                btnS7Restart.Enabled = false;
                btnS7start.Enabled = true;
                btnS7Stop.Enabled = false;

                lblS7Display.ForeColor = Color.Red;
                lblS7Display.Text = "Service Stopped";

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Success Service Stopped";
                    w.WriteLine(filelog);
                }
            }
            catch(Exception ex)
            {
                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Error_btnS7Stop_Click" + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }

        private void frmDashBoard_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                db.Close();
            }
            catch (Exception ex)
            {

                db.Close();
                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Error_frmDashBoard_FormClosed:" + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }

        private void btnS7start_Click(object sender, EventArgs e)
        {

            try
            {
                //Buttons controls 
                btnS7Restart.Enabled = true;
                btnS7start.Enabled = false;
                btnS7Stop.Enabled = true;

                //Start timer service for swift 
                timerMoSUCCESS = new System.Timers.Timer();
                timerMoSUCCESS.Interval = 30000; // 30 seconds
                timerMoSUCCESS.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerMoSUCCESS);
                timerMoSUCCESS.Start();

                lblS7Display.ForeColor = Color.Green;
                lblS7Display.Text = "Service is Running";

                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Success Service Started";
                    w.WriteLine(filelog);
                }
            }
            catch(Exception ex)
            {
               
            }
        }
        public void OnTimerMoSUCCESS(object sender, System.Timers.ElapsedEventArgs args)
        {
            try 
            {
               // TODO: Insert monitoring activities here.

                
              //timerMoSUCCESS.Enabled = false;

                MOWILES mw = new MOWILES();
               mw.processMoneyWirelessSuccess(db);
               
                //Create logs file 
               //timerMoSUCCESS.Enabled = true;
           }
            catch (Exception ex)
            {

               //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss_") + "Error_OnTimerMoSUCCESS: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }


        }

        private void btnS7Restart_Click(object sender, EventArgs e)
        {
            try
            {
                timerMoWFAILURE.Enabled = false;
                timerMoWFAILURE.Enabled = true;

                //Buttons controls 
                btnS7Restart.Enabled = false;
                btnS7start.Enabled = true;
                btnS7Stop.Enabled = false;


                lblS7Display.ForeColor = Color.Green;
                lblS7Display.Text = "Service is Running";

              //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Success Service Restarted";
                    w.WriteLine(filelog);
                }
            }
            catch(Exception ex)
            {
               //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss_") + "Error_btnS7Restart_Click: " + ex.Message;
                    w.WriteLine(filelog);
                }
            }
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnStartAll_Click(object sender, EventArgs e)
        {
            try 
            {
             //Service no 1
                //Check for destination folder 
                if (!Directory.Exists(dFolderSwift))
                {
                    Directory.CreateDirectory(dFolderSwift);

                }

                //check for source files if they do exist 
                if (!Directory.Exists(sFoldeSwiftr))
                {
                    MessageBox.Show("Service will not start source folder is missing or is not accessible,\n please check if the " + sFoldeSwiftr + " is Mapped to this machine","Service Panel says \"Source directory not found\"",MessageBoxButtons.OK,MessageBoxIcon.Error);

                   //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                        string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT: Incoming Messages Service will not start source folder is missing or is not accessible";
                        w.WriteLine(filelog);
                    }
                }
                else
                {
                    //Buttons controls 
                    btnS1Restart.Enabled = true;
                    btnS1start.Enabled = false;
                    btnS1Stop.Enabled = true;

                    lblS1Display.ForeColor = Color.Green;
                    lblS1Display.Text = "Service is Running";
                    //Start connection 
                    dbSwift = dbg.MSSQL_SWIFT_connect();

                    //Start timer service for swift 
                    timerSWIFTINCO = new System.Timers.Timer();
                    timerSWIFTINCO.Interval = 30000; // 30 seconds
                    timerSWIFTINCO.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerSWIFTIN);
                    timerSWIFTINCO.Start();

                    
                }
    //Service no 2

                //Check for destination folder 

               

                if (!Directory.Exists(dFolderSwiftOt1))
                {
                    Directory.CreateDirectory(dFolderSwiftOt1);

                }
                if (!Directory.Exists(dFolderSwiftOt2))
                {
                    Directory.CreateDirectory(dFolderSwiftOt2);

                }

                //check for source files if they do exist 
                if (!Directory.Exists(sFoldeSwiftrOt))
                {
                    MessageBox.Show("Service will not start source folder is missing or is not accessible,\n please check if the " + sFoldeSwiftrOt + " is Mapped to this machine", "Service Panel says \"Source directory not found\"", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //Creating file logs 
                    
                }
                else
                {
                    dbSwiftOt = dbg.MSSQL_SWIFT_Out_connect();
                    //Start timer service for swift 
                    timerSWIFTOT = new System.Timers.Timer();
                    timerSWIFTOT.Interval = 30000; // 30 seconds
                    timerSWIFTOT.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimertimerSWIFTOT);
                    timerSWIFTOT.Start();

                    //Button control 

                    lblS2Display.ForeColor = Color.Green;
                    lblS2Display.Text = "Service is Running";
                    btnS2start.Enabled = false;
                    btnS2Restart.Enabled = true;
                    btnS2Stop.Enabled = true;

                 
                }
        // Service no 3
		 

                    //Check for destination folder 
                    if (!Directory.Exists(dFolderSwiftNonCust))
                    {
                        Directory.CreateDirectory(dFolderSwiftNonCust);

                    }


                    //check for source files if they do exist 
                    if (!Directory.Exists(sFolderSwiftNonCust))
                    {
                        MessageBox.Show("Service will not start source folder is missing or is not accessible,\n please check if the " + sFolderSwiftNonCust + " is Mapped to this machine", "Service Panel says \"Source directory not found\"", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        //Creating file logs 
                    DateTime dttm = new DateTime();
                    dttm = DateTime.Now;
                    string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                    using (StreamWriter w = File.AppendText(logfilename))
                    {
                            string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SService will not start source folder is missing or is not accessible,\n please check if the " + sFolderSwiftNonCust + "is Mapped to this machine";
                            w.WriteLine(filelog);
                        }
                    }
                    else
                    {
                        //Button control 

                        lblS3Display.ForeColor = Color.Green;
                        lblS3Display.Text = "Service is Running";
                        btnS3start.Enabled = false;
                        btnS3Restart.Enabled = true;
                        btnS3Stop.Enabled = true;

                        //Start timer service for swift 
                        timerSWIFTOTNONCU = new System.Timers.Timer();
                        timerSWIFTOTNONCU.Interval = 30000; // 30 seconds
                        timerSWIFTOTNONCU.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimertimerSWIFTOTNONCU);
                        timerSWIFTOTNONCU.Start();


                    }
//Service no 4
                //Starting up the FCDB service 
                DateTime dt = new DateTime();
                dt = DateTime.Now;

                sourceFolder = "\\\\192.168.110.192\\d$\\BANKM\\Download\\";
                destinationFolder = "Z:\\FCDB\\" + dt.Year + "\\" + dt.ToString("MMMM") + "\\" + dt.Day + "\\";

                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }
                 //check for source files if they do exist 
                if (!Directory.Exists(sourceFolder))
                {
                    MessageBox.Show("Service will not start source folder is missing or is not accessible,\n please check if the " + sourceFolder + " is Mapped to this machine", "Service Panel says \"Source directory not found\"", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                        string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Service will not start source folder is missing or is not accessible,\n please check if the " + sourceFolder + "is Mapped to this machine";
                        w.WriteLine(filelog);
                    }
                }
                else
                {
                    //Start database session 
                   

                    //Start timer service for FCDB  
                    timerFCDB = new System.Timers.Timer();
                    timerFCDB.Interval = 30000; // 30 seconds
                    timerFCDB.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerFCDB);
                    timerFCDB.Start();


                    //Additional control 
                    btnS4start.Enabled = false;
                    btnS4Restart.Enabled = true;
                    btnS4Stop.Enabled = true;

                    //Command status 

                    lblS4Display.ForeColor = Color.Green;
                    lblS4Display.Text = "Service is Running";

                  
                }
            
       //Service no 5



                 //Start timer service for FCDB  
                    timerFCDB_CMF = new System.Timers.Timer();
                    timerFCDB_CMF.Interval = 30000; // 30 seconds
                    timerFCDB_CMF.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerFCDB_CMF);
                    timerFCDB_CMF.Start();


                    //Additional control 
                    btnS5start.Enabled = false;
                    btnS5Restart.Enabled = true;
                    btnS5Stop.Enabled = true;

                    //Command status 

                    lblS5Display.ForeColor = Color.Green;
                    lblS5Display.Text = "Service is Running";

                   
             
      //Service no 6
	 
                //Buttons controls 
                btnS6Restart.Enabled = true;
                btnS6start.Enabled = false;
                btnS6Stop.Enabled = true;

                lblS6Display.ForeColor = Color.Green;
                lblS6Display.Text = "Service is Running";
                //Start timer service for swift 
                timerMoWFAILURE = new System.Timers.Timer();

                timerMoWFAILURE.Interval = 30000; // 30 seconds
                timerMoWFAILURE.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerMoWFAILURE);
                timerMoWFAILURE.Start();

               
        //Service 7 
		
		 //Buttons controls 
                btnS7Restart.Enabled = true;
                btnS7start.Enabled = false;
                btnS7Stop.Enabled = true;

                //Start timer service for swift 
                timerMoSUCCESS = new System.Timers.Timer();
                timerMoSUCCESS.Interval = 30000; // 30 seconds
                timerMoSUCCESS.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimerMoSUCCESS);
                timerMoSUCCESS.Start();

                lblS7Display.ForeColor = Color.Green;
                lblS7Display.Text = "Service is Running";

               //Creating file logs 
                DateTime dttm1 = new DateTime();
                dttm1 = DateTime.Now;
                string logfilename1 = "AppLogs" + dttm1.Year + dttm1.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename1))
                {
                    string filelog = dttm1.Year + dttm1.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Success Service Started";
                    w.WriteLine(filelog);
					
					string filelog1 = dttm1.Year + dttm1.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Failed Service Started";
                    w.WriteLine(filelog1);

                    string filelog2 = dttm1.Year + dttm1.ToString("-MM-dd hh:mm:ss") + "_Service FCDB CMF Service Started";
                     w.WriteLine(filelog2);
					 
					string filelog3 = dttm1.Year + dttm1.ToString("-MM-dd hh:mm:ss") + "_Service FCDB File upload Started";
                     w.WriteLine(filelog3);

                     string filelog4 = dttm1.Year + dttm1.ToString("-MM-dd hh:mm:ss") + "_Service Swift Outgoing non Customer Started";
                    w.WriteLine(filelog4);

                    string filelog6 = dttm1.Year + dttm1.ToString("-MM-dd hh:mm:ss") + "_Service SWIFT: Incoming Messages Service Started";
                     w.WriteLine(filelog6);

                     string filelog7 = dttm1.Year + dttm1.ToString("-MM-dd hh:mm:ss") + "_Service SWIFT Outgoing Service Started";
                        w.WriteLine(filelog7);
                }
                btnStopAll.Enabled = true;
                btnStartAll.Enabled = false;


	}
catch(Exception ex )
{

    MessageBox.Show(ex.Message,"Service Panel says");
   //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelogtm = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Srvice_All_Start_Error: " + ex.Message;
                    w.WriteLine(logfilename);
                }
}	
				
	
        }

        private void btnStopAll_Click(object sender, EventArgs e)
        {
            try
              {
                DialogResult input = MessageBox.Show("Your about to stop  all services!\n Do you want to proceed?", " Stop all services", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (input == DialogResult.Yes)
                {

                    //Service no 1 
                    btnS1start.Enabled = true;
                    btnS1Restart.Enabled = false;
                    btnS1Stop.Enabled = false;

                    //Command status 

                    lblS1Display.ForeColor = Color.Red;
                    lblS1Display.Text = "Service Stoped";

                    timerSWIFTINCO.Enabled = false;   //Start again the timer to proces new incomming
                    //Close connection
                    dbSwift.Close();


                    //Service no 2
                    timerSWIFTOT.Enabled = false;   //Start again the timer to proces new incomming
                    //Close connection
                    dbSwiftOt.Close();
                    //Button control 

                    lblS2Display.ForeColor = Color.Red;
                    lblS2Display.Text = "Service Stopped";
                    btnS2start.Enabled = true;
                    btnS2Restart.Enabled = false;
                    btnS2Stop.Enabled = false;



                    //Service 3 
                    timerSWIFTOTNONCU.Enabled = false;   //Start again the timer to proces new incomming
                    timerSWIFTOTNONCU.Close();

                    lblS3Display.ForeColor = Color.Red;
                    lblS3Display.Text = "Service Stopped";
                    btnS3start.Enabled = true;
                    btnS3Restart.Enabled = false;
                    btnS3Stop.Enabled = false;


                    //Service 4

                    timerFCDB.Enabled = false;
                    timerFCDB.Close();

                    btnS4start.Enabled = true;
                    btnS4Stop.Enabled = false;
                    btnS4Restart.Enabled = false;

                    //Command status 

                    lblS4Display.ForeColor = Color.Red;
                    lblS4Display.Text = "Service is Stoped";

                    //Service 5 


                    timerFCDB_CMF.Enabled = false;
                    timerFCDB_CMF.Close();

                    //Additional control 
                    btnS5start.Enabled = true;
                    btnS5Restart.Enabled = false;
                    btnS5Stop.Enabled = false;

                    //Command status 

                    lblS5Display.ForeColor = Color.Red;
                    lblS5Display.Text = "Service Stoped";

                    //Services 6

                    timerMoWFAILURE.Enabled = false;
                    timerMoWFAILURE.Close();

                    //Buttons controls 
                    btnS6Restart.Enabled = false;
                    btnS6start.Enabled = true;
                    btnS6Stop.Enabled = false;

                    lblS6Display.ForeColor = Color.Red;
                    lblS6Display.Text = "Service Stopped";

                    //Service 7



                    timerMoSUCCESS.Enabled = false;
                    timerMoSUCCESS.Close();

                    //Buttons controls 
                    btnS7Restart.Enabled = true;
                    btnS7start.Enabled = true;
                    btnS7Stop.Enabled = false;

                    lblS7Display.ForeColor = Color.Red;
                    lblS7Display.Text = "Service Stopped";

                    //enable  start buttons
                    btnStartAll.Enabled = true;
                    btnStopAll.Enabled = false;

                    db.Close();

                  //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                        string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Success Service Stopped";
                        w.WriteLine(filelog);

                        string filelog1 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Failed Service Stopped";
                        w.WriteLine(filelog1);

                        string filelog2 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ FCDB CMF Service stopped";
                        w.WriteLine(filelog2);

                        string filelog3 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Service Stopped";
                        w.WriteLine(filelog3);

                        string filelog4 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Service Swift non Customer Restarted";
                        w.WriteLine(filelog4);

                        string filelog5 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT Outgoin Service Stopped";
                        w.WriteLine(filelog5);

                        string filelog6 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT: Incoming Messages Service Stopped";
                        w.WriteLine(filelog6);


                    }
                }
            
        }
          catch (Exception ex)
            {

                //enable  start buttons
                btnStartAll.Enabled = true;
                btnStopAll.Enabled = false;
                db.Close();

                // MessageBox.Show(ex.Message, "Service Panel says");
                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                  string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Service_All_Stop_Error: " + ex.Message;
                  w.WriteLine(filelog);
               }


            }
        
        }

        private void btnExitClose_Click(object sender, EventArgs e)
        {
            try
            {

                DialogResult input = MessageBox.Show("Your about to stop and close all services!\n Do you want to proceed?", " Stop all services", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (input == DialogResult.Yes)
                {
                    try
                    {
                        db.Close();

                        //Creating file logs 
                        DateTime dttm = new DateTime();
                        dttm = DateTime.Now;
                        string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                        using (StreamWriter w = File.AppendText(logfilename))
                        {
                            string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Success Service Stopped";
                            w.WriteLine(filelog);

                            string filelog1 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Money Wireless Fund Transfer Failed Service Stopped";
                            w.WriteLine(filelog1);

                            string filelog2 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ FCDB CMF Service stopped";
                            w.WriteLine(filelog2);

                            string filelog3 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ Service Stopped";
                            w.WriteLine(filelog3);

                            string filelog4 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_Service Swift non Customer Restarted";
                            w.WriteLine(filelog4);

                            string filelog5 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT Outgoin Service Stopped";
                            w.WriteLine(filelog5);

                            string filelog6 = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss") + "_ SWIFT: Incoming Messages Service Stopped";
                            w.WriteLine(filelog6);


                        }

                        this.Close();
                    }
                    catch (Exception ex)
                    {

                        db.Close();
                    }
                }
            }
            catch(Exception ex )
            {
                //Creating file logs 
                DateTime dttm = new DateTime();
                dttm = DateTime.Now;
                string logfilename = "AppLogs" + dttm.Year + dttm.ToString("-MM-dd") + ".fcdb";
                using (StreamWriter w = File.AppendText(logfilename))
                {
                    string filelog = dttm.Year + dttm.ToString("-MM-dd hh:mm:ss_") + "Error_On closing dashboard form: " + ex.Message;
                    w.WriteLine(filelog); 
                }
            }
            
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void lblS5Display_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

    }
}
