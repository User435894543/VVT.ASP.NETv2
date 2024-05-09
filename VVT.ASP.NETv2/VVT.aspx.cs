using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//crystal reports
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;


//database
using Microsoft.VisualBasic;
using System.IO;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;

namespace VVT.ASP.NETv2
{
    public partial class VVT : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //if (!IsPostBack) {

            //   printjobTicketLbl.Text = "...Running";

            //}

        }




        #region print job ticket
        protected void printJobTicketBttn_Click(object sender, EventArgs e)
        {

            string jobNumber = jobNumTextBox.Text;

            //here is where i will put the creation and manipualtion of the crystal report object
            #region start crystal report config:

            #region connection and CR object properties/settings
            //rerport object



            CrystalReport2 cryrpt = new CrystalReport2();

            cryrpt.DataSourceConnections.Clear();  //clear the connections (will popualte with fresh sql query defined data

            //set the databse login info, use twice first one is to login into sql server

            //getting write only error


            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");
            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this does not error out!?
            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //try our best to never have paramter panel pop-up, works like 95% of time
            //error here as well?
            //LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;

            #endregion connectiona and CR object properties/settings

            #region UI on CR object editing (non-DB dependant)

            //change text object, tell user what ticket the are curentlly running/looking at
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Full Ticket";


            #endregion UI on CR object editing (non-DB dependant)

            #endregion crystal report config end




            //here is where i need to clean up lots, make functions, sql queryies, DT and DS manipualtion, pass the job number, etc 
            //main code for program functionallity
            #region DB connection 


            //here is where i iwll be connecting to DB's (all queries will need access to basically my connection properties globals)
            #region global connection properties

            //connection string for DB
            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //open th econnection and error check
            OdbcConnection dbConn = new OdbcConnection(connectStr);
            dbConn.ConnectionTimeout = 150;
            try
            {
                dbConn.Open();
            }
            catch (Exception ex)
            {

                string error = ex + " : DB error cannot connect";

                ErrorLog(error);

            }
            #endregion global connection propeties




            //here is where i can change the UI of the report based on database data
            //ex) show word nailing on report if job# has a 810 tag associated with it
            #region UI crystal report editing (DB dependant)


            #region Header

            //set the job numbers from iuser input
            CrystalDecisions.CrystalReports.Engine.TextObject jobNum1;
            jobNum1 = cryrpt.ReportDefinition.ReportObjects["jobNum"] as TextObject;
            jobNum1.Text = jobNumber;

            CrystalDecisions.CrystalReports.Engine.TextObject jobNum2;
            jobNum2 = cryrpt.ReportDefinition.ReportObjects["jobNum2"] as TextObject;
            jobNum2.Text = jobNumber;



            String headerJob = "SELECT \"Job-Desc\", \"Date-Promised\", \"Sales-Rep-ID\", \"CSR-ID\", \"" +
                "PO-Number\", \"Overs-Allowed\", \"Last-Estimate-ID\", \"Quantity-Ordered\", \"Contact-Name\", \"Date-Entered\", \"Cust-ID-Ordered-by\"" +
                " FROM PUB.JOB WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtHeader = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter header = new OdbcDataAdapter(headerJob, dbConn);
                header.Fill(dtHeader);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set all text objects to the data from datatable
            try
            {
                //job descriptions
                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc;
                jobDesc = cryrpt.ReportDefinition.ReportObjects["jobDesc"] as TextObject;
                jobDesc.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc2;
                jobDesc2 = cryrpt.ReportDefinition.ReportObjects["jobDesc2"] as TextObject;
                jobDesc2.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                //date promised
                CrystalDecisions.CrystalReports.Engine.TextObject dateProm;
                dateProm = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
                dateProm.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject dateProm2;
                dateProm2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
                dateProm2.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                //qty, format from "3400" -> "3,400"
                CrystalDecisions.CrystalReports.Engine.TextObject qty;
                qty = cryrpt.ReportDefinition.ReportObjects["qty"] as TextObject;
                int qtyFormat = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty.Text = String.Format("{0:N0}", qtyFormat);

                CrystalDecisions.CrystalReports.Engine.TextObject qty2;
                qty2 = cryrpt.ReportDefinition.ReportObjects["qty2"] as TextObject;
                int qtyFormat2 = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty2.Text = String.Format("{0:N0}", qtyFormat2);

                //end formatting qty

                //job contact
                CrystalDecisions.CrystalReports.Engine.TextObject jobContact;
                jobContact = cryrpt.ReportDefinition.ReportObjects["contactName"] as TextObject;
                jobContact.Text = dtHeader.Rows[0]["Contact-Name"].ToString();

                //job date entered
                CrystalDecisions.CrystalReports.Engine.TextObject jobDE;
                jobDE = cryrpt.ReportDefinition.ReportObjects["jobDE"] as TextObject;
                jobDE.Text = dtHeader.Rows[0]["Date-Entered"].ToString();

                //over allowed
                CrystalDecisions.CrystalReports.Engine.TextObject OA;
                OA = cryrpt.ReportDefinition.ReportObjects["jobOA"] as TextObject;
                OA.Text = dtHeader.Rows[0]["Overs-Allowed"].ToString();

                //po num
                CrystalDecisions.CrystalReports.Engine.TextObject PO;
                PO = cryrpt.ReportDefinition.ReportObjects["poNum"] as TextObject;
                PO.Text = dtHeader.Rows[0]["PO-Number"].ToString();

                //estimate
                string est = dtHeader.Rows[0]["Last-Estimate-ID"].ToString().Insert(6, "-");
                CrystalDecisions.CrystalReports.Engine.TextObject estimate;
                estimate = cryrpt.ReportDefinition.ReportObjects["estimate"] as TextObject;
                estimate.Text = est;
            }
            catch (Exception ex) { }
            //customer query and text objects
            String headerCust = "SELECT \"cust-name\", \"Address-1\", \"Address-2\", \"City\", \"" +
               "State\", \"Zip\", \"Phone\", \"Address-3\", \"Group-ID\"FROM PUB.cust WHERE \"Cust-code\" = " + dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();




            DataTable dtCust = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter cust = new OdbcDataAdapter(headerCust, dbConn);
                cust.Fill(dtCust);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {



                //No viso labels
                CrystalDecisions.CrystalReports.Engine.TextObject visoLbl;
                visoLbl = cryrpt.ReportDefinition.ReportObjects["Text37"] as TextObject;
                //if the customer group-ID contains "broker" then
                if (dtCust.Rows[0]["Group-ID"].ToString().Contains("broker") || dtCust.Rows[0]["Group-ID"].ToString().Contains("Broker"))
                {

                    visoLbl.Text = "BLIND SHIP";

                }


                //set the Customer info text objects
                //cust name
                CrystalDecisions.CrystalReports.Engine.TextObject custName;
                custName = cryrpt.ReportDefinition.ReportObjects["custName"] as TextObject;
                custName.Text = dtCust.Rows[0]["cust-name"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject custName2;
                custName2 = cryrpt.ReportDefinition.ReportObjects["custName2"] as TextObject;
                custName2.Text = dtCust.Rows[0]["cust-name"].ToString();

                //address -> add 1 and 2 and 3 combined
                CrystalDecisions.CrystalReports.Engine.TextObject custAdd;
                custAdd = cryrpt.ReportDefinition.ReportObjects["custAddress"] as TextObject;
                custAdd.Text = dtCust.Rows[0]["Address-1"].ToString() + " " + dtCust.Rows[0]["Address-2"].ToString() + " " + dtCust.Rows[0]["Address-3"].ToString();

                //city state zip customer
                CrystalDecisions.CrystalReports.Engine.TextObject custCSZ;
                custCSZ = cryrpt.ReportDefinition.ReportObjects["custCSZ"] as TextObject;
                custCSZ.Text = dtCust.Rows[0]["City"].ToString() + " " + dtCust.Rows[0]["State"].ToString() + " " + dtCust.Rows[0]["Zip"].ToString();

                //customerPhone
                CrystalDecisions.CrystalReports.Engine.TextObject custPhone;
                custPhone = cryrpt.ReportDefinition.ReportObjects["custPhone"] as TextObject;
                custPhone.Text = dtCust.Rows[0]["Phone"].ToString();

                //customer number
                CrystalDecisions.CrystalReports.Engine.TextObject custNum;
                custNum = cryrpt.ReportDefinition.ReportObjects["custNum"] as TextObject;
                custNum.Text = dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();

                //sales agent query and txt obj change

            }
            catch (Exception ex) { }

            String headerSalesAgent = "SELECT \"Sales-Agent-Name\" FROM PUB.\"sales-agent\" WHERE \"Sales-agent-id\" = " + "'" + dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "'";

            // String headerSalesAgent = "SELECT \"Sales-agent-id\" , \"Sales-Agent-Name\" FROM PUB.\"sales-agent\"";

            DataTable dtSalesAgent = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter salesAgent = new OdbcDataAdapter(headerSalesAgent, dbConn);
                salesAgent.Fill(dtSalesAgent);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {
                //sales agent name
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep;
                salesRep = cryrpt.ReportDefinition.ReportObjects["salesRep"] as TextObject;
                salesRep.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

                //sales rep ID for billing
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep2;
                salesRep2 = cryrpt.ReportDefinition.ReportObjects["salesRep2"] as TextObject;
                salesRep2.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString();// + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();
            }
            catch (Exception ex) { }


            //csr query and txt obj change
            String headerCSR = "SELECT \"CSR-Name\" FROM PUB.CSR WHERE \"CSR-ID\"=" + "'" + dtHeader.Rows[0]["CSR-ID"].ToString() + "'";

            DataTable dtCsr = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter csrAdap = new OdbcDataAdapter(headerCSR, dbConn);
                csrAdap.Fill(dtCsr);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {
                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = dtHeader.Rows[0]["CSR-ID"].ToString() + "-" + dtCsr.Rows[0]["CSR-Name"].ToString();
            }
            catch (Exception ex)
            {

                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = "";

            }
            #endregion Header


            #region 810 tag check - show MAILING
            //DataTable for all UI db-depenedant editing
            DataTable dtEdit = new DataTable();


            String query810Tag = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumber;

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810Tag = new OdbcDataAdapter(query810Tag, dbConn);
                adap810Tag.Fill(dtEdit);
            }
            catch (Exception ex)
            {


                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //here is needed for 810 chck
            bool check = false;

            foreach (DataRow dr in dtEdit.Rows)
            {
                //do nothing 
                if (dr["Work-Center-ID"].ToString().Contains("810"))
                {

                    check = true;
                }

            }//end foreach

            CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
            txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
            //no check to see if 810 tag is present
            if (!check)
            {
                txtObj.Border.LeftLineStyle = 0;
                txtObj.Border.RightLineStyle = 0;
                txtObj.Border.TopLineStyle = 0;
                txtObj.Border.BottomLineStyle = 0;
                txtObj.Text = "";


            }//end check for 810
            else
            {
                txtObj.Border.BorderColor = Color.LawnGreen;
            }
            #endregion 810 tag check - show MAILING

            //add section Samples orange box check


            //can use same data from above query and dataTable to get the the job status and description
            //ex) 50d status and it's description is ready to run on digital press
            #region Job free fields subRPt
            string queryFF = "SELECT \"Free-Field-Char\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);
                adapFF.Fill(dtFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Free fields report";

                ErrorLog(error);
            }


            try
            {

                //here is where i need to convert decimal -> free-fieldchar
                string lastJob = dtFF.Rows[1]["Free-Field-Decimal"].ToString();
                dtFF.Rows[1]["Free-Field-Char"] = lastJob;


                dtFF.Columns.Remove("Free-Field-Decimal");
                dtFF.Columns.Add("FFname");
                dtFF.Rows[0]["FFname"] = "Customer To Supply";
                dtFF.Rows[1]["FFname"] = "Last Job#";
                dtFF.Rows[2]["FFname"] = "Type of Proof Needed";
                dtFF.Rows[3]["FFname"] = "Proof Needed By";
                dtFF.Rows[4]["FFname"] = "Coating";
                dtFF.Rows[5]["FFname"] = "Perf Score";
                dtFF.Rows[6]["FFname"] = "Certification";
                dtFF.Rows[7]["FFname"] = "Mailing Services";
                dtFF.Rows[8]["FFname"] = "Litho/Digital/DSF/ASI";
                dtFF.Rows[9]["FFname"] = "Samples";

                CrystalDecisions.CrystalReports.Engine.TextObject txtObjSample;
                txtObjSample = cryrpt.ReportDefinition.ReportObjects["txtSample"] as TextObject;

                if (dtFF.Rows[9]["Free-Field-Char"].ToString() != "None")
                {
                    txtObjSample.Text = "";
                    txtObjSample.Border.BorderColor = Color.Orange;
                    txtObjSample.Text = dtFF.Rows[9]["Free-Field-Char"].ToString();
                }
                else
                {

                    txtObjSample.Border.LeftLineStyle = 0;
                    txtObjSample.Border.RightLineStyle = 0;
                    txtObjSample.Border.TopLineStyle = 0;
                    txtObjSample.Border.BottomLineStyle = 0;
                    txtObjSample.Text = "";
                }
            }
            catch (Exception ex) { }

            //also check if empty exists it is empty hideSubs it
            if (dtFF.Rows.Count != 0)
            {

                dtFF.Rows.RemoveAt(6);
                dtFF.Rows.RemoveAt(4);
                dtFF.Rows.RemoveAt(3);
                dtFF.Rows.RemoveAt(2);
                dtFF.Rows.RemoveAt(0);
                dtFF.AcceptChanges();

                //will have to do same process as adding field to the dataSet as in Mail Version
                //also need to flag DSF jobs 

                if (dtFF.Rows[0]["Free-Field-Char"].ToString() == "DSF")
                {

                    CrystalDecisions.CrystalReports.Engine.TextObject txtDSF;
                    txtDSF = cryrpt.ReportDefinition.ReportObjects["txtDSF"] as TextObject;
                    txtDSF.Text = "DSF";

                }

                cryrpt.Subreports["subJobFreeFields"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobFreeFields"].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subJobFreeFields";
                HideSubs(cryrpt, subFF);

            }


            #endregion job free fields subRpt

            #endregion UI
            dbConn.Close();
            #endregion db connect

            #region hidesubs
            string subMailVersionFF = "subMailVersion";
            HideSubs(cryrpt, subMailVersionFF);

            string subMailVersionFF2 = "subMailVersion2";
            HideSubs(cryrpt, subMailVersionFF2);

            string subBinderyMatts = "subBinderyMatts";
            HideSubs(cryrpt, subBinderyMatts);

            string subMailFF = "subMailFreeFields";
            HideSubs(cryrpt, subMailFF);

            string subJobComments = "subJobNotes";
            HideSubs(cryrpt, subJobComments);

            string subAlt = "subAlterations";
            HideSubs(cryrpt, subAlt);

            string subFF2 = "subJobFreeFields";
            HideSubs(cryrpt, subFF2);

            string subPOreq = "subPOReq";
            HideSubs(cryrpt, subPOreq);

            string subPOLine = "subPOLine";
            HideSubs(cryrpt, subPOLine);

            string subShipTo = "subShipTo";
            HideSubs(cryrpt, subShipTo);

            string subFormSpec = "subFormSpecs";
            HideSubs(cryrpt, subFormSpec);

            string subFormNotes = "subFormNotes";
            HideSubs(cryrpt, subFormNotes);

            string subPrepress = "subPrepress";
            HideSubs(cryrpt, subPrepress);

            string subPress = "subPress";
            HideSubs(cryrpt, subPress);

            string subStock = "subStock";
            HideSubs(cryrpt, subStock);

            string subBindery = "subBindery";
            HideSubs(cryrpt, subBindery);

            string sub810Notes = "sub810Notes";
            HideSubs(cryrpt, sub810Notes);


            string subJobMV = "subJobMailVersion";
            HideSubs(cryrpt, subJobMV);

            string subJobMV2 = "subJobMailVersion2";
            HideSubs(cryrpt, subJobMV2);

            string subestiDetail = "subEstimateDetails";
            HideSubs(cryrpt, subestiDetail);

            string subestiDetail2 = "subEstimateDetails2";
            HideSubs(cryrpt, subestiDetail2);

            string subestiDetail3 = "subEstimateDetails3";
            HideSubs(cryrpt, subestiDetail3);

            //use radio button for yes or no billing section
            //cryrpt.ReportDefinition.Sections["billingSection"].SectionFormat.EnableSuppress = true;


            #endregion hideSubs

            //remove the date promised and due date here
            CrystalDecisions.CrystalReports.Engine.TextObject txt4;
            txt4 = cryrpt.ReportDefinition.ReportObjects["Text4"] as TextObject;
            txt4.Text = "";

            CrystalDecisions.CrystalReports.Engine.TextObject txt12;
            txt12 = cryrpt.ReportDefinition.ReportObjects["Text12"] as TextObject;
            txt12.Text = "";

            CrystalDecisions.CrystalReports.Engine.TextObject datePromClear;
            datePromClear = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
            datePromClear.Text = "";

            CrystalDecisions.CrystalReports.Engine.TextObject datePromClear2;
            datePromClear2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
            datePromClear2.Text = "";



            #region refresh rpt

            CrystalReportViewer1.ReportSource = cryrpt;
            CrystalReportViewer1.RefreshReport();

            cryrpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "VVT Report");


            #endregion refresh rpt
        }
        #endregion print job ticket



        public void ErrorLog(string logWrite)
        {

            //write to a public txt file (\\visonas\Public\Kyle\errorLog)
            string machineName = Environment.MachineName;

            logWrite += " : on computer name - " + machineName;

            //remove this and just write errors to log file
            //MessageBox.Show("Please copy and paste the next pop-up box and save to txt file");
            //MessageBox.Show(logWrite);

            //real path:                                       \\visonas\public\kyle\vvt releases\log/vvt_log.txt
           using (StreamWriter writeErrors = new StreamWriter(@"\\visonas\public\kyle\vvt releases\log/vvt_log.txt"))
            {
           
              writeErrors.WriteLine(logWrite);
           }

        }

        public void HideSubs(CrystalReport2 crObj, string str)
        {

            try
            {
                var subRPT = (SubreportObject)crObj.ReportDefinition.ReportObjects[str];
                subRPT.ObjectFormat.EnableSuppress = true;

            }
            catch (Exception ex)
            {

                string error = ex + " : Error in HideSubs(); : ";

                ErrorLog(error);

            }
        }//end Hide subs



        #region pre press ticket
        protected void prePressTicketBttn_Click1(object sender, EventArgs e)
        {
            string jobNumber = jobNumTextBox.Text;

            #region connection and CR object properties/settings
            //rerport object



            CrystalReport2 cryrpt = new CrystalReport2();

            cryrpt.DataSourceConnections.Clear();  //clear the connections (will popualte with fresh sql query defined data

            //set the databse login info, use twice first one is to login into sql server

            //getting write only error


            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");
            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this does not error out!?
            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //try our best to never have paramter panel pop-up, works like 95% of time
            //error here as well?
            //LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;

            #endregion connectiona and CR object properties/settings

            #region UI on CR object editing (non-DB dependant)

            //change text object, tell user what ticket the are curentlly running/looking at
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Prepress/Press Ticket";


            #endregion UI on CR object editing (non-DB dependant)


            //here is where i need to clean up lots, make functions, sql queryies, DT and DS manipualtion, pass the job number, etc 
            //main code for program functionallity
            #region DB connection 


            //here is where i iwll be connecting to DB's (all queries will need access to basically my connection properties globals)
            #region global connection properties

            //connection string for DB
            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //open th econnection and error check
            OdbcConnection dbConn = new OdbcConnection(connectStr);
            dbConn.ConnectionTimeout = 150;
            try
            {
                dbConn.Open();
            }
            catch (Exception ex)
            {

                string error = ex + " : DB error cannot connect";

                ErrorLog(error);

            }
            #endregion global connection propeties




            //here is where i can change the UI of the report based on database data
            //ex) show word nailing on report if job# has a 810 tag associated with it
            #region UI crystal report editing (DB dependant)


            #region Header

            //set the job numbers from iuser input
            CrystalDecisions.CrystalReports.Engine.TextObject jobNum1;
            jobNum1 = cryrpt.ReportDefinition.ReportObjects["jobNum"] as TextObject;
            jobNum1.Text = jobNumber;

            CrystalDecisions.CrystalReports.Engine.TextObject jobNum2;
            jobNum2 = cryrpt.ReportDefinition.ReportObjects["jobNum2"] as TextObject;
            jobNum2.Text = jobNumber;



            String headerJob = "SELECT \"Job-Desc\", \"Date-Promised\", \"Sales-Rep-ID\", \"CSR-ID\", \"" +
                "PO-Number\", \"Overs-Allowed\", \"Last-Estimate-ID\", \"Quantity-Ordered\", \"Contact-Name\", \"Date-Entered\", \"Cust-ID-Ordered-by\"" +
                " FROM PUB.JOB WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtHeader = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter header = new OdbcDataAdapter(headerJob, dbConn);
                header.Fill(dtHeader);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set all text objects to the data from datatable

            try
            {
                //job descriptions
                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc;
                jobDesc = cryrpt.ReportDefinition.ReportObjects["jobDesc"] as TextObject;
                jobDesc.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc2;
                jobDesc2 = cryrpt.ReportDefinition.ReportObjects["jobDesc2"] as TextObject;
                jobDesc2.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                //date promised
                CrystalDecisions.CrystalReports.Engine.TextObject dateProm;
                dateProm = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
                dateProm.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject dateProm2;
                dateProm2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
                dateProm2.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                //qty, format from "3400" -> "3,400"
                CrystalDecisions.CrystalReports.Engine.TextObject qty;
                qty = cryrpt.ReportDefinition.ReportObjects["qty"] as TextObject;
                int qtyFormat = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty.Text = String.Format("{0:N0}", qtyFormat);

                CrystalDecisions.CrystalReports.Engine.TextObject qty2;
                qty2 = cryrpt.ReportDefinition.ReportObjects["qty2"] as TextObject;
                int qtyFormat2 = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty2.Text = String.Format("{0:N0}", qtyFormat2);

                //end formatting qty

                //job contact
                CrystalDecisions.CrystalReports.Engine.TextObject jobContact;
                jobContact = cryrpt.ReportDefinition.ReportObjects["contactName"] as TextObject;
                jobContact.Text = dtHeader.Rows[0]["Contact-Name"].ToString();

                //job date entered
                CrystalDecisions.CrystalReports.Engine.TextObject jobDE;
                jobDE = cryrpt.ReportDefinition.ReportObjects["jobDE"] as TextObject;
                jobDE.Text = dtHeader.Rows[0]["Date-Entered"].ToString();

                //over allowed
                CrystalDecisions.CrystalReports.Engine.TextObject OA;
                OA = cryrpt.ReportDefinition.ReportObjects["jobOA"] as TextObject;
                OA.Text = dtHeader.Rows[0]["Overs-Allowed"].ToString();

                //po num
                CrystalDecisions.CrystalReports.Engine.TextObject PO;
                PO = cryrpt.ReportDefinition.ReportObjects["poNum"] as TextObject;
                PO.Text = dtHeader.Rows[0]["PO-Number"].ToString();

                //estimate
                string est = dtHeader.Rows[0]["Last-Estimate-ID"].ToString().Insert(6, "-");
                CrystalDecisions.CrystalReports.Engine.TextObject estimate;
                estimate = cryrpt.ReportDefinition.ReportObjects["estimate"] as TextObject;
                estimate.Text = est;
            }
            catch (Exception ex) { }
            //customer query and text objects
            String headerCust = "SELECT \"cust-name\", \"Address-1\", \"Address-2\", \"City\", \"" +
               "State\", \"Zip\", \"Phone\", \"Address-3\", \"Group-ID\"FROM PUB.cust WHERE \"Cust-code\" = " + dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();


            DataTable dtCust = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter cust = new OdbcDataAdapter(headerCust, dbConn);
                cust.Fill(dtCust);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set the Customer info text objects
            try
            {

                //No viso labels
                CrystalDecisions.CrystalReports.Engine.TextObject visoLbl;
                visoLbl = cryrpt.ReportDefinition.ReportObjects["Text37"] as TextObject;
                //if the customer group-ID contains "broker" then
                if (dtCust.Rows[0]["Group-ID"].ToString().Contains("broker") || dtCust.Rows[0]["Group-ID"].ToString().Contains("Broker"))
                {

                    visoLbl.Text = "BLIND SHIP";

                }

                //cust name
                CrystalDecisions.CrystalReports.Engine.TextObject custName;
                custName = cryrpt.ReportDefinition.ReportObjects["custName"] as TextObject;
                custName.Text = dtCust.Rows[0]["cust-name"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject custName2;
                custName2 = cryrpt.ReportDefinition.ReportObjects["custName2"] as TextObject;
                custName2.Text = dtCust.Rows[0]["cust-name"].ToString();

                //address -> add 1 and 2 and 3 combined
                CrystalDecisions.CrystalReports.Engine.TextObject custAdd;
                custAdd = cryrpt.ReportDefinition.ReportObjects["custAddress"] as TextObject;
                custAdd.Text = dtCust.Rows[0]["Address-1"].ToString() + " " + dtCust.Rows[0]["Address-2"].ToString() + " " + dtCust.Rows[0]["Address-3"].ToString();

                //city state zip customer
                CrystalDecisions.CrystalReports.Engine.TextObject custCSZ;
                custCSZ = cryrpt.ReportDefinition.ReportObjects["custCSZ"] as TextObject;
                custCSZ.Text = dtCust.Rows[0]["City"].ToString() + " " + dtCust.Rows[0]["State"].ToString() + " " + dtCust.Rows[0]["Zip"].ToString();

                //customerPhone
                CrystalDecisions.CrystalReports.Engine.TextObject custPhone;
                custPhone = cryrpt.ReportDefinition.ReportObjects["custPhone"] as TextObject;
                custPhone.Text = dtCust.Rows[0]["Phone"].ToString();

                //customer number
                CrystalDecisions.CrystalReports.Engine.TextObject custNum;
                custNum = cryrpt.ReportDefinition.ReportObjects["custNum"] as TextObject;
                custNum.Text = dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();
            }
            catch (Exception ex) { }
            //sales agent query and txt obj change
            //why this does not work i have no fkin clue, making stack overflwo see what the brians can thunk up
            String headerSalesAgent = "SELECT \"Sales-Agent-Name\" FROM PUB.\"sales-agent\" WHERE \"Sales-agent-id\" = " + "'" + dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "'";


            DataTable dtSalesAgent = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter salesAgent = new OdbcDataAdapter(headerSalesAgent, dbConn);
                salesAgent.Fill(dtSalesAgent);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {
                //sales agent name
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep;
                salesRep = cryrpt.ReportDefinition.ReportObjects["salesRep"] as TextObject;
                salesRep.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

                //sales rep ID for billing
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep2;
                salesRep2 = cryrpt.ReportDefinition.ReportObjects["salesRep2"] as TextObject;
                salesRep2.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString();// + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();
            }
            catch (Exception ex) { }
            //csr query and txt obj change
            String headerCSR = "SELECT \"CSR-Name\" FROM PUB.CSR WHERE \"CSR-ID\"=" + "'" + dtHeader.Rows[0]["CSR-ID"].ToString() + "'";

            DataTable dtCsr = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter csrAdap = new OdbcDataAdapter(headerCSR, dbConn);
                csrAdap.Fill(dtCsr);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {
                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = dtHeader.Rows[0]["CSR-ID"].ToString() + "-" + dtCsr.Rows[0]["CSR-Name"].ToString();
            }
            catch (Exception ex)
            {

                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = "";
            }
            #endregion Header


            #region 810 tag check - show MAILING
            //DataTable for all UI db-depenedant editing
            DataTable dtEdit = new DataTable();


            String query810Tag = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumber;

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810Tag = new OdbcDataAdapter(query810Tag, dbConn);
                adap810Tag.Fill(dtEdit);
            }
            catch (Exception ex)
            {


                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //here is needed for 810 chck
            bool check = false;

            foreach (DataRow dr in dtEdit.Rows)
            {
                //do nothing 
                if (dr["Work-Center-ID"].ToString().Contains("810"))
                {

                    check = true;
                }

            }//end foreach

            CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
            txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
            //no check to see if 810 tag is present
            if (!check)
            {
                txtObj.Border.LeftLineStyle = 0;
                txtObj.Border.RightLineStyle = 0;
                txtObj.Border.TopLineStyle = 0;
                txtObj.Border.BottomLineStyle = 0;
                txtObj.Text = "";


            }//end check for 810
            else
            {
                txtObj.Border.BorderColor = Color.LawnGreen;

            }//end check for 810
            #endregion 810 tag check - show MAILING

            //can use same data from above query and dataTable to get the the job status and description
            //ex) 50d status and it's description is ready to run on digital press
            #region tag status grab (no query, re-use data gathered from 810 check)

            try
            {
                //need to base of the 900 tag's -> tag status
                string tagStat = "";

                foreach (DataRow dr in dtEdit.Rows)
                {
                    //do nothing 
                    if (dr["Work-Center-ID"].ToString() == "900")
                    {

                        tagStat = dr["TagStatus-ID"].ToString();
                    }

                }//end foreach


                //grab first record's 'TagStatus-ID" and set status to it ex) 50d, 09, etc
                var statusObj = cryrpt.ReportDefinition.ReportObjects["status"] as TextObject;
                statusObj.Text = tagStat;

                var statusDescObj = cryrpt.ReportDefinition.ReportObjects["statusDesc"] as TextObject;

                if (tagStat == "01")
                {
                    statusDescObj.Text = "Outside Service";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }

                if (tagStat == "02")
                {
                    statusDescObj.Text = "On Hold";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "05")
                {
                    statusDescObj.Text = "Digital Mailing/Need Run Files";
                }
                if (tagStat == "07")
                {
                    statusDescObj.Text = "Long Term Project (In-House)";
                }
                if (tagStat == "08")
                {
                    statusDescObj.Text = "Long Term Project (out on proof)";
                }
                if (tagStat == "09")
                {
                    statusDescObj.Text = "In Pre Press Production";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "09-R")
                {
                    statusDescObj.Text = "Art corrections after proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "18")
                {
                    statusDescObj.Text = "Out on Random Proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "20")
                {
                    statusDescObj.Text = "Ready to Strip and Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "20p")
                {
                    statusDescObj.Text = "Approved-Waiting for Mock up";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "21")
                {
                    statusDescObj.Text = "Rerun-Pill Plates";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Ready to Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50")
                {
                    statusDescObj.Text = "Press-Running/Plated";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50d" || tagStat == "50D")
                {
                    statusDescObj.Text = "Ready to Run Digital Press";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50e" || tagStat == "50E")
                {
                    statusDescObj.Text = "Ready to Run Digital Press ENVELOPE";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "70")
                {
                    statusDescObj.Text = "Printed and in Bindery";
                    statusObj.Color = Color.Brown;
                    statusDescObj.Color = Color.Brown;
                }
                if (tagStat == "72")
                {
                    statusDescObj.Text = "Monthly DSF billing jobs";
                }
                if (tagStat == "75")
                {
                    statusDescObj.Text = "Waiting for Mailing Data";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "80")
                {
                    statusDescObj.Text = "Bindery Done-Ready for Mailing";
                    statusObj.Color = Color.Blue;
                    statusDescObj.Color = Color.Blue;
                }
                if (tagStat == "82")
                {
                    statusDescObj.Text = "Running on Netjet";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "85")
                {
                    statusDescObj.Text = "Mail Complete/Need Paperwork";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88")
                {
                    statusDescObj.Text = "Mail Complete - Ready to Mail";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88p")
                {
                    statusDescObj.Text = "PARTIAL mail/Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "90")
                {
                    statusDescObj.Text = "Job Completed-Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "92")
                {
                    statusDescObj.Text = "Being Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "95")
                {
                    statusDescObj.Text = "Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "97")
                {
                    statusDescObj.Text = "DSF Jobs To Be Billed";

                }
                if (tagStat == "97b" || tagStat == "97B")
                {
                    statusDescObj.Text = "DSF jobs Already Billed";
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Job Close Pending";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
            }//end try
            catch (Exception ex)
            {

                string error = ex + " : Tag Status update error check code";

                ErrorLog(error);

            }

            #endregion tag status

            #endregion UI


            //sub-reports section
            #region sub report creater

            //Press/Prepress sub rereports - Mailing Version, Mailing Free Feilds, Job Notes, Job Free Feilds, PO Req, PO line,
            //Forms, press, Stock

            #region Mailing Version subReport
            string queryMailVersion = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtMailVersion = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailVersion = new OdbcDataAdapter(queryMailVersion, dbConn);
                adapMailVersion.Fill(dtMailVersion);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing Version FF report";

                ErrorLog(error);
            }

            //sql query jobMailingVersion for {Version #, Description, Qty}
            string queryJobMailVersion = "SELECT \"Version-Number\", \"Version-Description\", \"Version-Qty\" FROM PUB.JobMailingVersion WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtJobMailVersion = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobMailVersion = new OdbcDataAdapter(queryJobMailVersion, dbConn);
                adapJobMailVersion.Fill(dtJobMailVersion);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Job Mailing Version FF report";

                ErrorLog(error);
            }
            //load each version header
            if (dtJobMailVersion.Rows.Count != 0)
            {
                cryrpt.Subreports["subJobMailVersion"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobMailVersion"].SetDataSource(dtJobMailVersion);

                if (dtJobMailVersion.Rows.Count > 1)
                {
                    //first one goo now delete row 0
                    dtJobMailVersion.Rows[0].Delete();
                    dtJobMailVersion.AcceptChanges();
                    cryrpt.Subreports["subJobMailVersion2"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subJobMailVersion2"].SetDataSource(dtJobMailVersion);
                }
                else
                {
                    string subJobMailVersion2 = "subJobMailVersion2";
                    HideSubs(cryrpt, subJobMailVersion2);
                }
            }
            else
            {

                string subJobMailVersion = "subJobMailVersion";
                HideSubs(cryrpt, subJobMailVersion);
                string subJobMailVersion2 = "subJobMailVersion2";
                HideSubs(cryrpt, subJobMailVersion2);

            }

            //and change second column name to match CR report value "FFvalue"
            dtMailVersion.Columns["Free-Field-Char"].ColumnName = "FFvalue";
            dtMailVersion.Columns.Add("FFname");

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailVersion.Rows.Count != 0)
            {

                //check how many bversions there are
                if (dtMailVersion.Rows.Count > 9)
                {

                    //process as 2 mailing versions

                    try
                    {
                        dtMailVersion.Rows[0]["FFname"] = "Permit Type";
                        dtMailVersion.Rows[1]["FFname"] = "Permit Owner";
                        dtMailVersion.Rows[2]["FFname"] = "Permit #/city & zip";
                        dtMailVersion.Rows[3]["FFname"] = "Postage Class";
                        dtMailVersion.Rows[4]["FFname"] = "Specialty Presort";
                        dtMailVersion.Rows[5]["FFname"] = "List Type";
                        dtMailVersion.Rows[6]["FFname"] = "Dedupe";
                        dtMailVersion.Rows[7]["FFname"] = "Surpression";
                        dtMailVersion.Rows[8]["FFname"] = "Move Update/NCOA";

                        dtMailVersion.Rows[9]["FFname"] = "Permit Type";
                        dtMailVersion.Rows[10]["FFname"] = "Permit Owner";
                        dtMailVersion.Rows[11]["FFname"] = "Permit #/city & zip";
                        dtMailVersion.Rows[12]["FFname"] = "Postage Class";
                        dtMailVersion.Rows[13]["FFname"] = "Specialty Presort";
                        dtMailVersion.Rows[14]["FFname"] = "List Type";
                        dtMailVersion.Rows[15]["FFname"] = "Dedupe";
                        dtMailVersion.Rows[16]["FFname"] = "Surpression";
                        dtMailVersion.Rows[17]["FFname"] = "Move Update/NCOA";
                    }
                    catch (Exception ex) { }

                    //copy DT and pop off first nine
                    DataTable dtMailV2 = dtMailVersion.Copy();

                    try
                    {
                        //first suppress from back 17-14
                        dtMailV2.Rows[17].Delete();
                        dtMailV2.Rows[16].Delete();
                        dtMailV2.Rows[15].Delete();
                        dtMailV2.Rows[14].Delete();
                        dtMailV2.Rows[0].Delete();
                        dtMailV2.Rows[1].Delete();
                        dtMailV2.Rows[2].Delete();
                        dtMailV2.Rows[3].Delete();
                        dtMailV2.Rows[4].Delete();
                        dtMailV2.Rows[5].Delete();
                        dtMailV2.Rows[6].Delete();
                        dtMailV2.Rows[7].Delete();
                        dtMailV2.Rows[8].Delete();
                        dtMailV2.AcceptChanges();

                        //remove last 9 from dtMailversion
                        dtMailVersion.Rows[5].Delete();
                        dtMailVersion.Rows[6].Delete();
                        dtMailVersion.Rows[7].Delete();
                        dtMailVersion.Rows[8].Delete();
                        dtMailVersion.Rows[9].Delete();
                        dtMailVersion.Rows[10].Delete();
                        dtMailVersion.Rows[11].Delete();
                        dtMailVersion.Rows[12].Delete();
                        dtMailVersion.Rows[13].Delete();
                        dtMailVersion.Rows[14].Delete();
                        dtMailVersion.Rows[15].Delete();
                        dtMailVersion.Rows[16].Delete();
                        dtMailVersion.Rows[17].Delete();
                        dtMailVersion.AcceptChanges();
                    }
                    catch (Exception ex) { }

                    //load the subReports with both tables dtJobMailingVersion and dtMailVersion
                    //cannot can only load one at a time, have to make the version info its own subrpt
                    cryrpt.Subreports["subMailVersion"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subMailVersion"].SetDataSource(dtMailVersion);


                    cryrpt.Subreports["subMailVersion2"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subMailVersion2"].SetDataSource(dtMailV2);



                }//end how many versions check (if >9)

                else
                {
                    try
                    {
                        dtMailVersion.Rows[0]["FFname"] = "Permit Type";
                        dtMailVersion.Rows[1]["FFname"] = "Permit Owner";
                        dtMailVersion.Rows[2]["FFname"] = "Permit #/city & zip";
                        dtMailVersion.Rows[3]["FFname"] = "Postage Class";
                        dtMailVersion.Rows[4]["FFname"] = "Specialty Presort";
                        dtMailVersion.Rows[5]["FFname"] = "List Type";
                        dtMailVersion.Rows[6]["FFname"] = "Dedupe";
                        dtMailVersion.Rows[7]["FFname"] = "Surpression";
                        dtMailVersion.Rows[8]["FFname"] = "Move Update/NCOA";
                    }
                    catch (Exception ex) { }

                    //first mail version subrpt
                    cryrpt.Subreports["subMailVersion"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subMailVersion"].SetDataSource(dtMailVersion);

                    string subMailVersionFF2 = "subMailVersion2";
                    HideSubs(cryrpt, subMailVersionFF2);
                }

            }//end intial check if no records then hide both sub reports
            else
            {

                string subMailVersionFF = "subMailVersion";
                HideSubs(cryrpt, subMailVersionFF);

                //hide second one as well
                string subMailVersionFF2 = "subMailVersion2";
                HideSubs(cryrpt, subMailVersionFF2);

            }//this else if no mailign versions found


            #endregion Mailing Version subReport

            #region Mailing Free Fields subReport
            string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtMailFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);
                adapMailFF.Fill(dtMailFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailFF.Columns["Free-Field-Char"].ColumnName = "FFval";
            dtMailFF.Columns.Add("FFname");

            try
            {
                dtMailFF.Rows[0]["FFname"] = "Mailpeice Size";
                dtMailFF.Rows[1]["FFname"] = "Customer Sign-Offs";
                dtMailFF.Rows[2]["FFname"] = "Inserting";
                dtMailFF.Rows[3]["FFname"] = "Tab/Seal/Glue";
                dtMailFF.Rows[4]["FFname"] = "Samples for Postage";
            }
            catch (Exception ex) { }
            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailFF.Rows.Count != 0)
            {
                cryrpt.Subreports["subMailFreeFields"].DataSourceConnections.Clear();
                cryrpt.Subreports["subMailFreeFields"].SetDataSource(dtMailFF);

            }
            else
            {

                string subMailFF = "subMailFreeFields";
                HideSubs(cryrpt, subMailFF);

            }

            #endregion Mailing Free Fields subReport

            #region Job Notes subReport
            string queryJobNotes = "SELECT \"SpecCategory-ID\", Description, \"Created-By\", \"Comment-Date\", \"Update-date\" FROM PUB.JobComments WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtJobNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobNotes = new OdbcDataAdapter(queryJobNotes, dbConn);
                adapJobNotes.Fill(dtJobNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Job Notes report";

                ErrorLog(error);
            }

            //change the names
            dtJobNotes.Columns["SpecCategory-ID"].ColumnName = "SpecID";
            dtJobNotes.Columns["Created-By"].ColumnName = "EnterBy";
            dtJobNotes.Columns["Comment-Date"].ColumnName = "DateEntered";
            dtJobNotes.Columns["Update-date"].ColumnName = "DateUpdated";


            for (int x = dtJobNotes.Rows.Count - 1; x >= 0; x--)
            {


                DataRow dr = dtJobNotes.Rows[x];


                if (dr["SpecID"].ToString() == "01" || dr["SpecID"].ToString() == "00" || dr["SpecID"].ToString() == "" || dr["SpecID"].ToString() == "05" || dr["SpecID"].ToString() == "80")
                {



                }
                else
                {
                    dr.Delete();
                }

                dtJobNotes.AcceptChanges();
            }

            dtJobNotes.AcceptChanges();

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtJobNotes.Rows.Count != 0)
            {
                cryrpt.Subreports["subJobNotes"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobNotes"].SetDataSource(dtJobNotes);

            }
            else
            {

                string subJobComments = "subJobNotes";
                HideSubs(cryrpt, subJobComments);

            }


            #endregion Job Notes subReports


            #region Job free fields subRPt
            string queryFF = "SELECT \"Free-Field-Char\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);
                adapFF.Fill(dtFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Free fields report";

                ErrorLog(error);
            }

            //here is where i need to convert decimal -> free-fieldchar
            string lastJob = dtFF.Rows[1]["Free-Field-Decimal"].ToString();
            dtFF.Rows[1]["Free-Field-Char"] = lastJob;

            dtFF.Columns.Remove("Free-Field-Decimal");

            dtFF.Columns.Add("FFname");
            dtFF.Rows[0]["FFname"] = "Customer To Supply";
            dtFF.Rows[1]["FFname"] = "Last Job#";
            dtFF.Rows[2]["FFname"] = "Type of Proof Needed";
            dtFF.Rows[3]["FFname"] = "Proof Needed By";
            dtFF.Rows[4]["FFname"] = "Coating";
            dtFF.Rows[5]["FFname"] = "Perf Score";
            dtFF.Rows[6]["FFname"] = "Certification";
            dtFF.Rows[7]["FFname"] = "Mailing Services";
            dtFF.Rows[8]["FFname"] = "Litho/Digital/DSF/ASI";
            dtFF.Rows[9]["FFname"] = "Samples";

            CrystalDecisions.CrystalReports.Engine.TextObject txtObjSample;
            txtObjSample = cryrpt.ReportDefinition.ReportObjects["txtSample"] as TextObject;

            if (dtFF.Rows[9]["Free-Field-Char"].ToString() != "None")
            {

                txtObjSample.Text = "";
                txtObjSample.Border.BorderColor = Color.Orange;
                txtObjSample.Text = dtFF.Rows[9]["Free-Field-Char"].ToString();
            }
            else
            {

                txtObjSample.Border.LeftLineStyle = 0;
                txtObjSample.Border.RightLineStyle = 0;
                txtObjSample.Border.TopLineStyle = 0;
                txtObjSample.Border.BottomLineStyle = 0;
                txtObjSample.Text = "";
            }

            //also check if empty exists it is empty hideSubs it
            if (dtFF.Rows.Count != 0)
            {


                if (dtFF.Rows[0]["Free-Field-Char"].ToString() == "DSF")
                {

                    CrystalDecisions.CrystalReports.Engine.TextObject txtDSF;
                    txtDSF = cryrpt.ReportDefinition.ReportObjects["txtDSF"] as TextObject;
                    txtDSF.Text = "DSF";

                }

                cryrpt.Subreports["subJobFreeFields"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobFreeFields"].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subJobFreeFields";
                HideSubs(cryrpt, subFF);

            }


            #endregion job free fields subRpt


            #region PO req subRpt
            string queryPOreq = "SELECT \"Req-Number\", \"Supplier-ID\", \"Supplier-Name\", \"Inventory-Item-ID\", \"Item-Desc\", \"Qty-Requisitioned\", \"Unit-Meas-Purchase\", \"Unit-Cost-Purchase\", \"Req-Status\" FROM PUB.PORequisition WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtPOreq = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOreq = new OdbcDataAdapter(queryPOreq, dbConn);
                adapPOreq.Fill(dtPOreq);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Req report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOreq.Rows.Count != 0)
            {
                cryrpt.Subreports["subPOReq"].DataSourceConnections.Clear();
                cryrpt.Subreports["subPOReq"].SetDataSource(dtPOreq);

            }
            else
            {

                string subPOreq = "subPOReq";
                HideSubs(cryrpt, subPOreq);

            }

            #endregion PO req subRpt


            #region PO line info subRpt
            string queryPOLine = "SELECT \"PO-Number\", \"Line-Status\", \"Date-PO\", \"Date-Last-Receipt\", \"Item-Desc\", \"Qty-Received-Purchase\" FROM PUB.POLine WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtPOLine = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOLine = new OdbcDataAdapter(queryPOLine, dbConn);
                adapPOLine.Fill(dtPOLine);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Line report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOLine.Rows.Count != 0)
            {
                cryrpt.Subreports["subPOLine"].DataSourceConnections.Clear();
                cryrpt.Subreports["subPOLine"].SetDataSource(dtPOLine);

            }
            else
            {

                string subPOLine = "subPOLine";
                HideSubs(cryrpt, subPOLine);

            }
            #endregion Po Line info Subrpt



            #region form spec subRpt
            string querySpecForm = "SELECT \"Form-ID\",  \"Form-Desc\", \"Number-Pages\", Width, Length, \"Number-forms\", \"Number-Out\", Imposition, Notes FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtSpecForm = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapSpecForm = new OdbcDataAdapter(querySpecForm, dbConn);
                adapSpecForm.Fill(dtSpecForm);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Spec Form report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtSpecForm.Rows.Count != 0)
            {
                cryrpt.Subreports["subFormSpecs"].DataSourceConnections.Clear();
                cryrpt.Subreports["subFormSpecs"].SetDataSource(dtSpecForm);

            }
            else
            {

                string subFormSpec = "subFormSpecs";
                HideSubs(cryrpt, subFormSpec);

            }


            #endregion form spec subRpt

            #region form Notes subrpt
            string queryFormNotes = "SELECT \"Form-ID\", Description FROM PUB.JobComments WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtFormNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFormNotes = new OdbcDataAdapter(queryFormNotes, dbConn);
                adapFormNotes.Fill(dtFormNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Form Notes report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtFormNotes.Rows.Count != 0)
            {
                cryrpt.Subreports["subFormNotes"].DataSourceConnections.Clear();
                cryrpt.Subreports["subFormNotes"].SetDataSource(dtFormNotes);

            }
            else
            {

                string subFormNotes = "subFormNotes";
                HideSubs(cryrpt, subFormNotes);

            }


            #endregion form notes subRpt

            #region press rpt
            string queryPress = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtPress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPress = new OdbcDataAdapter(queryPress, dbConn);
                adapPress.Fill(dtPress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Press report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPress.Rows.Count != 0)
            {
                cryrpt.Subreports["subPress"].DataSourceConnections.Clear();
                cryrpt.Subreports["subPress"].SetDataSource(dtPress);

            }
            else
            {

                string subPress = "subPress";
                HideSubs(cryrpt, subPress);

            }

            #endregion press rpt

            #region stock rpt
            string queryStock = "SELECT \"Form-ID\", \"Gross-Sheets\", \"Gross-Weight\",  Notes,  \"Inventory-Item-ID\", \"Inventory-Item-Desc\", \"Material-Type\", Length, Width FROM PUB.JobSpecMaterial WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtStock = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapStock = new OdbcDataAdapter(queryStock, dbConn);
                adapStock.Fill(dtStock);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report";

                ErrorLog(error);
            }

            //need to get page-grain new query
            string queryPageGrain = "SELECT \"Page-Grain\" FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtPG = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPG = new OdbcDataAdapter(queryPageGrain, dbConn);
                adapPG.Fill(dtPG);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report -PG";

                ErrorLog(error);
            }

            dtStock.Columns.Add("Page-Grain");

            try
            {
                string val = dtPG.Rows[0]["Page-Grain"].ToString();

                foreach (DataRow dr in dtStock.Rows)
                {

                    dr["Page-Grain"] = val;

                }
            }
            catch (Exception ex) { }

            //also check if empty exists it is empty hideSubs it
            if (dtStock.Rows.Count != 0)
            {
                cryrpt.Subreports["subStock"].DataSourceConnections.Clear();
                cryrpt.Subreports["subStock"].SetDataSource(dtStock);

            }
            else
            {

                string subStock = "subStock";
                HideSubs(cryrpt, subStock);

            }

            #endregion stock rpt


            #region estimateDetail

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;

            DataTable estimateIDTbl = new DataTable();
            DataTable estDetailLines = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId = new OdbcDataAdapter(estimateID, dbConn);
                adapEstId.Fill(estimateIDTbl);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl.Rows.Count == 0) { }

            else
            {

                string eID = estimateIDTbl.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines = new OdbcDataAdapter(estDetail, dbConn);
                    adapEstIdLines.Fill(estDetailLines);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines.Rows[i][1].ToString() == "" || estDetailLines.Rows[i][1].ToString() == "Main Component" || estDetailLines.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines.AcceptChanges();

                cryrpt.Subreports[4].DataSourceConnections.Clear();
                cryrpt.Subreports[4].SetDataSource(estDetailLines);

            }
            else
            {

                string subEstDetail = "subEstimateDetails";
                HideSubs(cryrpt, subEstDetail);

            }


            #endregion

            #region estimateDetail2

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID2 = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;

            DataTable estimateIDTbl2 = new DataTable();
            DataTable estDetailLines2 = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId2 = new OdbcDataAdapter(estimateID2, dbConn);
                adapEstId2.Fill(estimateIDTbl2);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl2.Rows.Count == 0) { }

            else
            {

                string eID2 = estimateIDTbl2.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail2 = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID2;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines2 = new OdbcDataAdapter(estDetail2, dbConn);
                    adapEstIdLines2.Fill(estDetailLines2);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines2.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines2.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines2.Rows[i][1].ToString() == "" || estDetailLines2.Rows[i][1].ToString() == "Main Component" || estDetailLines2.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines2.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines2.AcceptChanges();
                cryrpt.Subreports[5].DataSourceConnections.Clear();
                cryrpt.Subreports[5].SetDataSource(estDetailLines2);

            }
            else
            {

                string subEstDetail2 = "subEstimateDetails2";
                HideSubs(cryrpt, subEstDetail2);

            }


            #endregion

            #region estimateDetail3

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID3 = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;

            DataTable estimateIDTbl3 = new DataTable();
            DataTable estDetailLines3 = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId3 = new OdbcDataAdapter(estimateID3, dbConn);
                adapEstId3.Fill(estimateIDTbl3);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl3.Rows.Count == 0) { }

            else
            {

                string eID3 = estimateIDTbl3.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail3 = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID3;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines3 = new OdbcDataAdapter(estDetail3, dbConn);
                    adapEstIdLines3.Fill(estDetailLines3);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines3.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines3.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines3.Rows[i][1].ToString() == "" || estDetailLines3.Rows[i][1].ToString() == "Main Component" || estDetailLines3.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines3.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines3.AcceptChanges();

                cryrpt.Subreports[6].DataSourceConnections.Clear();
                cryrpt.Subreports[6].SetDataSource(estDetailLines3);

            }
            else
            {

                string subEstDetail3 = "subEstimateDetails3";
                HideSubs(cryrpt, subEstDetail3);

            }


            #endregion

            #endregion sub report creation


            dbConn.Close();
            #endregion DB close connection

            #region hideSubs
            string subBindery = "subBindery";
            HideSubs(cryrpt, subBindery);

            string subBinderyMatts = "subBinderyMatts";
            HideSubs(cryrpt, subBinderyMatts);

            string subPrepress = "subPrepress";
            HideSubs(cryrpt, subPrepress);

            string subAlt = "subAlterations";
            HideSubs(cryrpt, subAlt);

            string subShipTo = "subShipTo";
            HideSubs(cryrpt, subShipTo);

            string sub810Notes = "sub810Notes";
            HideSubs(cryrpt, sub810Notes);

            #endregion hideSubs

            #region display rpt

            //surpress billing section
            cryrpt.ReportDefinition.Sections["billingSection"].SectionFormat.EnableSuppress = true;


            CrystalReportViewer1.ReportSource = cryrpt;
            CrystalReportViewer1.RefreshReport();

            cryrpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "VVT Report");



            #endregion display rpt

        }//end press/prepress
        #endregion



        #region mailing ticket
        protected void mailTicketBttn_Click(object sender, EventArgs e)
        {

            string jobNumber = jobNumTextBox.Text;

            #region connection and CR object properties/settings
            //rerport object



            CrystalReport2 cryrpt = new CrystalReport2();

            cryrpt.DataSourceConnections.Clear();  //clear the connections (will popualte with fresh sql query defined data

            //set the databse login info, use twice first one is to login into sql server

            //getting write only error


            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");
            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this does not error out!?
            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //try our best to never have paramter panel pop-up, works like 95% of time
            //error here as well?
            //LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;
            #region UI on CR object editing (non-DB dependant)

            //change text object, tell user what ticket the are curentlly running/looking at
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Mailing Ticket";


            #endregion UI on CR object editing (non-DB dependant)

            #endregion crystal report config end


            //here is where i need to clean up lots, make functions, sql queryies, DT and DS manipualtion, pass the job number, etc 
            //main code for program functionallity
            #region DB connection 


            //here is where i iwll be connecting to DB's (all queries will need access to basically my connection properties globals)
            #region global connection properties

            //connection string for DB
            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //open th econnection and error check
            OdbcConnection dbConn = new OdbcConnection(connectStr);

            dbConn.ConnectionTimeout = 150;

            try
            {
                dbConn.Open();

            }
            catch (Exception ex)
            {

                string error = ex + " : DB error cannot connect";

                ErrorLog(error);

            }
            #endregion global connection propeties

            //here is where i can change the UI of the report based on database data
            //ex) show word nailing on report if job# has a 810 tag associated with it
            #region UI crystal report editing (DB dependant)


            #region Header

            //set the job numbers from iuser input
            CrystalDecisions.CrystalReports.Engine.TextObject jobNum1;
            jobNum1 = cryrpt.ReportDefinition.ReportObjects["jobNum"] as TextObject;
            jobNum1.Text = jobNumber;

            CrystalDecisions.CrystalReports.Engine.TextObject jobNum2;
            jobNum2 = cryrpt.ReportDefinition.ReportObjects["jobNum2"] as TextObject;
            jobNum2.Text = jobNumber;



            String headerJob = "SELECT \"Job-Desc\", \"Date-Promised\", \"Sales-Rep-ID\", \"CSR-ID\", \"" +
                "PO-Number\", \"Overs-Allowed\", \"Last-Estimate-ID\", \"Quantity-Ordered\", \"Contact-Name\", \"Date-Entered\", \"Cust-ID-Ordered-by\"" +
                " FROM PUB.JOB WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtHeader = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter header = new OdbcDataAdapter(headerJob, dbConn);
                header.Fill(dtHeader);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set all text objects to the data from datatable
            try
            {
                //job descriptions
                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc;
                jobDesc = cryrpt.ReportDefinition.ReportObjects["jobDesc"] as TextObject;
                jobDesc.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc2;
                jobDesc2 = cryrpt.ReportDefinition.ReportObjects["jobDesc2"] as TextObject;
                jobDesc2.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                //date promised
                CrystalDecisions.CrystalReports.Engine.TextObject dateProm;
                dateProm = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
                dateProm.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject dateProm2;
                dateProm2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
                dateProm2.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                //qty, format from "3400" -> "3,400"
                CrystalDecisions.CrystalReports.Engine.TextObject qty;
                qty = cryrpt.ReportDefinition.ReportObjects["qty"] as TextObject;
                int qtyFormat = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty.Text = String.Format("{0:N0}", qtyFormat);

                CrystalDecisions.CrystalReports.Engine.TextObject qty2;
                qty2 = cryrpt.ReportDefinition.ReportObjects["qty2"] as TextObject;
                int qtyFormat2 = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty2.Text = String.Format("{0:N0}", qtyFormat2);

                //end formatting qty

                //job contact
                CrystalDecisions.CrystalReports.Engine.TextObject jobContact;
                jobContact = cryrpt.ReportDefinition.ReportObjects["contactName"] as TextObject;
                jobContact.Text = dtHeader.Rows[0]["Contact-Name"].ToString();

                //job date entered
                CrystalDecisions.CrystalReports.Engine.TextObject jobDE;
                jobDE = cryrpt.ReportDefinition.ReportObjects["jobDE"] as TextObject;
                jobDE.Text = dtHeader.Rows[0]["Date-Entered"].ToString();

                //over allowed
                CrystalDecisions.CrystalReports.Engine.TextObject OA;
                OA = cryrpt.ReportDefinition.ReportObjects["jobOA"] as TextObject;
                OA.Text = dtHeader.Rows[0]["Overs-Allowed"].ToString();

                //po num
                CrystalDecisions.CrystalReports.Engine.TextObject PO;
                PO = cryrpt.ReportDefinition.ReportObjects["poNum"] as TextObject;
                PO.Text = dtHeader.Rows[0]["PO-Number"].ToString();

                //estimate
                string est = dtHeader.Rows[0]["Last-Estimate-ID"].ToString().Insert(6, "-");
                CrystalDecisions.CrystalReports.Engine.TextObject estimate;
                estimate = cryrpt.ReportDefinition.ReportObjects["estimate"] as TextObject;
                estimate.Text = est;
            }
            catch (Exception ex) { }
            //customer query and text objects
            String headerCust = "SELECT \"cust-name\", \"Address-1\", \"Address-2\", \"City\", \"" +
               "State\", \"Zip\", \"Phone\", \"Address-3\", \"Group-ID\" FROM PUB.cust WHERE \"Cust-code\" = " + dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();


            DataTable dtCust = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter cust = new OdbcDataAdapter(headerCust, dbConn);
                cust.Fill(dtCust);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {
                //set the Customer info text objects
                //No viso labels
                CrystalDecisions.CrystalReports.Engine.TextObject visoLbl;
                visoLbl = cryrpt.ReportDefinition.ReportObjects["Text37"] as TextObject;
                //if the customer group-ID contains "broker" then
                if (dtCust.Rows[0]["Group-ID"].ToString().Contains("broker") || dtCust.Rows[0]["Group-ID"].ToString().Contains("Broker"))
                {

                    visoLbl.Text = "BLIND SHIP";

                }


                //cust name
                CrystalDecisions.CrystalReports.Engine.TextObject custName;
                custName = cryrpt.ReportDefinition.ReportObjects["custName"] as TextObject;
                custName.Text = dtCust.Rows[0]["cust-name"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject custName2;
                custName2 = cryrpt.ReportDefinition.ReportObjects["custName2"] as TextObject;
                custName2.Text = dtCust.Rows[0]["cust-name"].ToString();

                //address -> add 1 and 2 and 3 combined
                CrystalDecisions.CrystalReports.Engine.TextObject custAdd;
                custAdd = cryrpt.ReportDefinition.ReportObjects["custAddress"] as TextObject;
                custAdd.Text = dtCust.Rows[0]["Address-1"].ToString() + " " + dtCust.Rows[0]["Address-2"].ToString() + " " + dtCust.Rows[0]["Address-3"].ToString();

                //city state zip customer
                CrystalDecisions.CrystalReports.Engine.TextObject custCSZ;
                custCSZ = cryrpt.ReportDefinition.ReportObjects["custCSZ"] as TextObject;
                custCSZ.Text = dtCust.Rows[0]["City"].ToString() + " " + dtCust.Rows[0]["State"].ToString() + " " + dtCust.Rows[0]["Zip"].ToString();

                //customerPhone
                CrystalDecisions.CrystalReports.Engine.TextObject custPhone;
                custPhone = cryrpt.ReportDefinition.ReportObjects["custPhone"] as TextObject;
                custPhone.Text = dtCust.Rows[0]["Phone"].ToString();

                //customer number
                CrystalDecisions.CrystalReports.Engine.TextObject custNum;
                custNum = cryrpt.ReportDefinition.ReportObjects["custNum"] as TextObject;
                custNum.Text = dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();

            }
            catch (Exception ex) { }

            //sales agent query and txt obj change
            //why this does not work i have no fkin clue, making stack overflwo see what the brians can thunk up
            String headerSalesAgent = "SELECT \"Sales-Agent-Name\" FROM PUB.\"sales-agent\" WHERE \"Sales-agent-id\" = " + "'" + dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "'";

            // String headerSalesAgent = "SELECT \"Sales-agent-id\" , \"Sales-Agent-Name\" FROM PUB.\"sales-agent\"";

            DataTable dtSalesAgent = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter salesAgent = new OdbcDataAdapter(headerSalesAgent, dbConn);
                salesAgent.Fill(dtSalesAgent);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }


            try
            {
                //sales agent name
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep;
                salesRep = cryrpt.ReportDefinition.ReportObjects["salesRep"] as TextObject;
                salesRep.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

                //sales rep ID for billing
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep2;
                salesRep2 = cryrpt.ReportDefinition.ReportObjects["salesRep2"] as TextObject;
                salesRep2.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString();// + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();
            }
            catch (Exception ex) { }

            //csr query and txt obj change
            String headerCSR = "SELECT \"CSR-Name\" FROM PUB.CSR WHERE \"CSR-ID\"=" + "'" + dtHeader.Rows[0]["CSR-ID"].ToString() + "'";

            DataTable dtCsr = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter csrAdap = new OdbcDataAdapter(headerCSR, dbConn);
                csrAdap.Fill(dtCsr);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {
                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = dtHeader.Rows[0]["CSR-ID"].ToString() + "-" + dtCsr.Rows[0]["CSR-Name"].ToString();
            }
            catch (Exception ex)
            { //can be blank to catch that error and set to blank
                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = "";
            }
            #endregion Header


            #region 810 tag check - show MAILING
            //DataTable for all UI db-depenedant editing
            DataTable dtEdit = new DataTable();


            String query810Tag = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumber;

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810Tag = new OdbcDataAdapter(query810Tag, dbConn);
                adap810Tag.Fill(dtEdit);
            }
            catch (Exception ex)
            {


                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //here is needed for 810 chck
            bool check = false;

            foreach (DataRow dr in dtEdit.Rows)
            {
                //do nothing 
                if (dr["Work-Center-ID"].ToString().Contains("810"))
                {

                    check = true;
                }

            }//end foreach

            CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
            txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
            //no check to see if 810 tag is present
            if (!check)
            {
                txtObj.Border.LeftLineStyle = 0;
                txtObj.Border.RightLineStyle = 0;
                txtObj.Border.TopLineStyle = 0;
                txtObj.Border.BottomLineStyle = 0;
                txtObj.Text = "";


            }//end check for 810
            else
            {
                txtObj.Border.BorderColor = Color.LawnGreen;
            }
            #endregion 810 tag check - show MAILING

            //can use same data from above query and dataTable to get the the job status and description
            //ex) 50d status and it's description is ready to run on digital press
            #region tag status grab (no query, re-use data gathered from 810 check)

            try
            {
                //need to base of the 900 tag's -> tag status
                string tagStat = "";

                foreach (DataRow dr in dtEdit.Rows)
                {
                    //do nothing 
                    if (dr["Work-Center-ID"].ToString() == "900")
                    {

                        tagStat = dr["TagStatus-ID"].ToString();
                    }

                }//end foreach


                //grab first record's 'TagStatus-ID" and set status to it ex) 50d, 09, etc
                var statusObj = cryrpt.ReportDefinition.ReportObjects["status"] as TextObject;
                statusObj.Text = tagStat;

                var statusDescObj = cryrpt.ReportDefinition.ReportObjects["statusDesc"] as TextObject;

                if (tagStat == "01")
                {
                    statusDescObj.Text = "Outside Service";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }

                if (tagStat == "02")
                {
                    statusDescObj.Text = "On Hold";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "05")
                {
                    statusDescObj.Text = "Digital Mailing/Need Run Files";
                }
                if (tagStat == "07")
                {
                    statusDescObj.Text = "Long Term Project (In-House)";
                }
                if (tagStat == "08")
                {
                    statusDescObj.Text = "Long Term Project (out on proof)";
                }
                if (tagStat == "09")
                {
                    statusDescObj.Text = "In Pre Press Production";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "09-R")
                {
                    statusDescObj.Text = "Art corrections after proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "18")
                {
                    statusDescObj.Text = "Out on Random Proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "20")
                {
                    statusDescObj.Text = "Ready to Strip and Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "20p")
                {
                    statusDescObj.Text = "Approved-Waiting for Mock up";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "21")
                {
                    statusDescObj.Text = "Rerun-Pill Plates";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Ready to Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50")
                {
                    statusDescObj.Text = "Press-Running/Plated";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50d" || tagStat == "50D")
                {
                    statusDescObj.Text = "Ready to Run Digital Press";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50e" || tagStat == "50E")
                {
                    statusDescObj.Text = "Ready to Run Digital Press ENVELOPE";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "70")
                {
                    statusDescObj.Text = "Printed and in Bindery";
                    statusObj.Color = Color.Brown;
                    statusDescObj.Color = Color.Brown;
                }
                if (tagStat == "72")
                {
                    statusDescObj.Text = "Monthly DSF billing jobs";
                }
                if (tagStat == "75")
                {
                    statusDescObj.Text = "Waiting for Mailing Data";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "80")
                {
                    statusDescObj.Text = "Bindery Done-Ready for Mailing";
                    statusObj.Color = Color.Blue;
                    statusDescObj.Color = Color.Blue;
                }
                if (tagStat == "82")
                {
                    statusDescObj.Text = "Running on Netjet";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "85")
                {
                    statusDescObj.Text = "Mail Complete/Need Paperwork";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88")
                {
                    statusDescObj.Text = "Mail Complete - Ready to Mail";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88p")
                {
                    statusDescObj.Text = "PARTIAL mail/Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "90")
                {
                    statusDescObj.Text = "Job Completed-Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "92")
                {
                    statusDescObj.Text = "Being Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "95")
                {
                    statusDescObj.Text = "Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "97")
                {
                    statusDescObj.Text = "DSF Jobs To Be Billed";

                }
                if (tagStat == "97b" || tagStat == "97B")
                {
                    statusDescObj.Text = "DSF jobs Already Billed";
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Job Close Pending";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
            }//end try
            catch (Exception ex)
            {

                string error = ex + " : Tag Status update error check code";

                ErrorLog(error);

            }

            #endregion tag status

            #endregion UI


            //sub-reports section
            #region sub report creater

            #region Mailing Version subReport
            string queryMailVersion = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtMailVersion = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailVersion = new OdbcDataAdapter(queryMailVersion, dbConn);
                adapMailVersion.Fill(dtMailVersion);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing Version FF report";

                ErrorLog(error);
            }

            //sql query jobMailingVersion for {Version #, Description, Qty}
            string queryJobMailVersion = "SELECT \"Version-Number\", \"Version-Description\", \"Version-Qty\" FROM PUB.JobMailingVersion WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtJobMailVersion = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobMailVersion = new OdbcDataAdapter(queryJobMailVersion, dbConn);
                adapJobMailVersion.Fill(dtJobMailVersion);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Job Mailing Version FF report";

                ErrorLog(error);
            }
            //load each version header
            if (dtJobMailVersion.Rows.Count != 0)
            {
                cryrpt.Subreports["subJobMailVersion"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobMailVersion"].SetDataSource(dtJobMailVersion);

                if (dtJobMailVersion.Rows.Count > 1)
                {
                    //first one goo now delete row 0
                    dtJobMailVersion.Rows[0].Delete();
                    dtJobMailVersion.AcceptChanges();
                    cryrpt.Subreports["subJobMailVersion2"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subJobMailVersion2"].SetDataSource(dtJobMailVersion);
                }
                else
                {
                    string subJobMailVersion2 = "subJobMailVersion2";
                    HideSubs(cryrpt, subJobMailVersion2);
                }
            }
            else
            {

                string subJobMailVersion = "subJobMailVersion";
                HideSubs(cryrpt, subJobMailVersion);
                string subJobMailVersion2 = "subJobMailVersion2";
                HideSubs(cryrpt, subJobMailVersion2);

            }

            //and change second column name to match CR report value "FFvalue"
            dtMailVersion.Columns["Free-Field-Char"].ColumnName = "FFvalue";
            dtMailVersion.Columns.Add("FFname");

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailVersion.Rows.Count != 0)
            {

                //check how many bversions there are
                if (dtMailVersion.Rows.Count > 9)
                {

                    //process as 2 mailing versions

                    try
                    {
                        dtMailVersion.Rows[0]["FFname"] = "Permit Type";
                        dtMailVersion.Rows[1]["FFname"] = "Permit Owner";
                        dtMailVersion.Rows[2]["FFname"] = "Permit #/city & zip";
                        dtMailVersion.Rows[3]["FFname"] = "Postage Class";
                        dtMailVersion.Rows[4]["FFname"] = "Specialty Presort";
                        dtMailVersion.Rows[5]["FFname"] = "List Type";
                        dtMailVersion.Rows[6]["FFname"] = "Dedupe";
                        dtMailVersion.Rows[7]["FFname"] = "Surpression";
                        dtMailVersion.Rows[8]["FFname"] = "Move Update/NCOA";

                        dtMailVersion.Rows[9]["FFname"] = "Permit Type";
                        dtMailVersion.Rows[10]["FFname"] = "Permit Owner";
                        dtMailVersion.Rows[11]["FFname"] = "Permit #/city & zip";
                        dtMailVersion.Rows[12]["FFname"] = "Postage Class";
                        dtMailVersion.Rows[13]["FFname"] = "Specialty Presort";
                        dtMailVersion.Rows[14]["FFname"] = "List Type";
                        dtMailVersion.Rows[15]["FFname"] = "Dedupe";
                        dtMailVersion.Rows[16]["FFname"] = "Surpression";
                        dtMailVersion.Rows[17]["FFname"] = "Move Update/NCOA";
                    }
                    catch (Exception ex) { }

                    //copy DT and pop off first nine
                    DataTable dtMailV2 = dtMailVersion.Copy();

                    try
                    {
                        //first suppress from back 17-14
                        dtMailV2.Rows[17].Delete();
                        dtMailV2.Rows[16].Delete();
                        dtMailV2.Rows[15].Delete();
                        dtMailV2.Rows[14].Delete();
                        dtMailV2.Rows[0].Delete();
                        dtMailV2.Rows[1].Delete();
                        dtMailV2.Rows[2].Delete();
                        dtMailV2.Rows[3].Delete();
                        dtMailV2.Rows[4].Delete();
                        dtMailV2.Rows[5].Delete();
                        dtMailV2.Rows[6].Delete();
                        dtMailV2.Rows[7].Delete();
                        dtMailV2.Rows[8].Delete();
                        dtMailV2.AcceptChanges();

                        //remove last 9 from dtMailversion
                        dtMailVersion.Rows[5].Delete();
                        dtMailVersion.Rows[6].Delete();
                        dtMailVersion.Rows[7].Delete();
                        dtMailVersion.Rows[8].Delete();
                        dtMailVersion.Rows[9].Delete();
                        dtMailVersion.Rows[10].Delete();
                        dtMailVersion.Rows[11].Delete();
                        dtMailVersion.Rows[12].Delete();
                        dtMailVersion.Rows[13].Delete();
                        dtMailVersion.Rows[14].Delete();
                        dtMailVersion.Rows[15].Delete();
                        dtMailVersion.Rows[16].Delete();
                        dtMailVersion.Rows[17].Delete();
                        dtMailVersion.AcceptChanges();
                    }
                    catch (Exception ex) { }

                    //load the subReports with both tables dtJobMailingVersion and dtMailVersion
                    //cannot can only load one at a time, have to make the version info its own subrpt
                    cryrpt.Subreports["subMailVersion"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subMailVersion"].SetDataSource(dtMailVersion);


                    cryrpt.Subreports["subMailVersion2"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subMailVersion2"].SetDataSource(dtMailV2);



                }//end how many versions check (if >9)

                else
                {
                    try
                    {
                        dtMailVersion.Rows[0]["FFname"] = "Permit Type";
                        dtMailVersion.Rows[1]["FFname"] = "Permit Owner";
                        dtMailVersion.Rows[2]["FFname"] = "Permit #/city & zip";
                        dtMailVersion.Rows[3]["FFname"] = "Postage Class";
                        dtMailVersion.Rows[4]["FFname"] = "Specialty Presort";
                        dtMailVersion.Rows[5]["FFname"] = "List Type";
                        dtMailVersion.Rows[6]["FFname"] = "Dedupe";
                        dtMailVersion.Rows[7]["FFname"] = "Surpression";
                        dtMailVersion.Rows[8]["FFname"] = "Move Update/NCOA";
                    }
                    catch (Exception ex) { }

                    //first mail version subrpt
                    cryrpt.Subreports["subMailVersion"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subMailVersion"].SetDataSource(dtMailVersion);

                    string subMailVersionFF2 = "subMailVersion2";
                    HideSubs(cryrpt, subMailVersionFF2);
                }

            }//end intial check if no records then hide both sub reports
            else
            {

                string subMailVersionFF = "subMailVersion";
                HideSubs(cryrpt, subMailVersionFF);

                //hide second one as well
                string subMailVersionFF2 = "subMailVersion2";
                HideSubs(cryrpt, subMailVersionFF2);

            }//this else if no mailign versions found


            #endregion Mailing Version subReport

            #region Mailing Free Fields subReport
            string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtMailFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);
                adapMailFF.Fill(dtMailFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailFF.Columns["Free-Field-Char"].ColumnName = "FFval";
            dtMailFF.Columns.Add("FFname");

            try
            {
                dtMailFF.Rows[0]["FFname"] = "Mailpeice Size";
                dtMailFF.Rows[1]["FFname"] = "Customer Sign-Offs";
                dtMailFF.Rows[2]["FFname"] = "Inserting";
                dtMailFF.Rows[3]["FFname"] = "Tab/Seal/Glue";
                dtMailFF.Rows[4]["FFname"] = "Samples for Postage";
            }
            catch (Exception ex) { }

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailFF.Rows.Count != 0)
            {


                dtMailFF.Rows[4].Delete();
                dtMailFF.AcceptChanges();

                cryrpt.Subreports["subMailFreeFields"].DataSourceConnections.Clear();
                cryrpt.Subreports["subMailFreeFields"].SetDataSource(dtMailFF);

            }
            else
            {

                string subMailFF = "subMailFreeFields";
                HideSubs(cryrpt, subMailFF);

            }

            #endregion Mailing Free Fields subReport

            #region Job Notes subReport
            string queryJobNotes = "SELECT \"SpecCategory-ID\", Description, \"Created-By\", \"Comment-Date\", \"Update-date\" FROM PUB.JobComments WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtJobNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobNotes = new OdbcDataAdapter(queryJobNotes, dbConn);
                adapJobNotes.Fill(dtJobNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Job Notes report";

                ErrorLog(error);
            }

            //change the names
            dtJobNotes.Columns["SpecCategory-ID"].ColumnName = "SpecID";
            dtJobNotes.Columns["Created-By"].ColumnName = "EnterBy";
            dtJobNotes.Columns["Comment-Date"].ColumnName = "DateEntered";
            dtJobNotes.Columns["Update-date"].ColumnName = "DateUpdated";


            for (int x = dtJobNotes.Rows.Count - 1; x >= 0; x--)
            {


                DataRow dr = dtJobNotes.Rows[x];


                if (dr["SpecID"].ToString() == "03" || dr["SpecID"].ToString() == "00" || dr["SpecID"].ToString() == "")
                {



                }
                else
                {
                    dr.Delete();
                }

                dtJobNotes.AcceptChanges();
            }

            dtJobNotes.AcceptChanges();

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtJobNotes.Rows.Count != 0)
            {
                cryrpt.Subreports["subJobNotes"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobNotes"].SetDataSource(dtJobNotes);

            }
            else
            {

                string subJobComments = "subJobNotes";
                HideSubs(cryrpt, subJobComments);

            }


            #endregion Job Notes subReports

            #region Job free fields subRPt
            string queryFF = "SELECT \"Free-Field-Char\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);
                adapFF.Fill(dtFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Free fields report";

                ErrorLog(error);
            }

            //here is where i need to convert decimal -> free-fieldchar
            string lastJob = dtFF.Rows[1]["Free-Field-Decimal"].ToString();
            dtFF.Rows[1]["Free-Field-Char"] = lastJob;

            try
            {
                dtFF.Columns.Remove("Free-Field-Decimal");
                dtFF.Columns.Add("FFname");
                dtFF.Rows[0]["FFname"] = "Customer To Supply";
                dtFF.Rows[1]["FFname"] = "Last Job#";
                dtFF.Rows[2]["FFname"] = "Type of Proof Needed";
                dtFF.Rows[3]["FFname"] = "Proof Needed By";
                dtFF.Rows[4]["FFname"] = "Coating";
                dtFF.Rows[5]["FFname"] = "Perf Score";
                dtFF.Rows[6]["FFname"] = "Certification";
                dtFF.Rows[7]["FFname"] = "Mailing Services";
                dtFF.Rows[8]["FFname"] = "Litho/Digital/DSF/ASI";
                dtFF.Rows[9]["FFname"] = "Samples";

                CrystalDecisions.CrystalReports.Engine.TextObject txtObjSample;
                txtObjSample = cryrpt.ReportDefinition.ReportObjects["txtSample"] as TextObject;

                if (dtFF.Rows[9]["Free-Field-Char"].ToString() != "None")
                {
                    txtObjSample.Text = "";
                    txtObjSample.Border.BorderColor = Color.Orange;
                    txtObjSample.Text = dtFF.Rows[9]["Free-Field-Char"].ToString();
                }
                else
                {

                    txtObjSample.Border.LeftLineStyle = 0;
                    txtObjSample.Border.RightLineStyle = 0;
                    txtObjSample.Border.TopLineStyle = 0;
                    txtObjSample.Border.BottomLineStyle = 0;
                    txtObjSample.Text = "";
                }
            }
            catch (Exception ex) { }

            //also check if empty exists it is empty hideSubs it
            if (dtFF.Rows.Count != 0)
            {

                dtFF.Rows.RemoveAt(6);
                dtFF.Rows.RemoveAt(4);
                dtFF.Rows.RemoveAt(3);
                dtFF.Rows.RemoveAt(2);
                dtFF.Rows.RemoveAt(0);
                dtFF.AcceptChanges();

                //will have to do same process as adding field to the dataSet as in Mail Version
                //also need to flag DSF jobs 

                if (dtFF.Rows[0]["Free-Field-Char"].ToString() == "DSF")
                {

                    CrystalDecisions.CrystalReports.Engine.TextObject txtDSF;
                    txtDSF = cryrpt.ReportDefinition.ReportObjects["txtDSF"] as TextObject;
                    txtDSF.Text = "DSF";

                }

                cryrpt.Subreports["subJobFreeFields"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobFreeFields"].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subJobFreeFields";
                HideSubs(cryrpt, subFF);

            }


            #endregion job free fields subRpt

            #region 810 notes
            string query810Notes = "SELECT \"Work-Center-ID\", Notes FROM PUB.ScheduleByJob WHERE \"Job-ID\" = " + jobNumber;

            DataTable dt810Notes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810 = new OdbcDataAdapter(query810Notes, dbConn);
                adap810.Fill(dt810Notes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - 810 notes report";

                ErrorLog(error);
            }

            //filter out all work center id's except 810
            for (int i = dt810Notes.Rows.Count - 1; i >= 0; i--)
            {

                DataRow dr = dt810Notes.Rows[i];
                if (!dr["Work-Center-ID"].ToString().Contains("810"))
                {
                    dr.Delete();
                }

            }
            dt810Notes.AcceptChanges();

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dt810Notes.Rows.Count != 0)
            {

                cryrpt.Subreports["sub810Notes"].DataSourceConnections.Clear();
                cryrpt.Subreports["sub810Notes"].SetDataSource(dt810Notes);

            }
            else
            {

                string sub810Notes = "sub810Notes";
                HideSubs(cryrpt, sub810Notes);

            }
            #endregion 810 notes



            #endregion sub report creation


            dbConn.Close();
            #endregion DB close connection

            #region hide subs
            //mailing should have:
            // subMailVersion, subMailFF, subJobNotes,subJobFreeFields,sub810Notes

            string subBindery = "subBindery";
            HideSubs(cryrpt, subBindery);

            string subBinderyMatts = "subBinderyMatts";
            HideSubs(cryrpt, subBinderyMatts);

            string subStock = "subStock";
            HideSubs(cryrpt, subStock);

            string subPress = "subPress";
            HideSubs(cryrpt, subPress);

            string subPrepress = "subPrepress";
            HideSubs(cryrpt, subPrepress);

            string subFormNotes = "subFormNotes";
            HideSubs(cryrpt, subFormNotes);

            string subFormSpec = "subFormSpecs";
            HideSubs(cryrpt, subFormSpec);

            string subShipTo = "subShipTo";
            HideSubs(cryrpt, subShipTo);

            string subPOLine = "subPOLine";
            HideSubs(cryrpt, subPOLine);

            string subPOreq = "subPOReq";
            HideSubs(cryrpt, subPOreq);

            string subAlt = "subAlterations";
            HideSubs(cryrpt, subAlt);

            string subEstDetail = "subEstimateDetails";
            HideSubs(cryrpt, subEstDetail);

            string subEstDetail2 = "subEstimateDetails2";
            HideSubs(cryrpt, subEstDetail2);
            string subEstDetail3 = "subEstimateDetails3";
            HideSubs(cryrpt, subEstDetail3);

            #endregion hideSubs


            #region display rpt

            //surpress billing section
            cryrpt.ReportDefinition.Sections["billingSection"].SectionFormat.EnableSuppress = true;

            CrystalReportViewer1.ReportSource = cryrpt;
            CrystalReportViewer1.RefreshReport();

            cryrpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "VVT Report");




        }//end mail ticket button click
        #endregion mail ticket
        #endregion



        #region shipping ticket
        protected void shipTicketBttn_Click(object sender, EventArgs e)
        {
            string jobNumber = jobNumTextBox.Text;

            #region connection and CR object properties/settings
            //rerport object



            CrystalReport2 cryrpt = new CrystalReport2();

            cryrpt.DataSourceConnections.Clear();  //clear the connections (will popualte with fresh sql query defined data

            //set the databse login info, use twice first one is to login into sql server

            //getting write only error


            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");
            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this does not error out!?
            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //try our best to never have paramter panel pop-up, works like 95% of time
            //error here as well?
            //LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;

            #region UI on CR object editing (non-DB dependant)

            //change text object, tell user what ticket the are curentlly running/looking at
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Bindery/Shipping Ticket";


            #endregion UI on CR object editing (non-DB dependant)

            #endregion crystal report config end




            //here is where i need to clean up lots, make functions, sql queryies, DT and DS manipualtion, pass the job number, etc 
            //main code for program functionallity
            #region DB connection 


            //here is where i iwll be connecting to DB's (all queries will need access to basically my connection properties globals)
            #region global connection properties

            //connection string for DB
            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //open th econnection and error check
            OdbcConnection dbConn = new OdbcConnection(connectStr);
            dbConn.ConnectionTimeout = 150;
            try
            {
                dbConn.Open();
            }
            catch (Exception ex)
            {

                string error = ex + " : DB error cannot connect";

                ErrorLog(error);

            }
            #endregion global connection propeties




            //here is where i can change the UI of the report based on database data
            //ex) show word nailing on report if job# has a 810 tag associated with it
            #region UI crystal report editing (DB dependant)


            #region Header

            //set the job numbers from iuser input
            CrystalDecisions.CrystalReports.Engine.TextObject jobNum1;
            jobNum1 = cryrpt.ReportDefinition.ReportObjects["jobNum"] as TextObject;
            jobNum1.Text = jobNumber;

            CrystalDecisions.CrystalReports.Engine.TextObject jobNum2;
            jobNum2 = cryrpt.ReportDefinition.ReportObjects["jobNum2"] as TextObject;
            jobNum2.Text = jobNumber;



            String headerJob = "SELECT \"Job-Desc\", \"Date-Promised\", \"Sales-Rep-ID\", \"CSR-ID\", \"" +
                "PO-Number\", \"Overs-Allowed\", \"Last-Estimate-ID\", \"Quantity-Ordered\", \"Contact-Name\", \"Date-Entered\", \"Cust-ID-Ordered-by\"" +
                " FROM PUB.JOB WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtHeader = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter header = new OdbcDataAdapter(headerJob, dbConn);
                header.Fill(dtHeader);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set all text objects to the data from datatable

            try
            {
                //job descriptions
                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc;
                jobDesc = cryrpt.ReportDefinition.ReportObjects["jobDesc"] as TextObject;
                jobDesc.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc2;
                jobDesc2 = cryrpt.ReportDefinition.ReportObjects["jobDesc2"] as TextObject;
                jobDesc2.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                //date promised
                CrystalDecisions.CrystalReports.Engine.TextObject dateProm;
                dateProm = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
                dateProm.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject dateProm2;
                dateProm2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
                dateProm2.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                //qty, format from "3400" -> "3,400"
                CrystalDecisions.CrystalReports.Engine.TextObject qty;
                qty = cryrpt.ReportDefinition.ReportObjects["qty"] as TextObject;
                int qtyFormat = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty.Text = String.Format("{0:N0}", qtyFormat);

                CrystalDecisions.CrystalReports.Engine.TextObject qty2;
                qty2 = cryrpt.ReportDefinition.ReportObjects["qty2"] as TextObject;
                int qtyFormat2 = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty2.Text = String.Format("{0:N0}", qtyFormat2);

                //end formatting qty

                //job contact
                CrystalDecisions.CrystalReports.Engine.TextObject jobContact;
                jobContact = cryrpt.ReportDefinition.ReportObjects["contactName"] as TextObject;
                jobContact.Text = dtHeader.Rows[0]["Contact-Name"].ToString();

                //job date entered
                CrystalDecisions.CrystalReports.Engine.TextObject jobDE;
                jobDE = cryrpt.ReportDefinition.ReportObjects["jobDE"] as TextObject;
                jobDE.Text = dtHeader.Rows[0]["Date-Entered"].ToString();

                //over allowed
                CrystalDecisions.CrystalReports.Engine.TextObject OA;
                OA = cryrpt.ReportDefinition.ReportObjects["jobOA"] as TextObject;
                OA.Text = dtHeader.Rows[0]["Overs-Allowed"].ToString();

                //po num
                CrystalDecisions.CrystalReports.Engine.TextObject PO;
                PO = cryrpt.ReportDefinition.ReportObjects["poNum"] as TextObject;
                PO.Text = dtHeader.Rows[0]["PO-Number"].ToString();

                //estimate
                string est = dtHeader.Rows[0]["Last-Estimate-ID"].ToString().Insert(6, "-");
                CrystalDecisions.CrystalReports.Engine.TextObject estimate;
                estimate = cryrpt.ReportDefinition.ReportObjects["estimate"] as TextObject;
                estimate.Text = est;
            }
            catch (Exception ex) { }
            //customer query and text objects
            //customer query and text objects
            String headerCust = "SELECT \"cust-name\", \"Address-1\", \"Address-2\", \"City\", \"" +
               "State\", \"Zip\", \"Phone\", \"Address-3\", \"Group-ID\"FROM PUB.cust WHERE \"Cust-code\" = " + dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();



            DataTable dtCust = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter cust = new OdbcDataAdapter(headerCust, dbConn);
                cust.Fill(dtCust);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set the Customer info text objects
            //cust name
            try
            {

                //No viso labels
                CrystalDecisions.CrystalReports.Engine.TextObject visoLbl;
                visoLbl = cryrpt.ReportDefinition.ReportObjects["Text37"] as TextObject;
                //if the customer group-ID contains "broker" then
                if (dtCust.Rows[0]["Group-ID"].ToString().Contains("broker") || dtCust.Rows[0]["Group-ID"].ToString().Contains("Broker"))
                {

                    visoLbl.Text = "BLIND SHIP";

                }

                CrystalDecisions.CrystalReports.Engine.TextObject custName;
                custName = cryrpt.ReportDefinition.ReportObjects["custName"] as TextObject;
                custName.Text = dtCust.Rows[0]["cust-name"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject custName2;
                custName2 = cryrpt.ReportDefinition.ReportObjects["custName2"] as TextObject;
                custName2.Text = dtCust.Rows[0]["cust-name"].ToString();

                //address -> add 1 and 2 and 3 combined
                CrystalDecisions.CrystalReports.Engine.TextObject custAdd;
                custAdd = cryrpt.ReportDefinition.ReportObjects["custAddress"] as TextObject;
                custAdd.Text = dtCust.Rows[0]["Address-1"].ToString() + " " + dtCust.Rows[0]["Address-2"].ToString() + " " + dtCust.Rows[0]["Address-3"].ToString();

                //city state zip customer
                CrystalDecisions.CrystalReports.Engine.TextObject custCSZ;
                custCSZ = cryrpt.ReportDefinition.ReportObjects["custCSZ"] as TextObject;
                custCSZ.Text = dtCust.Rows[0]["City"].ToString() + " " + dtCust.Rows[0]["State"].ToString() + " " + dtCust.Rows[0]["Zip"].ToString();

                //customerPhone
                CrystalDecisions.CrystalReports.Engine.TextObject custPhone;
                custPhone = cryrpt.ReportDefinition.ReportObjects["custPhone"] as TextObject;
                custPhone.Text = dtCust.Rows[0]["Phone"].ToString();

                //customer number
                CrystalDecisions.CrystalReports.Engine.TextObject custNum;
                custNum = cryrpt.ReportDefinition.ReportObjects["custNum"] as TextObject;
                custNum.Text = dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();

            }
            catch (Exception ex) { }
            //sales agent query and txt obj change
            //why this does not work i have no fkin clue, making stack overflwo see what the brians can thunk up
            String headerSalesAgent = "SELECT \"Sales-Agent-Name\" FROM PUB.\"sales-agent\" WHERE \"Sales-agent-id\" = " + "'" + dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "'";

            DataTable dtSalesAgent = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter salesAgent = new OdbcDataAdapter(headerSalesAgent, dbConn);
                salesAgent.Fill(dtSalesAgent);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {
                //sales agent name
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep;
                salesRep = cryrpt.ReportDefinition.ReportObjects["salesRep"] as TextObject;
                salesRep.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

                //sales rep ID for billing
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep2;
                salesRep2 = cryrpt.ReportDefinition.ReportObjects["salesRep2"] as TextObject;
                salesRep2.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString();// + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();
            }
            catch (Exception ex) { }
            //csr query and txt obj change
            String headerCSR = "SELECT \"CSR-Name\" FROM PUB.CSR WHERE \"CSR-ID\"=" + "'" + dtHeader.Rows[0]["CSR-ID"].ToString() + "'";

            DataTable dtCsr = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter csrAdap = new OdbcDataAdapter(headerCSR, dbConn);
                csrAdap.Fill(dtCsr);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {
                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = dtHeader.Rows[0]["CSR-ID"].ToString() + "-" + dtCsr.Rows[0]["CSR-Name"].ToString();
            }
            catch (Exception ex)
            {

                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = "";

            }
            #endregion Header


            #region 810 tag check - show MAILING
            //DataTable for all UI db-depenedant editing
            DataTable dtEdit = new DataTable();


            String query810Tag = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumber;

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810Tag = new OdbcDataAdapter(query810Tag, dbConn);
                adap810Tag.Fill(dtEdit);
            }
            catch (Exception ex)
            {


                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //here is needed for 810 chck
            bool check = false;

            foreach (DataRow dr in dtEdit.Rows)
            {
                //do nothing 
                if (dr["Work-Center-ID"].ToString().Contains("810"))
                {

                    check = true;
                }

            }//end foreach

            CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
            txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
            //no check to see if 810 tag is present
            if (!check)
            {
                txtObj.Border.LeftLineStyle = 0;
                txtObj.Border.RightLineStyle = 0;
                txtObj.Border.TopLineStyle = 0;
                txtObj.Border.BottomLineStyle = 0;
                txtObj.Text = "";


            }//end check for 810
            else
            {
                txtObj.Border.BorderColor = Color.LawnGreen;
            }
            #endregion 810 tag check - show MAILING

            //can use same data from above query and dataTable to get the the job status and description
            //ex) 50d status and it's description is ready to run on digital press
            #region tag status grab (no query, re-use data gathered from 810 check)

            try
            {
                //need to base of the 900 tag's -> tag status
                string tagStat = "";

                foreach (DataRow dr in dtEdit.Rows)
                {
                    //do nothing 
                    if (dr["Work-Center-ID"].ToString() == "900")
                    {

                        tagStat = dr["TagStatus-ID"].ToString();
                    }

                }//end foreach


                //grab first record's 'TagStatus-ID" and set status to it ex) 50d, 09, etc
                var statusObj = cryrpt.ReportDefinition.ReportObjects["status"] as TextObject;
                statusObj.Text = tagStat;

                var statusDescObj = cryrpt.ReportDefinition.ReportObjects["statusDesc"] as TextObject;

                if (tagStat == "01")
                {
                    statusDescObj.Text = "Outside Service";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }

                if (tagStat == "02")
                {
                    statusDescObj.Text = "On Hold";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "05")
                {
                    statusDescObj.Text = "Digital Mailing/Need Run Files";
                }
                if (tagStat == "07")
                {
                    statusDescObj.Text = "Long Term Project (In-House)";
                }
                if (tagStat == "08")
                {
                    statusDescObj.Text = "Long Term Project (out on proof)";
                }
                if (tagStat == "09")
                {
                    statusDescObj.Text = "In Pre Press Production";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "09-R")
                {
                    statusDescObj.Text = "Art corrections after proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "18")
                {
                    statusDescObj.Text = "Out on Random Proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "20")
                {
                    statusDescObj.Text = "Ready to Strip and Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "20p")
                {
                    statusDescObj.Text = "Approved-Waiting for Mock up";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "21")
                {
                    statusDescObj.Text = "Rerun-Pill Plates";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Ready to Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50")
                {
                    statusDescObj.Text = "Press-Running/Plated";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50d" || tagStat == "50D")
                {
                    statusDescObj.Text = "Ready to Run Digital Press";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50e" || tagStat == "50E")
                {
                    statusDescObj.Text = "Ready to Run Digital Press ENVELOPE";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "70")
                {
                    statusDescObj.Text = "Printed and in Bindery";
                    statusObj.Color = Color.Brown;
                    statusDescObj.Color = Color.Brown;
                }
                if (tagStat == "72")
                {
                    statusDescObj.Text = "Monthly DSF billing jobs";
                }
                if (tagStat == "75")
                {
                    statusDescObj.Text = "Waiting for Mailing Data";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "80")
                {
                    statusDescObj.Text = "Bindery Done-Ready for Mailing";
                    statusObj.Color = Color.Blue;
                    statusDescObj.Color = Color.Blue;
                }
                if (tagStat == "82")
                {
                    statusDescObj.Text = "Running on Netjet";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "85")
                {
                    statusDescObj.Text = "Mail Complete/Need Paperwork";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88")
                {
                    statusDescObj.Text = "Mail Complete - Ready to Mail";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88p")
                {
                    statusDescObj.Text = "PARTIAL mail/Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "90")
                {
                    statusDescObj.Text = "Job Completed-Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "92")
                {
                    statusDescObj.Text = "Being Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "95")
                {
                    statusDescObj.Text = "Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "97")
                {
                    statusDescObj.Text = "DSF Jobs To Be Billed";

                }
                if (tagStat == "97b" || tagStat == "97B")
                {
                    statusDescObj.Text = "DSF jobs Already Billed";
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Job Close Pending";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
            }//end try
            catch (Exception ex)
            {

                string error = ex + " : Tag Status update error check code";

                ErrorLog(error);

            }

            #endregion tag status

            #endregion UI


            //sub-reports section
            #region sub report creater


            //Press/Prepress sub rereports - Mailing Version, Mailing Free Feilds, Job Notes, Job Free Feilds, PO Req, PO line,
            //Forms, press, Stock

            #region Job Notes subReport
            string queryJobNotes = "SELECT \"SpecCategory-ID\", Description, \"Created-By\", \"Comment-Date\", \"Update-date\" FROM PUB.JobComments WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtJobNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobNotes = new OdbcDataAdapter(queryJobNotes, dbConn);
                adapJobNotes.Fill(dtJobNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Job Notes report";

                ErrorLog(error);
            }

            //change the names
            dtJobNotes.Columns["SpecCategory-ID"].ColumnName = "SpecID";
            dtJobNotes.Columns["Created-By"].ColumnName = "EnterBy";
            dtJobNotes.Columns["Comment-Date"].ColumnName = "DateEntered";
            dtJobNotes.Columns["Update-date"].ColumnName = "DateUpdated";



            for (int x = dtJobNotes.Rows.Count - 1; x >= 0; x--)
            {


                DataRow dr = dtJobNotes.Rows[x];


                if (dr["SpecID"].ToString() == "10" || dr["SpecID"].ToString() == "00" || dr["SpecID"].ToString() == "" || dr["SpecID"].ToString() == "09")
                {



                }
                else
                {
                    dr.Delete();
                }

                dtJobNotes.AcceptChanges();
            }

            dtJobNotes.AcceptChanges();

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtJobNotes.Rows.Count != 0)
            {
                cryrpt.Subreports["subJobNotes"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobNotes"].SetDataSource(dtJobNotes);

            }
            else
            {

                string subJobComments = "subJobNotes";
                HideSubs(cryrpt, subJobComments);

            }


            #endregion Job Notes subReports

            #region Job free fields subRPt
            string queryFF = "SELECT \"Free-Field-Char\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);
                adapFF.Fill(dtFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Free fields report";

                ErrorLog(error);
            }

            //here is where i need to convert decimal -> free-fieldchar
            string lastJob = dtFF.Rows[1]["Free-Field-Decimal"].ToString();
            dtFF.Rows[1]["Free-Field-Char"] = lastJob;

            dtFF.Columns.Remove("Free-Field-Decimal");

            dtFF.Columns.Add("FFname");
            dtFF.Rows[0]["FFname"] = "Customer To Supply";
            dtFF.Rows[1]["FFname"] = "Last Job#";
            dtFF.Rows[2]["FFname"] = "Type of Proof Needed";
            dtFF.Rows[3]["FFname"] = "Proof Needed By";
            dtFF.Rows[4]["FFname"] = "Coating";
            dtFF.Rows[5]["FFname"] = "Perf Score";
            dtFF.Rows[6]["FFname"] = "Certification";
            dtFF.Rows[7]["FFname"] = "Mailing Services";
            dtFF.Rows[8]["FFname"] = "Litho/Digital/DSF/ASI";
            dtFF.Rows[9]["FFname"] = "Samples";

            CrystalDecisions.CrystalReports.Engine.TextObject txtObjSample;
            txtObjSample = cryrpt.ReportDefinition.ReportObjects["txtSample"] as TextObject;

            if (dtFF.Rows[9]["Free-Field-Char"].ToString() != "None")
            {
                txtObjSample.Text = "";
                txtObjSample.Border.BorderColor = Color.Orange;
                txtObjSample.Text = dtFF.Rows[9]["Free-Field-Char"].ToString();
            }
            else
            {

                txtObjSample.Border.LeftLineStyle = 0;
                txtObjSample.Border.RightLineStyle = 0;
                txtObjSample.Border.TopLineStyle = 0;
                txtObjSample.Border.BottomLineStyle = 0;
                txtObjSample.Text = "";
            }
            //also check if empty exists it is empty hideSubs it
            if (dtFF.Rows.Count != 0)
            {


                if (dtFF.Rows[0]["Free-Field-Char"].ToString() == "DSF")
                {

                    CrystalDecisions.CrystalReports.Engine.TextObject txtDSF;
                    txtDSF = cryrpt.ReportDefinition.ReportObjects["txtDSF"] as TextObject;
                    txtDSF.Text = "DSF";

                }

                cryrpt.Subreports["subJobFreeFields"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobFreeFields"].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subJobFreeFields";
                HideSubs(cryrpt, subFF);

            }


            #endregion job free fields subRpt

            #region ship to data subRpt
            string queryShipTo = "SELECT \"Ship-To-Name\", \"Ship-To-Address1\", \"Ship-To-Address2\", \"Ship-To-Address3\", \"Ship-To-City\", \"Ship-To-State\",\"Ship-To-Zip\",\"Ship-To-Attention\"" +
                ",\"Instructions\", \"Requested-Ship-Date\" ,\"Requested-Quantity\", \"Requested-Number-Packages\", FAX, Phone, \"Ship-To-Country\", \"Shipment-Method-ID\"  FROM PUB.JobShipTo WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtShipTo = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapShipTo = new OdbcDataAdapter(queryShipTo, dbConn);
                adapShipTo.Fill(dtShipTo);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -ship to report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtShipTo.Rows.Count != 0)
            {
                cryrpt.Subreports["subShipTo"].DataSourceConnections.Clear();
                cryrpt.Subreports["subShipTo"].SetDataSource(dtShipTo);

            }
            else
            {

                string subShipTo = "subShipTo";
                HideSubs(cryrpt, subShipTo);

            }

            #endregion ship to data subRpt

            #region form spec subRpt
            string querySpecForm = "SELECT \"Form-ID\",  \"Form-Desc\", \"Number-Pages\", Width, Length, \"Number-forms\", \"Number-Out\", Imposition, Notes FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtSpecForm = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapSpecForm = new OdbcDataAdapter(querySpecForm, dbConn);
                adapSpecForm.Fill(dtSpecForm);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Spec Form report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtSpecForm.Rows.Count != 0)
            {
                cryrpt.Subreports["subFormSpecs"].DataSourceConnections.Clear();
                cryrpt.Subreports["subFormSpecs"].SetDataSource(dtSpecForm);

            }
            else
            {

                string subFormSpec = "subFormSpecs";
                HideSubs(cryrpt, subFormSpec);

            }


            #endregion form spec subRpt

            #region stock rpt
            string queryStock = "SELECT \"Form-ID\", \"Gross-Sheets\", \"Gross-Weight\",  Notes,  \"Inventory-Item-ID\", \"Inventory-Item-Desc\", \"Material-Type\", Length, Width FROM PUB.JobSpecMaterial WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtStock = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapStock = new OdbcDataAdapter(queryStock, dbConn);
                adapStock.Fill(dtStock);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report";

                ErrorLog(error);
            }

            //need to get page-grain new query
            string queryPageGrain = "SELECT \"Page-Grain\" FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtPG = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPG = new OdbcDataAdapter(queryPageGrain, dbConn);
                adapPG.Fill(dtPG);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report -PG";

                ErrorLog(error);
            }

            dtStock.Columns.Add("Page-Grain");

            try
            {
                string val = dtPG.Rows[0]["Page-Grain"].ToString();

                foreach (DataRow dr in dtStock.Rows)
                {

                    dr["Page-Grain"] = val;

                }
            }
            catch (Exception ex) { }

            //also check if empty exists it is empty hideSubs it
            if (dtStock.Rows.Count != 0)
            {
                cryrpt.Subreports["subStock"].DataSourceConnections.Clear();
                cryrpt.Subreports["subStock"].SetDataSource(dtStock);

            }

            else
            {

                string subStock = "subStock";
                HideSubs(cryrpt, subStock);

            }

            #endregion stock rpt

            #region bindery rpt
            string queryBind = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\", \"Number-Out-for-Oper\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtBind = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapBind = new OdbcDataAdapter(queryBind, dbConn);
                adapBind.Fill(dtBind);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Bindery report";

                ErrorLog(error);
            }


            //also check if empty exists it is empty hideSubs it
            if (dtBind.Rows.Count != 0)
            {
                cryrpt.Subreports["subBindery"].DataSourceConnections.Clear();
                cryrpt.Subreports["subBindery"].SetDataSource(dtBind);

            }
            else
            {

                string subBindery = "subBindery";
                HideSubs(cryrpt, subBindery);

            }


            #endregion bindery rpt

            #region bindery matts rpt
            string queryBindMatts = "SELECT \"Form-ID\", \"Material-Type\",  \"Inventory-Item-ID\", \"Qty-Gross-Costing\", \"Qty-Net-Costing\", \"Inventory-Item-Desc\" FROM PUB.JobSpecMaterial WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtBindMatts = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapBindMatts = new OdbcDataAdapter(queryBindMatts, dbConn);
                adapBindMatts.Fill(dtBindMatts);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Bindery Matts report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtBind.Rows.Count != 0)
            {
                cryrpt.Subreports["subBinderyMatts"].DataSourceConnections.Clear();
                cryrpt.Subreports["subBinderyMatts"].SetDataSource(dtBindMatts);

            }
            else
            {

                string subBinderyMatts = "subBinderyMatts";
                HideSubs(cryrpt, subBinderyMatts);

            }

            #endregion bindery matts rpt

            #region estimateDetail

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;

            DataTable estimateIDTbl = new DataTable();
            DataTable estDetailLines = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId = new OdbcDataAdapter(estimateID, dbConn);
                adapEstId.Fill(estimateIDTbl);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl.Rows.Count == 0) { }

            else
            {

                string eID = estimateIDTbl.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines = new OdbcDataAdapter(estDetail, dbConn);
                    adapEstIdLines.Fill(estDetailLines);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines.Rows[i][1].ToString() == "" || estDetailLines.Rows[i][1].ToString() == "Main Component" || estDetailLines.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines.AcceptChanges();

                cryrpt.Subreports[4].DataSourceConnections.Clear();
                cryrpt.Subreports[4].SetDataSource(estDetailLines);

            }
            else
            {

                string subEstDetail = "subEstimateDetails";
                HideSubs(cryrpt, subEstDetail);

            }


            #endregion

            #region estimateDetail2

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID2 = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;

            DataTable estimateIDTbl2 = new DataTable();
            DataTable estDetailLines2 = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId2 = new OdbcDataAdapter(estimateID2, dbConn);
                adapEstId2.Fill(estimateIDTbl2);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl2.Rows.Count == 0) { }

            else
            {

                string eID2 = estimateIDTbl2.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail2 = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID2;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines2 = new OdbcDataAdapter(estDetail2, dbConn);
                    adapEstIdLines2.Fill(estDetailLines2);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines2.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines2.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines2.Rows[i][1].ToString() == "" || estDetailLines2.Rows[i][1].ToString() == "Main Component" || estDetailLines2.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines2.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines2.AcceptChanges();
                cryrpt.Subreports[5].DataSourceConnections.Clear();
                cryrpt.Subreports[5].SetDataSource(estDetailLines2);

            }
            else
            {

                string subEstDetail2 = "subEstimateDetails2";
                HideSubs(cryrpt, subEstDetail2);

            }


            #endregion

            #region estimateDetail3

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID3 = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;

            DataTable estimateIDTbl3 = new DataTable();
            DataTable estDetailLines3 = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId3 = new OdbcDataAdapter(estimateID3, dbConn);
                adapEstId3.Fill(estimateIDTbl3);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl3.Rows.Count == 0) { }

            else
            {

                string eID3 = estimateIDTbl3.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail3 = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID3;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines3 = new OdbcDataAdapter(estDetail3, dbConn);
                    adapEstIdLines3.Fill(estDetailLines3);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines3.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines3.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines3.Rows[i][1].ToString() == "" || estDetailLines3.Rows[i][1].ToString() == "Main Component" || estDetailLines3.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines3.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines3.AcceptChanges();

                cryrpt.Subreports[6].DataSourceConnections.Clear();
                cryrpt.Subreports[6].SetDataSource(estDetailLines3);

            }
            else
            {

                string subEstDetail3 = "subEstimateDetails3";
                HideSubs(cryrpt, subEstDetail3);

            }


            #endregion

            #endregion sub report creation


            dbConn.Close();
            #endregion DB close connection

            #region hideSubs
            string subMailVersionFF = "subMailVersion";
            HideSubs(cryrpt, subMailVersionFF);

            string subMailFF = "subMailFreeFields";
            HideSubs(cryrpt, subMailFF);

            string subJobMV = "subJobMailVersion";
            HideSubs(cryrpt, subJobMV);

            string subJobMV2 = "subJobMailVersion2";
            HideSubs(cryrpt, subJobMV2);

            string subPOreq = "subPOReq";
            HideSubs(cryrpt, subPOreq);

            string subPOLine = "subPOLine";
            HideSubs(cryrpt, subPOLine);

            string subPrepress = "subPrepress";
            HideSubs(cryrpt, subPrepress);

            string subPress = "subPress";
            HideSubs(cryrpt, subPress);

            string subFormNotes = "subFormNotes";
            HideSubs(cryrpt, subFormNotes);

            string subAlt = "subAlterations";
            HideSubs(cryrpt, subAlt);

            string sub810Notes = "sub810Notes";
            HideSubs(cryrpt, sub810Notes);

            string subMailVersionFF2 = "subMailVersion2";
            HideSubs(cryrpt, subMailVersionFF2);

            #endregion hideSubs


            #region display rpt

            //surpress billing section
            cryrpt.ReportDefinition.Sections["billingSection"].SectionFormat.EnableSuppress = true;

            CrystalReportViewer1.ReportSource = cryrpt;
            CrystalReportViewer1.RefreshReport();

            cryrpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "VVT Report");

            #endregion display rpt
        }


        #endregion

        protected void poTicketBttn_Click(object sender, EventArgs e)
        {

            string jobNumber = jobNumTextBox.Text;
            CrystalReport2 cryrpt = new CrystalReport2();

            cryrpt.DataSourceConnections.Clear();  //clear the connections (will popualte with fresh sql query defined data

            //set the databse login info, use twice first one is to login into sql server

            //getting write only error


            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");
            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this does not error out!?
            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //try our best to never have paramter panel pop-up, works like 95% of time
            //error here as well?
            //LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;


            #region UI on CR object editing (non-DB dependant)

            //change text object, tell user what ticket the are curentlly running/looking at
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "PO Ticket";


            #endregion UI on CR object editing (non-DB dependant)



            //here is where i need to clean up lots, make functions, sql queryies, DT and DS manipualtion, pass the job number, etc 
            //main code for program functionallity
            #region DB connection 


            //here is where i iwll be connecting to DB's (all queries will need access to basically my connection properties globals)
            #region global connection properties

            //connection string for DB
            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //open th econnection and error check
            OdbcConnection dbConn = new OdbcConnection(connectStr);
            dbConn.ConnectionTimeout = 150;
            try
            {
                dbConn.Open();
            }
            catch (Exception ex)
            {

                string error = ex + " : DB error cannot connect";

                ErrorLog(error);

            }
            #endregion global connection propeties




            //here is where i can change the UI of the report based on database data
            //ex) show word nailing on report if job# has a 810 tag associated with it
            #region UI crystal report editing (DB dependant)


            #region Header

            //set the job numbers from iuser input
            CrystalDecisions.CrystalReports.Engine.TextObject jobNum1;
            jobNum1 = cryrpt.ReportDefinition.ReportObjects["jobNum"] as TextObject;
            jobNum1.Text = jobNumber;

            CrystalDecisions.CrystalReports.Engine.TextObject jobNum2;
            jobNum2 = cryrpt.ReportDefinition.ReportObjects["jobNum2"] as TextObject;
            jobNum2.Text = jobNumber;



            String headerJob = "SELECT \"Job-Desc\", \"Date-Promised\", \"Sales-Rep-ID\", \"CSR-ID\", \"" +
                "PO-Number\", \"Overs-Allowed\", \"Last-Estimate-ID\", \"Quantity-Ordered\", \"Contact-Name\", \"Date-Entered\", \"Cust-ID-Ordered-by\"" +
                " FROM PUB.JOB WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtHeader = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter header = new OdbcDataAdapter(headerJob, dbConn);
                header.Fill(dtHeader);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set all text objects to the data from datatable
            try
            {
                //job descriptions
                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc;
                jobDesc = cryrpt.ReportDefinition.ReportObjects["jobDesc"] as TextObject;
                jobDesc.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc2;
                jobDesc2 = cryrpt.ReportDefinition.ReportObjects["jobDesc2"] as TextObject;
                jobDesc2.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                //date promised
                CrystalDecisions.CrystalReports.Engine.TextObject dateProm;
                dateProm = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
                dateProm.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject dateProm2;
                dateProm2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
                dateProm2.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                //qty, format from "3400" -> "3,400"
                CrystalDecisions.CrystalReports.Engine.TextObject qty;
                qty = cryrpt.ReportDefinition.ReportObjects["qty"] as TextObject;
                int qtyFormat = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty.Text = String.Format("{0:N0}", qtyFormat);

                CrystalDecisions.CrystalReports.Engine.TextObject qty2;
                qty2 = cryrpt.ReportDefinition.ReportObjects["qty2"] as TextObject;
                int qtyFormat2 = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty2.Text = String.Format("{0:N0}", qtyFormat2);

                //end formatting qty

                //job contact
                CrystalDecisions.CrystalReports.Engine.TextObject jobContact;
                jobContact = cryrpt.ReportDefinition.ReportObjects["contactName"] as TextObject;
                jobContact.Text = dtHeader.Rows[0]["Contact-Name"].ToString();

                //job date entered
                CrystalDecisions.CrystalReports.Engine.TextObject jobDE;
                jobDE = cryrpt.ReportDefinition.ReportObjects["jobDE"] as TextObject;
                jobDE.Text = dtHeader.Rows[0]["Date-Entered"].ToString();

                //over allowed
                CrystalDecisions.CrystalReports.Engine.TextObject OA;
                OA = cryrpt.ReportDefinition.ReportObjects["jobOA"] as TextObject;
                OA.Text = dtHeader.Rows[0]["Overs-Allowed"].ToString();

                //po num
                CrystalDecisions.CrystalReports.Engine.TextObject PO;
                PO = cryrpt.ReportDefinition.ReportObjects["poNum"] as TextObject;
                PO.Text = dtHeader.Rows[0]["PO-Number"].ToString();

                //estimate
                string est = dtHeader.Rows[0]["Last-Estimate-ID"].ToString().Insert(6, "-");
                CrystalDecisions.CrystalReports.Engine.TextObject estimate;
                estimate = cryrpt.ReportDefinition.ReportObjects["estimate"] as TextObject;
                estimate.Text = est;
            }
            catch (Exception ex) { }
            //customer query and text objects
            String headerCust = "SELECT \"cust-name\", \"Address-1\", \"Address-2\", \"City\", \"" +
               "State\", \"Zip\", \"Phone\", \"Address-3\", \"Group-ID\"FROM PUB.cust WHERE \"Cust-code\" = " + dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();


            DataTable dtCust = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter cust = new OdbcDataAdapter(headerCust, dbConn);
                cust.Fill(dtCust);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {


                //No viso labels
                CrystalDecisions.CrystalReports.Engine.TextObject visoLbl;
                visoLbl = cryrpt.ReportDefinition.ReportObjects["Text37"] as TextObject;
                //if the customer group-ID contains "broker" then
                if (dtCust.Rows[0]["Group-ID"].ToString().Contains("broker") || dtCust.Rows[0]["Group-ID"].ToString().Contains("Broker"))
                {

                    visoLbl.Text = "BLIND SHIP";

                }


                //set the Customer info text objects
                //cust name
                CrystalDecisions.CrystalReports.Engine.TextObject custName;
                custName = cryrpt.ReportDefinition.ReportObjects["custName"] as TextObject;
                custName.Text = dtCust.Rows[0]["cust-name"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject custName2;
                custName2 = cryrpt.ReportDefinition.ReportObjects["custName2"] as TextObject;
                custName2.Text = dtCust.Rows[0]["cust-name"].ToString();

                //address -> add 1 and 2 and 3 combined
                CrystalDecisions.CrystalReports.Engine.TextObject custAdd;
                custAdd = cryrpt.ReportDefinition.ReportObjects["custAddress"] as TextObject;
                custAdd.Text = dtCust.Rows[0]["Address-1"].ToString() + " " + dtCust.Rows[0]["Address-2"].ToString() + " " + dtCust.Rows[0]["Address-3"].ToString();

                //city state zip customer
                CrystalDecisions.CrystalReports.Engine.TextObject custCSZ;
                custCSZ = cryrpt.ReportDefinition.ReportObjects["custCSZ"] as TextObject;
                custCSZ.Text = dtCust.Rows[0]["City"].ToString() + " " + dtCust.Rows[0]["State"].ToString() + " " + dtCust.Rows[0]["Zip"].ToString();

                //customerPhone
                CrystalDecisions.CrystalReports.Engine.TextObject custPhone;
                custPhone = cryrpt.ReportDefinition.ReportObjects["custPhone"] as TextObject;
                custPhone.Text = dtCust.Rows[0]["Phone"].ToString();

                //customer number
                CrystalDecisions.CrystalReports.Engine.TextObject custNum;
                custNum = cryrpt.ReportDefinition.ReportObjects["custNum"] as TextObject;
                custNum.Text = dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();
            }
            catch (Exception ex) { }
            //sales agent query and txt obj change
            //why this does not work i have no fkin clue, making stack overflwo see what the brians can thunk up
            String headerSalesAgent = "SELECT \"Sales-Agent-Name\" FROM PUB.\"sales-agent\" WHERE \"Sales-agent-id\" = " + "'" + dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "'";

            // String headerSalesAgent = "SELECT \"Sales-agent-id\" , \"Sales-Agent-Name\" FROM PUB.\"sales-agent\"";

            DataTable dtSalesAgent = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter salesAgent = new OdbcDataAdapter(headerSalesAgent, dbConn);
                salesAgent.Fill(dtSalesAgent);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {
                //sales agent name
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep;
                salesRep = cryrpt.ReportDefinition.ReportObjects["salesRep"] as TextObject;
                salesRep.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

                //sales rep ID for billing
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep2;
                salesRep2 = cryrpt.ReportDefinition.ReportObjects["salesRep2"] as TextObject;
                salesRep2.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString();// + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();
            }
            catch (Exception ex) { }

            //csr query and txt obj change
            String headerCSR = "SELECT \"CSR-Name\" FROM PUB.CSR WHERE \"CSR-ID\"=" + "'" + dtHeader.Rows[0]["CSR-ID"].ToString() + "'";

            DataTable dtCsr = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter csrAdap = new OdbcDataAdapter(headerCSR, dbConn);
                csrAdap.Fill(dtCsr);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            try
            {
                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = dtHeader.Rows[0]["CSR-ID"].ToString() + "-" + dtCsr.Rows[0]["CSR-Name"].ToString();
            }
            catch (Exception ex)
            {

                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = "";

            }
            #endregion Header


            #region 810 tag check - show MAILING
            //DataTable for all UI db-depenedant editing
            DataTable dtEdit = new DataTable();


            String query810Tag = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumber;

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810Tag = new OdbcDataAdapter(query810Tag, dbConn);
                adap810Tag.Fill(dtEdit);
            }
            catch (Exception ex)
            {


                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //here is needed for 810 chck
            bool check = false;

            foreach (DataRow dr in dtEdit.Rows)
            {
                //do nothing 
                if (dr["Work-Center-ID"].ToString().Contains("810"))
                {

                    check = true;
                }

            }//end foreach

            //no check to see if 810 tag is present
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
            txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
            //no check to see if 810 tag is present
            if (!check)
            {
                txtObj.Border.LeftLineStyle = 0;
                txtObj.Border.RightLineStyle = 0;
                txtObj.Border.TopLineStyle = 0;
                txtObj.Border.BottomLineStyle = 0;
                txtObj.Text = "";


            }//end check for 810
            else
            {
                txtObj.Border.BorderColor = Color.LawnGreen;
            }
            #endregion 810 tag check - show MAILING

            //can use same data from above query and dataTable to get the the job status and description
            //ex) 50d status and it's description is ready to run on digital press
            #region tag status grab (no query, re-use data gathered from 810 check)

            try
            {
                //need to base of the 900 tag's -> tag status
                string tagStat = "";

                foreach (DataRow dr in dtEdit.Rows)
                {
                    //do nothing 
                    if (dr["Work-Center-ID"].ToString() == "900")
                    {

                        tagStat = dr["TagStatus-ID"].ToString();
                    }

                }//end foreach


                //grab first record's 'TagStatus-ID" and set status to it ex) 50d, 09, etc
                var statusObj = cryrpt.ReportDefinition.ReportObjects["status"] as TextObject;
                statusObj.Text = tagStat;

                var statusDescObj = cryrpt.ReportDefinition.ReportObjects["statusDesc"] as TextObject;

                if (tagStat == "01")
                {
                    statusDescObj.Text = "Outside Service";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }

                if (tagStat == "02")
                {
                    statusDescObj.Text = "On Hold";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "05")
                {
                    statusDescObj.Text = "Digital Mailing/Need Run Files";
                }
                if (tagStat == "07")
                {
                    statusDescObj.Text = "Long Term Project (In-House)";
                }
                if (tagStat == "08")
                {
                    statusDescObj.Text = "Long Term Project (out on proof)";
                }
                if (tagStat == "09")
                {
                    statusDescObj.Text = "In Pre Press Production";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "09-R")
                {
                    statusDescObj.Text = "Art corrections after proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "18")
                {
                    statusDescObj.Text = "Out on Random Proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "20")
                {
                    statusDescObj.Text = "Ready to Strip and Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "20p")
                {
                    statusDescObj.Text = "Approved-Waiting for Mock up";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "21")
                {
                    statusDescObj.Text = "Rerun-Pill Plates";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Ready to Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50")
                {
                    statusDescObj.Text = "Press-Running/Plated";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50d" || tagStat == "50D")
                {
                    statusDescObj.Text = "Ready to Run Digital Press";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50e" || tagStat == "50E")
                {
                    statusDescObj.Text = "Ready to Run Digital Press ENVELOPE";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "70")
                {
                    statusDescObj.Text = "Printed and in Bindery";
                    statusObj.Color = Color.Brown;
                    statusDescObj.Color = Color.Brown;
                }
                if (tagStat == "72")
                {
                    statusDescObj.Text = "Monthly DSF billing jobs";
                }
                if (tagStat == "75")
                {
                    statusDescObj.Text = "Waiting for Mailing Data";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "80")
                {
                    statusDescObj.Text = "Bindery Done-Ready for Mailing";
                    statusObj.Color = Color.Blue;
                    statusDescObj.Color = Color.Blue;
                }
                if (tagStat == "82")
                {
                    statusDescObj.Text = "Running on Netjet";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "85")
                {
                    statusDescObj.Text = "Mail Complete/Need Paperwork";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88")
                {
                    statusDescObj.Text = "Mail Complete - Ready to Mail";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88p")
                {
                    statusDescObj.Text = "PARTIAL mail/Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "90")
                {
                    statusDescObj.Text = "Job Completed-Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "92")
                {
                    statusDescObj.Text = "Being Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "95")
                {
                    statusDescObj.Text = "Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "97")
                {
                    statusDescObj.Text = "DSF Jobs To Be Billed";

                }
                if (tagStat == "97b" || tagStat == "97B")
                {
                    statusDescObj.Text = "DSF jobs Already Billed";
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Job Close Pending";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
            }//end try
            catch (Exception ex)
            {

                string error = ex + " : Tag Status update error check code";

                ErrorLog(error);

            }

            #endregion tag status

            #region Job free fields subRPt
            string queryFF = "SELECT \"Free-Field-Char\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);
                adapFF.Fill(dtFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Free fields report";

                ErrorLog(error);
            }

            //here is where i need to convert decimal -> free-fieldchar
            string lastJob = dtFF.Rows[1]["Free-Field-Decimal"].ToString();
            dtFF.Rows[1]["Free-Field-Char"] = lastJob;

            try
            {
                dtFF.Columns.Remove("Free-Field-Decimal");
                dtFF.Columns.Add("FFname");
                dtFF.Rows[0]["FFname"] = "Customer To Supply";
                dtFF.Rows[1]["FFname"] = "Last Job#";
                dtFF.Rows[2]["FFname"] = "Type of Proof Needed";
                dtFF.Rows[3]["FFname"] = "Proof Needed By";
                dtFF.Rows[4]["FFname"] = "Coating";
                dtFF.Rows[5]["FFname"] = "Perf Score";
                dtFF.Rows[6]["FFname"] = "Certification";
                dtFF.Rows[7]["FFname"] = "Mailing Services";
                dtFF.Rows[8]["FFname"] = "Litho/Digital/DSF/ASI";
                dtFF.Rows[9]["FFname"] = "Samples";

                CrystalDecisions.CrystalReports.Engine.TextObject txtObjSample;
                txtObjSample = cryrpt.ReportDefinition.ReportObjects["txtSample"] as TextObject;

                if (dtFF.Rows[9]["Free-Field-Char"].ToString() != "None")
                {
                    txtObjSample.Text = "";
                    txtObjSample.Border.BorderColor = Color.Orange;
                    txtObjSample.Text = dtFF.Rows[9]["Free-Field-Char"].ToString();
                }
                else
                {

                    txtObjSample.Border.LeftLineStyle = 0;
                    txtObjSample.Border.RightLineStyle = 0;
                    txtObjSample.Border.TopLineStyle = 0;
                    txtObjSample.Border.BottomLineStyle = 0;
                    txtObjSample.Text = "";
                }
            }
            catch (Exception ex) { }

            //also check if empty exists it is empty hideSubs it
            if (dtFF.Rows.Count != 0)
            {

                dtFF.Rows.RemoveAt(6);
                dtFF.Rows.RemoveAt(4);
                dtFF.Rows.RemoveAt(3);
                dtFF.Rows.RemoveAt(2);
                dtFF.Rows.RemoveAt(0);
                dtFF.AcceptChanges();

                //will have to do same process as adding field to the dataSet as in Mail Version
                //also need to flag DSF jobs 

                if (dtFF.Rows[0]["Free-Field-Char"].ToString() == "DSF")
                {

                    CrystalDecisions.CrystalReports.Engine.TextObject txtDSF;
                    txtDSF = cryrpt.ReportDefinition.ReportObjects["txtDSF"] as TextObject;
                    txtDSF.Text = "DSF";

                }

                cryrpt.Subreports["subJobFreeFields"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobFreeFields"].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subJobFreeFields";
                HideSubs(cryrpt, subFF);

            }


            #endregion job free fields subRpt



            #endregion UI


            //sub-reports section
            #region sub report creater

            //Press/Prepress sub rereports - Mailing Version, Mailing Free Feilds, Job Notes, Job Free Feilds, PO Req, PO line,
            //Forms, press, Stock

            #region Job Notes subReport
            string queryJobNotes = "SELECT \"SpecCategory-ID\", Description, \"Created-By\", \"Comment-Date\", \"Update-date\" FROM PUB.JobComments WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtJobNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobNotes = new OdbcDataAdapter(queryJobNotes, dbConn);
                adapJobNotes.Fill(dtJobNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Job Notes report";

                ErrorLog(error);
            }

            //change the names
            dtJobNotes.Columns["SpecCategory-ID"].ColumnName = "SpecID";
            dtJobNotes.Columns["Created-By"].ColumnName = "EnterBy";
            dtJobNotes.Columns["Comment-Date"].ColumnName = "DateEntered";
            dtJobNotes.Columns["Update-date"].ColumnName = "DateUpdated";


            for (int x = dtJobNotes.Rows.Count - 1; x >= 0; x--)
            {


                DataRow dr = dtJobNotes.Rows[x];


                if (dr["SpecID"].ToString() == "10" || dr["SpecID"].ToString() == "00" || dr["SpecID"].ToString() == "" || dr["SpecID"].ToString() == "09" || dr["SpecID"].ToString() == "05")
                {



                }
                else
                {
                    dr.Delete();
                }

                dtJobNotes.AcceptChanges();
            }

            dtJobNotes.AcceptChanges();

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtJobNotes.Rows.Count != 0)
            {
                cryrpt.Subreports["subJobNotes"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobNotes"].SetDataSource(dtJobNotes);

            }
            else
            {

                string subJobComments = "subJobNotes";
                HideSubs(cryrpt, subJobComments);

            }


            #endregion Job Notes subReports

            #region PO req subRpt
            string queryPOreq = "SELECT \"Req-Number\", \"Supplier-ID\", \"Supplier-Name\", \"Inventory-Item-ID\", \"Item-Desc\", \"Qty-Requisitioned\", \"Unit-Meas-Purchase\", \"Unit-Cost-Purchase\", \"Req-Status\" FROM PUB.PORequisition WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtPOreq = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOreq = new OdbcDataAdapter(queryPOreq, dbConn);
                adapPOreq.Fill(dtPOreq);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Req report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOreq.Rows.Count != 0)
            {
                cryrpt.Subreports["subPOReq"].DataSourceConnections.Clear();
                cryrpt.Subreports["subPOReq"].SetDataSource(dtPOreq);

            }
            else
            {

                string subPOreq = "subPOReq";
                HideSubs(cryrpt, subPOreq);

            }

            #endregion PO req subRpt


            #region PO line info subRpt
            string queryPOLine = "SELECT \"PO-Number\", \"Line-Status\", \"Date-PO\", \"Date-Last-Receipt\", \"Item-Desc\", \"Qty-Received-Purchase\" FROM PUB.POLine WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtPOLine = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOLine = new OdbcDataAdapter(queryPOLine, dbConn);
                adapPOLine.Fill(dtPOLine);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Line report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOLine.Rows.Count != 0)
            {
                cryrpt.Subreports["subPOLine"].DataSourceConnections.Clear();
                cryrpt.Subreports["subPOLine"].SetDataSource(dtPOLine);

            }
            else
            {

                string subPOLine = "subPOLine";
                HideSubs(cryrpt, subPOLine);

            }
            #endregion Po Line info Subrpt

            #region estimateDetail

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;

            DataTable estimateIDTbl = new DataTable();
            DataTable estDetailLines = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId = new OdbcDataAdapter(estimateID, dbConn);
                adapEstId.Fill(estimateIDTbl);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl.Rows.Count == 0) { }

            else
            {

                string eID = estimateIDTbl.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines = new OdbcDataAdapter(estDetail, dbConn);
                    adapEstIdLines.Fill(estDetailLines);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines.Rows[i][1].ToString() == "" || estDetailLines.Rows[i][1].ToString() == "Main Component" || estDetailLines.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines.AcceptChanges();

                cryrpt.Subreports[4].DataSourceConnections.Clear();
                cryrpt.Subreports[4].SetDataSource(estDetailLines);

            }
            else
            {

                string subEstDetail = "subEstimateDetails";
                HideSubs(cryrpt, subEstDetail);

            }


            #endregion

            #region estimateDetail2

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID2 = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;

            DataTable estimateIDTbl2 = new DataTable();
            DataTable estDetailLines2 = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId2 = new OdbcDataAdapter(estimateID2, dbConn);
                adapEstId2.Fill(estimateIDTbl2);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl2.Rows.Count == 0) { }

            else
            {

                string eID2 = estimateIDTbl2.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail2 = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID2;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines2 = new OdbcDataAdapter(estDetail2, dbConn);
                    adapEstIdLines2.Fill(estDetailLines2);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines2.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines2.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines2.Rows[i][1].ToString() == "" || estDetailLines2.Rows[i][1].ToString() == "Main Component" || estDetailLines2.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines2.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines2.AcceptChanges();
                cryrpt.Subreports[5].DataSourceConnections.Clear();
                cryrpt.Subreports[5].SetDataSource(estDetailLines2);

            }
            else
            {

                string subEstDetail2 = "subEstimateDetails2";
                HideSubs(cryrpt, subEstDetail2);

            }


            #endregion

            #region estimateDetail3

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID3 = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;

            DataTable estimateIDTbl3 = new DataTable();
            DataTable estDetailLines3 = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId3 = new OdbcDataAdapter(estimateID3, dbConn);
                adapEstId3.Fill(estimateIDTbl3);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl3.Rows.Count == 0) { }

            else
            {

                string eID3 = estimateIDTbl3.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail3 = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID3;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines3 = new OdbcDataAdapter(estDetail3, dbConn);
                    adapEstIdLines3.Fill(estDetailLines3);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines3.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines3.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines3.Rows[i][1].ToString() == "" || estDetailLines3.Rows[i][1].ToString() == "Main Component" || estDetailLines3.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines3.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines3.AcceptChanges();

                cryrpt.Subreports[6].DataSourceConnections.Clear();
                cryrpt.Subreports[6].SetDataSource(estDetailLines3);

            }
            else
            {

                string subEstDetail3 = "subEstimateDetails3";
                HideSubs(cryrpt, subEstDetail3);

            }


            #endregion

            #endregion sub report creation


            dbConn.Close();
            #endregion DB close connection

            #region hideSubs
            string subMailVersionFF = "subMailVersion";
            HideSubs(cryrpt, subMailVersionFF);

            string subBinderyMatts = "subBinderyMatts";
            HideSubs(cryrpt, subBinderyMatts);

            string subMailFF = "subMailFreeFields";
            HideSubs(cryrpt, subMailFF);

            string subFF2 = "subJobFreeFields";
            HideSubs(cryrpt, subFF2);

            string subShipTo = "subShipTo";
            HideSubs(cryrpt, subShipTo);

            string subFormSpec = "subFormSpecs";
            HideSubs(cryrpt, subFormSpec);

            string subFormNotes = "subFormNotes";
            HideSubs(cryrpt, subFormNotes);

            string subPrepress = "subPrepress";
            HideSubs(cryrpt, subPrepress);

            string subPress = "subPress";
            HideSubs(cryrpt, subPress);

            string subStock = "subStock";
            HideSubs(cryrpt, subStock);

            string subBindery = "subBindery";
            HideSubs(cryrpt, subBindery);

            string subAlt = "subAlterations";
            HideSubs(cryrpt, subAlt);

            string sub810Notes = "sub810Notes";
            HideSubs(cryrpt, sub810Notes);

            string subMailVersionFF2 = "subMailVersion2";
            HideSubs(cryrpt, subMailVersionFF2);


            string subJobMV = "subJobMailVersion";
            HideSubs(cryrpt, subJobMV);

            string subJobMV2 = "subJobMailVersion2";
            HideSubs(cryrpt, subJobMV2);

            #endregion hideSubs

            #region display rpt

            //surpress billing section
            cryrpt.ReportDefinition.Sections["billingSection"].SectionFormat.EnableSuppress = true;


            CrystalReportViewer1.ReportSource = cryrpt;
            CrystalReportViewer1.RefreshReport();

            cryrpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "VVT Report");



        }

        #endregion PO TICKET


        protected void fullTicketBttn_Click(object sender, EventArgs e)
        {
            string jobNumber = jobNumTextBox.Text;
            CrystalReport2 cryrpt = new CrystalReport2();

            cryrpt.DataSourceConnections.Clear();  //clear the connections (will popualte with fresh sql query defined data

            //set the databse login info, use twice first one is to login into sql server

            //getting write only error


            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");
            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this does not error out!?
            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //try our best to never have paramter panel pop-up, works like 95% of time
            //error here as well?
            //LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;


            #region UI on CR object editing (non-DB dependant)

            //change text object, tell user what ticket the are curentlly running/looking at
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Full Ticket";


            #endregion UI on CR object editing (non-DB dependant)


            // Console.WriteLine("Time for non-DB connection related things: " +DateTime.Now.ToString());


            //here is where i need to clean up lots, make functions, sql queryies, DT and DS manipualtion, pass the job number, etc 
            //main code for program functionallity
            #region DB connection 


            //here is where i iwll be connecting to DB's (all queries will need access to basically my connection properties globals)
            #region global connection properties

            //connection string for DB
            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //open th econnection and error check
            OdbcConnection dbConn = new OdbcConnection(connectStr);
            dbConn.ConnectionTimeout = 150;
            try
            {
                dbConn.Open();
            }
            catch (Exception ex)
            {

                string error = ex + " : DB error cannot connect";

                ErrorLog(error);

            }
            #endregion global connection propeties


            // Console.WriteLine("Time for opening connection to DB " + DateTime.Now.ToString());

            //here is where i can change the UI of the report based on database data
            //ex) show word nailing on report if job# has a 810 tag associated with it
            #region UI crystal report editing (DB dependant)


            #region Header

            //set the job numbers from iuser input
            CrystalDecisions.CrystalReports.Engine.TextObject jobNum1;
            jobNum1 = cryrpt.ReportDefinition.ReportObjects["jobNum"] as TextObject;
            jobNum1.Text = jobNumber;

            CrystalDecisions.CrystalReports.Engine.TextObject jobNum2;
            jobNum2 = cryrpt.ReportDefinition.ReportObjects["jobNum2"] as TextObject;
            jobNum2.Text = jobNumber;



            String headerJob = "SELECT \"Job-Desc\", \"Date-Promised\", \"Sales-Rep-ID\", \"CSR-ID\", \"" +
                "PO-Number\", \"Overs-Allowed\", \"Last-Estimate-ID\", \"Quantity-Ordered\", \"Contact-Name\", \"Date-Entered\", \"Cust-ID-Ordered-by\"" +
                " FROM PUB.JOB WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtHeader = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter header = new OdbcDataAdapter(headerJob, dbConn);
                header.Fill(dtHeader);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //running totals
           // recordCount += dtHeader.Rows.Count;
            //itemCount += dtHeader.Columns.Count;

            //current DT
            //recordCurrentCount += dtHeader.Rows.Count;
           // itemCurrentCount += dtHeader.Columns.Count;
            // Console.WriteLine("Header of CR Job - This* sql statment/Datatable fill - Record Row Current: "+ recordCurrentCount+ " Elements current "+ itemCurrentCount +"\nRunning Total:Record Rows: "+recordCount+" Elements per row: "+itemCount);
            //sqlCount++;
            //recordCurrentCount = 0;
           // itemCurrentCount = 0;
            // Console.WriteLine("Time for Header of CR Job" + DateTime.Now.ToString());

            //set all text objects to the data from datatable
            try
            {
                //job descriptions
                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc;
                jobDesc = cryrpt.ReportDefinition.ReportObjects["jobDesc"] as TextObject;
                jobDesc.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject jobDesc2;
                jobDesc2 = cryrpt.ReportDefinition.ReportObjects["jobDesc2"] as TextObject;
                jobDesc2.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

                //date promised
                CrystalDecisions.CrystalReports.Engine.TextObject dateProm;
                dateProm = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
                dateProm.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject dateProm2;
                dateProm2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
                dateProm2.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

                //qty, format from "3400" -> "3,400"
                CrystalDecisions.CrystalReports.Engine.TextObject qty;
                qty = cryrpt.ReportDefinition.ReportObjects["qty"] as TextObject;
                int qtyFormat = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty.Text = String.Format("{0:N0}", qtyFormat);

                CrystalDecisions.CrystalReports.Engine.TextObject qty2;
                qty2 = cryrpt.ReportDefinition.ReportObjects["qty2"] as TextObject;
                int qtyFormat2 = Convert.ToInt32(dtHeader.Rows[0]["Quantity-Ordered"].ToString());
                qty2.Text = String.Format("{0:N0}", qtyFormat2);

                //end formatting qty

                //job contact
                CrystalDecisions.CrystalReports.Engine.TextObject jobContact;
                jobContact = cryrpt.ReportDefinition.ReportObjects["contactName"] as TextObject;
                jobContact.Text = dtHeader.Rows[0]["Contact-Name"].ToString();

                //job date entered
                CrystalDecisions.CrystalReports.Engine.TextObject jobDE;
                jobDE = cryrpt.ReportDefinition.ReportObjects["jobDE"] as TextObject;
                jobDE.Text = dtHeader.Rows[0]["Date-Entered"].ToString();

                //over allowed
                CrystalDecisions.CrystalReports.Engine.TextObject OA;
                OA = cryrpt.ReportDefinition.ReportObjects["jobOA"] as TextObject;
                OA.Text = dtHeader.Rows[0]["Overs-Allowed"].ToString();

                //po num
                CrystalDecisions.CrystalReports.Engine.TextObject PO;
                PO = cryrpt.ReportDefinition.ReportObjects["poNum"] as TextObject;
                PO.Text = dtHeader.Rows[0]["PO-Number"].ToString();

                //estimate
                string est = dtHeader.Rows[0]["Last-Estimate-ID"].ToString().Insert(6, "-");
                CrystalDecisions.CrystalReports.Engine.TextObject estimate;
                estimate = cryrpt.ReportDefinition.ReportObjects["estimate"] as TextObject;
                estimate.Text = est;
            }
            catch (Exception ex) { }

            //customer query and text objects
            //customer query and text objects
            String headerCust = "SELECT \"cust-name\", \"Address-1\", \"Address-2\", \"City\", \"" +
               "State\", \"Zip\", \"Phone\", \"Address-3\", \"Group-ID\"FROM PUB.cust WHERE \"Cust-code\" = " + dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();



            DataTable dtCust = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter cust = new OdbcDataAdapter(headerCust, dbConn);
                cust.Fill(dtCust);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }
            //running totals
           // recordCount += dtCust.Rows.Count;
           // itemCount += dtCust.Columns.Count;

            //current DT
           // recordCurrentCount += dtCust.Rows.Count;
          //  itemCurrentCount += dtCust.Columns.Count;
            // Console.WriteLine("Header of CR Customer - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
           // sqlCount++;
           // recordCurrentCount = 0;
           // itemCurrentCount = 0;
            // Console.WriteLine("Time for Header of CR Customer" + DateTime.Now.ToString());

            //set the Customer info text objects
            //cust name
            try
            {

                //No viso labels
                CrystalDecisions.CrystalReports.Engine.TextObject visoLbl;
                visoLbl = cryrpt.ReportDefinition.ReportObjects["Text37"] as TextObject;
                //if the customer group-ID contains "broker" then
                if (dtCust.Rows[0]["Group-ID"].ToString().Contains("broker") || dtCust.Rows[0]["Group-ID"].ToString().Contains("Broker"))
                {

                    visoLbl.Text = "BLIND SHIP";

                }


                CrystalDecisions.CrystalReports.Engine.TextObject custName;
                custName = cryrpt.ReportDefinition.ReportObjects["custName"] as TextObject;
                custName.Text = dtCust.Rows[0]["cust-name"].ToString();

                CrystalDecisions.CrystalReports.Engine.TextObject custName2;
                custName2 = cryrpt.ReportDefinition.ReportObjects["custName2"] as TextObject;
                custName2.Text = dtCust.Rows[0]["cust-name"].ToString();

                //address -> add 1 and 2 and 3 combined
                CrystalDecisions.CrystalReports.Engine.TextObject custAdd;
                custAdd = cryrpt.ReportDefinition.ReportObjects["custAddress"] as TextObject;
                custAdd.Text = dtCust.Rows[0]["Address-1"].ToString() + " " + dtCust.Rows[0]["Address-2"].ToString() + " " + dtCust.Rows[0]["Address-3"].ToString();

                //city state zip customer
                CrystalDecisions.CrystalReports.Engine.TextObject custCSZ;
                custCSZ = cryrpt.ReportDefinition.ReportObjects["custCSZ"] as TextObject;
                custCSZ.Text = dtCust.Rows[0]["City"].ToString() + " " + dtCust.Rows[0]["State"].ToString() + " " + dtCust.Rows[0]["Zip"].ToString();

                //customerPhone
                CrystalDecisions.CrystalReports.Engine.TextObject custPhone;
                custPhone = cryrpt.ReportDefinition.ReportObjects["custPhone"] as TextObject;
                custPhone.Text = dtCust.Rows[0]["Phone"].ToString();

                //customer number
                CrystalDecisions.CrystalReports.Engine.TextObject custNum;
                custNum = cryrpt.ReportDefinition.ReportObjects["custNum"] as TextObject;
                custNum.Text = dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();
            }
            catch (Exception ex) { }

            //sales agent query and txt obj change
            //why this does not work i have no fkin clue, making stack overflwo see what the brians can thunk up
            String headerSalesAgent = "SELECT \"Sales-Agent-Name\" FROM PUB.\"sales-agent\" WHERE \"Sales-agent-id\" = " + "'" + dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "'";

            DataTable dtSalesAgent = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter salesAgent = new OdbcDataAdapter(headerSalesAgent, dbConn);
                salesAgent.Fill(dtSalesAgent);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }
            //running totals
          //  recordCount += dtSalesAgent.Rows.Count;
           // itemCount += dtSalesAgent.Columns.Count;

            //current DT
          //  recordCurrentCount += dtSalesAgent.Rows.Count;
           // itemCurrentCount += dtSalesAgent.Columns.Count;
            // Console.WriteLine("Header of CR Sales Agent - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
          //  sqlCount++;
           // recordCurrentCount = 0;
           // itemCurrentCount = 0;
            //  Console.WriteLine("Time for Header of CR sales agent" + DateTime.Now.ToString());

            try
            {
                //sales agent name
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep;
                salesRep = cryrpt.ReportDefinition.ReportObjects["salesRep"] as TextObject;
                salesRep.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

                //sales rep ID for billing
                CrystalDecisions.CrystalReports.Engine.TextObject salesRep2;
                salesRep2 = cryrpt.ReportDefinition.ReportObjects["salesRep2"] as TextObject;
                salesRep2.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString();// + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();
            }
            catch (Exception ex) { }

            //csr query and txt obj change
            String headerCSR = "SELECT \"CSR-Name\" FROM PUB.CSR WHERE \"CSR-ID\"=" + "'" + dtHeader.Rows[0]["CSR-ID"].ToString() + "'";

            DataTable dtCsr = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter csrAdap = new OdbcDataAdapter(headerCSR, dbConn);
                csrAdap.Fill(dtCsr);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }
            //running totals
         //   recordCount += dtCsr.Rows.Count;
         //   itemCount += dtCsr.Columns.Count;

            //current DT
         //   recordCurrentCount += dtCsr.Rows.Count;
         //   itemCurrentCount += dtCsr.Columns.Count;
            // Console.WriteLine("Header of CR CSR - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
         //   sqlCount++;
          //  recordCurrentCount = 0;
          //  itemCurrentCount = 0;
            //  Console.WriteLine("Time for header for CR CSR" + DateTime.Now.ToString());

            try
            {
                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = dtHeader.Rows[0]["CSR-ID"].ToString() + "-" + dtCsr.Rows[0]["CSR-Name"].ToString();
            }
            catch (Exception ex)
            {

                CrystalDecisions.CrystalReports.Engine.TextObject csr;
                csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
                csr.Text = "";

            }
            #endregion Header


            #region 810 tag check - show MAILING
            //DataTable for all UI db-depenedant editing
            DataTable dtEdit = new DataTable();


            String query810Tag = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumber;

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810Tag = new OdbcDataAdapter(query810Tag, dbConn);
                adap810Tag.Fill(dtEdit);
            }
            catch (Exception ex)
            {


                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //running totals
           // recordCount += dtEdit.Rows.Count;
          //  itemCount += dtEdit.Columns.Count;

            //current DT
          //  recordCurrentCount += dtEdit.Rows.Count;
          //  itemCurrentCount += dtEdit.Columns.Count;
            // Console.WriteLine("TagStatus/810 check - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
         //   sqlCount++;
           // recordCurrentCount = 0;
          //  itemCurrentCount = 0;
            // Console.WriteLine("Time for TagStatus/810 check" + DateTime.Now.ToString());

            //here is needed for 810 chck
            bool check = false;

            foreach (DataRow dr in dtEdit.Rows)
            {
                //do nothing 
                if (dr["Work-Center-ID"].ToString().Contains("810"))
                {

                    check = true;
                }

            }//end foreach

            CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
            txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
            //no check to see if 810 tag is present
            if (!check)
            {
                txtObj.Border.LeftLineStyle = 0;
                txtObj.Border.RightLineStyle = 0;
                txtObj.Border.TopLineStyle = 0;
                txtObj.Border.BottomLineStyle = 0;
                txtObj.Text = "";


            }//end check for 810
            else
            {
                txtObj.Border.BorderColor = Color.LawnGreen;
            }
            #endregion 810 tag check - show MAILING

            //can use same data from above query and dataTable to get the the job status and description
            //ex) 50d status and it's description is ready to run on digital press
            #region tag status grab (no query, re-use data gathered from 810 check)

            try
            {
                //need to base of the 900 tag's -> tag status
                string tagStat = "";

                foreach (DataRow dr in dtEdit.Rows)
                {
                    //do nothing 
                    if (dr["Work-Center-ID"].ToString() == "900")
                    {

                        tagStat = dr["TagStatus-ID"].ToString();
                    }

                }//end foreach


                //grab first record's 'TagStatus-ID" and set status to it ex) 50d, 09, etc
                var statusObj = cryrpt.ReportDefinition.ReportObjects["status"] as TextObject;
                statusObj.Text = tagStat;

                var statusDescObj = cryrpt.ReportDefinition.ReportObjects["statusDesc"] as TextObject;

                if (tagStat == "01")
                {
                    statusDescObj.Text = "Outside Service";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }

                if (tagStat == "02")
                {
                    statusDescObj.Text = "On Hold";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "05")
                {
                    statusDescObj.Text = "Digital Mailing/Need Run Files";
                }
                if (tagStat == "07")
                {
                    statusDescObj.Text = "Long Term Project (In-House)";
                }
                if (tagStat == "08")
                {
                    statusDescObj.Text = "Long Term Project (out on proof)";
                }
                if (tagStat == "09")
                {
                    statusDescObj.Text = "In Pre Press Production";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "09-R")
                {
                    statusDescObj.Text = "Art corrections after proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "18")
                {
                    statusDescObj.Text = "Out on Random Proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "20")
                {
                    statusDescObj.Text = "Ready to Strip and Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "20p")
                {
                    statusDescObj.Text = "Approved-Waiting for Mock up";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "21")
                {
                    statusDescObj.Text = "Rerun-Pill Plates";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Ready to Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50")
                {
                    statusDescObj.Text = "Press-Running/Plated";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50d" || tagStat == "50D")
                {
                    statusDescObj.Text = "Ready to Run Digital Press";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50e" || tagStat == "50E")
                {
                    statusDescObj.Text = "Ready to Run Digital Press ENVELOPE";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "70")
                {
                    statusDescObj.Text = "Printed and in Bindery";
                    statusObj.Color = Color.Brown;
                    statusDescObj.Color = Color.Brown;
                }
                if (tagStat == "72")
                {
                    statusDescObj.Text = "Monthly DSF billing jobs";
                }
                if (tagStat == "75")
                {
                    statusDescObj.Text = "Waiting for Mailing Data";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "80")
                {
                    statusDescObj.Text = "Bindery Done-Ready for Mailing";
                    statusObj.Color = Color.Blue;
                    statusDescObj.Color = Color.Blue;
                }
                if (tagStat == "82")
                {
                    statusDescObj.Text = "Running on Netjet";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "85")
                {
                    statusDescObj.Text = "Mail Complete/Need Paperwork";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88")
                {
                    statusDescObj.Text = "Mail Complete - Ready to Mail";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88p")
                {
                    statusDescObj.Text = "PARTIAL mail/Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "90")
                {
                    statusDescObj.Text = "Job Completed-Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "92")
                {
                    statusDescObj.Text = "Being Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "95")
                {
                    statusDescObj.Text = "Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "97")
                {
                    statusDescObj.Text = "DSF Jobs To Be Billed";

                }
                if (tagStat == "97b" || tagStat == "97B")
                {
                    statusDescObj.Text = "DSF jobs Already Billed";
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Job Close Pending";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
            }//end try
            catch (Exception ex)
            {

                string error = ex + " : Tag Status update error check code";

                ErrorLog(error);

            }

            #endregion tag status

            #endregion UI


            //sub-reports section
            #region sub report creater

            #region Mailing Version subReport
            string queryMailVersion = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtMailVersion = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailVersion = new OdbcDataAdapter(queryMailVersion, dbConn);
                adapMailVersion.Fill(dtMailVersion);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing Version FF report";

                ErrorLog(error);
            }

            //sql query jobMailingVersion for {Version #, Description, Qty}
            string queryJobMailVersion = "SELECT \"Version-Number\", \"Version-Description\", \"Version-Qty\" FROM PUB.JobMailingVersion WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtJobMailVersion = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobMailVersion = new OdbcDataAdapter(queryJobMailVersion, dbConn);
                adapJobMailVersion.Fill(dtJobMailVersion);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Job Mailing Version FF report";

                ErrorLog(error);
            }
            //load each version header
            if (dtJobMailVersion.Rows.Count != 0)
            {
                cryrpt.Subreports["subJobMailVersion"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobMailVersion"].SetDataSource(dtJobMailVersion);

                if (dtJobMailVersion.Rows.Count > 1)
                {
                    //first one goo now delete row 0
                    dtJobMailVersion.Rows[0].Delete();
                    dtJobMailVersion.AcceptChanges();
                    cryrpt.Subreports["subJobMailVersion2"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subJobMailVersion2"].SetDataSource(dtJobMailVersion);
                }
                else
                {
                    string subJobMailVersion2 = "subJobMailVersion2";
                    HideSubs(cryrpt, subJobMailVersion2);
                }
            }
            else
            {

                string subJobMailVersion = "subJobMailVersion";
                HideSubs(cryrpt, subJobMailVersion);
                string subJobMailVersion2 = "subJobMailVersion2";
                HideSubs(cryrpt, subJobMailVersion2);

            }

            //and change second column name to match CR report value "FFvalue"
            dtMailVersion.Columns["Free-Field-Char"].ColumnName = "FFvalue";
            dtMailVersion.Columns.Add("FFname");

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailVersion.Rows.Count != 0)
            {

                //check how many bversions there are
                if (dtMailVersion.Rows.Count > 9)
                {

                    //process as 2 mailing versions

                    try
                    {
                        dtMailVersion.Rows[0]["FFname"] = "Permit Type";
                        dtMailVersion.Rows[1]["FFname"] = "Permit Owner";
                        dtMailVersion.Rows[2]["FFname"] = "Permit #/city & zip";
                        dtMailVersion.Rows[3]["FFname"] = "Postage Class";
                        dtMailVersion.Rows[4]["FFname"] = "Specialty Presort";
                        dtMailVersion.Rows[5]["FFname"] = "List Type";
                        dtMailVersion.Rows[6]["FFname"] = "Dedupe";
                        dtMailVersion.Rows[7]["FFname"] = "Surpression";
                        dtMailVersion.Rows[8]["FFname"] = "Move Update/NCOA";

                        dtMailVersion.Rows[9]["FFname"] = "Permit Type";
                        dtMailVersion.Rows[10]["FFname"] = "Permit Owner";
                        dtMailVersion.Rows[11]["FFname"] = "Permit #/city & zip";
                        dtMailVersion.Rows[12]["FFname"] = "Postage Class";
                        dtMailVersion.Rows[13]["FFname"] = "Specialty Presort";
                        dtMailVersion.Rows[14]["FFname"] = "List Type";
                        dtMailVersion.Rows[15]["FFname"] = "Dedupe";
                        dtMailVersion.Rows[16]["FFname"] = "Surpression";
                        dtMailVersion.Rows[17]["FFname"] = "Move Update/NCOA";
                    }
                    catch (Exception ex) { }

                    //copy DT and pop off first nine
                    DataTable dtMailV2 = dtMailVersion.Copy();

                    try
                    {
                        //first suppress from back 17-14
                        dtMailV2.Rows[17].Delete();
                        dtMailV2.Rows[16].Delete();
                        dtMailV2.Rows[15].Delete();
                        dtMailV2.Rows[14].Delete();
                        dtMailV2.Rows[0].Delete();
                        dtMailV2.Rows[1].Delete();
                        dtMailV2.Rows[2].Delete();
                        dtMailV2.Rows[3].Delete();
                        dtMailV2.Rows[4].Delete();
                        dtMailV2.Rows[5].Delete();
                        dtMailV2.Rows[6].Delete();
                        dtMailV2.Rows[7].Delete();
                        dtMailV2.Rows[8].Delete();
                        dtMailV2.AcceptChanges();

                        //remove last 9 from dtMailversion
                        dtMailVersion.Rows[5].Delete();
                        dtMailVersion.Rows[6].Delete();
                        dtMailVersion.Rows[7].Delete();
                        dtMailVersion.Rows[8].Delete();
                        dtMailVersion.Rows[9].Delete();
                        dtMailVersion.Rows[10].Delete();
                        dtMailVersion.Rows[11].Delete();
                        dtMailVersion.Rows[12].Delete();
                        dtMailVersion.Rows[13].Delete();
                        dtMailVersion.Rows[14].Delete();
                        dtMailVersion.Rows[15].Delete();
                        dtMailVersion.Rows[16].Delete();
                        dtMailVersion.Rows[17].Delete();
                        dtMailVersion.AcceptChanges();
                    }
                    catch (Exception ex) { }

                    //load the subReports with both tables dtJobMailingVersion and dtMailVersion
                    //cannot can only load one at a time, have to make the version info its own subrpt
                    cryrpt.Subreports["subMailVersion"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subMailVersion"].SetDataSource(dtMailVersion);


                    cryrpt.Subreports["subMailVersion2"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subMailVersion2"].SetDataSource(dtMailV2);



                }//end how many versions check (if >9)

                else
                {
                    try
                    {
                        dtMailVersion.Rows[0]["FFname"] = "Permit Type";
                        dtMailVersion.Rows[1]["FFname"] = "Permit Owner";
                        dtMailVersion.Rows[2]["FFname"] = "Permit #/city & zip";
                        dtMailVersion.Rows[3]["FFname"] = "Postage Class";
                        dtMailVersion.Rows[4]["FFname"] = "Specialty Presort";
                        dtMailVersion.Rows[5]["FFname"] = "List Type";
                        dtMailVersion.Rows[6]["FFname"] = "Dedupe";
                        dtMailVersion.Rows[7]["FFname"] = "Surpression";
                        dtMailVersion.Rows[8]["FFname"] = "Move Update/NCOA";
                    }
                    catch (Exception ex) { }

                    //first mail version subrpt
                    cryrpt.Subreports["subMailVersion"].DataSourceConnections.Clear();
                    cryrpt.Subreports["subMailVersion"].SetDataSource(dtMailVersion);

                    string subMailVersionFF2 = "subMailVersion2";
                    HideSubs(cryrpt, subMailVersionFF2);
                }

            }//end intial check if no records then hide both sub reports
            else
            {

                string subMailVersionFF = "subMailVersion";
                HideSubs(cryrpt, subMailVersionFF);

                //hide second one as well
                string subMailVersionFF2 = "subMailVersion2";
                HideSubs(cryrpt, subMailVersionFF2);

            }//this else if no mailign versions found


            #endregion Mailing Version subReport

            #region Mailing Free Fields subReport
            string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtMailFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);
                adapMailFF.Fill(dtMailFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing FF report";

                ErrorLog(error);
            }

            //running totals
          //  recordCount += dtMailFF.Rows.Count;
           // itemCount += dtMailFF.Columns.Count;

            //current DT
          //  recordCurrentCount += dtMailFF.Rows.Count;
          //  itemCurrentCount += dtMailFF.Columns.Count;
            // Console.WriteLine("mail free fields subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
           // sqlCount++;
           /// recordCurrentCount = 0;
          //  itemCurrentCount = 0;

            //and change second column name to match CR report value "FFvalue"
            dtMailFF.Columns["Free-Field-Char"].ColumnName = "FFval";
            dtMailFF.Columns.Add("FFname");

            try
            {
                dtMailFF.Rows[0]["FFname"] = "Mailpeice Size";
                dtMailFF.Rows[1]["FFname"] = "Customer Sign-Offs";
                dtMailFF.Rows[2]["FFname"] = "Inserting";
                dtMailFF.Rows[3]["FFname"] = "Tab/Seal/Glue";
                dtMailFF.Rows[4]["FFname"] = "Samples for Postage";
            }
            catch (Exception ex) { }
            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailFF.Rows.Count != 0)
            {
                cryrpt.Subreports["subMailFreeFields"].DataSourceConnections.Clear();
                cryrpt.Subreports["subMailFreeFields"].SetDataSource(dtMailFF);

            }
            else
            {

                string subMailFF = "subMailFreeFields";
                HideSubs(cryrpt, subMailFF);

            }
            // Console.WriteLine("Time for Mailing free fields subrpt" + DateTime.Now.ToString());
            #endregion Mailing Free Fields subReport

            #region Job Notes subReport
            string queryJobNotes = "SELECT \"SpecCategory-ID\", Description, \"Created-By\", \"Comment-Date\", \"Update-date\" FROM PUB.JobComments WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtJobNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobNotes = new OdbcDataAdapter(queryJobNotes, dbConn);
                adapJobNotes.Fill(dtJobNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Job Notes report";

                ErrorLog(error);
            }

            //running totals
           // recordCount += dtJobNotes.Rows.Count;
           // itemCount += dtJobNotes.Columns.Count;

            //current DT
           // recordCurrentCount += dtJobNotes.Rows.Count;
           // itemCurrentCount += dtJobNotes.Columns.Count;
            // Console.WriteLine("JobNotes subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
           // sqlCount++;
          //  recordCurrentCount = 0;
           // itemCurrentCount = 0;

            //change the names
            dtJobNotes.Columns["SpecCategory-ID"].ColumnName = "SpecID";
            dtJobNotes.Columns["Created-By"].ColumnName = "EnterBy";
            dtJobNotes.Columns["Comment-Date"].ColumnName = "DateEntered";
            dtJobNotes.Columns["Update-date"].ColumnName = "DateUpdated";




            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtJobNotes.Rows.Count != 0)
            {
                cryrpt.Subreports["subJobNotes"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobNotes"].SetDataSource(dtJobNotes);

            }
            else
            {

                string subJobComments = "subJobNotes";
                HideSubs(cryrpt, subJobComments);

            }

            // Console.WriteLine("Time for Job Notes subrpt" + DateTime.Now.ToString());
            #endregion Job Notes subReports


            #region Alterations subRpt
            string queryAlt = "SELECT * FROM PUB.Alterations WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtAlt = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapAlt = new OdbcDataAdapter(queryAlt, dbConn);
                adapAlt.Fill(dtAlt);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Alterations report";

                ErrorLog(error);
            }

            //running totals
          //  recordCount += dtAlt.Rows.Count;
           // itemCount += dtAlt.Columns.Count;
           // sqlCount++;
            //current DT
          //  recordCurrentCount += dtAlt.Rows.Count;
          //  itemCurrentCount += dtAlt.Columns.Count;
            // Console.WriteLine("Alterations subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);

           // recordCurrentCount = 0;
           // itemCurrentCount = 0;

            //also check if empty exists it is empty hideSubs it
            if (dtAlt.Rows.Count != 0)
            {
                cryrpt.Subreports["subAlterations"].DataSourceConnections.Clear();
                cryrpt.Subreports["subAlterations"].SetDataSource(dtAlt);

            }
            else
            {

                string subAlt = "subAlterations";
                HideSubs(cryrpt, subAlt);

            }
            // Console.WriteLine("Time for alteration subrpt" + DateTime.Now.ToString());
            #endregion alterations subRpt

            #region Job free fields subRPt
            string queryFF = "SELECT \"Free-Field-Char\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);
                adapFF.Fill(dtFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Free fields report";

                ErrorLog(error);
            }

            //running totals
           // recordCount += dtFF.Rows.Count;
           // itemCount += dtFF.Columns.Count;

            //current DT
           // recordCurrentCount += dtFF.Rows.Count;
           // itemCurrentCount += dtFF.Columns.Count;
            //  Console.WriteLine("Job free fields subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
           // sqlCount++;
          //  recordCurrentCount = 0;
           // itemCurrentCount = 0;

            //here is where i need to convert decimal -> free-fieldchar
            try
            {
                string lastJob = dtFF.Rows[1]["Free-Field-Decimal"].ToString();
                dtFF.Rows[1]["Free-Field-Char"] = lastJob;

                dtFF.Columns.Remove("Free-Field-Decimal");

                dtFF.Columns.Add("FFname");
                dtFF.Rows[0]["FFname"] = "Customer To Supply";
                dtFF.Rows[1]["FFname"] = "Last Job#";
                dtFF.Rows[2]["FFname"] = "Type of Proof Needed";
                dtFF.Rows[3]["FFname"] = "Proof Needed By";
                dtFF.Rows[4]["FFname"] = "Coating";
                dtFF.Rows[5]["FFname"] = "Perf Score";
                dtFF.Rows[6]["FFname"] = "Certification";
                dtFF.Rows[7]["FFname"] = "Mailing Services";
                dtFF.Rows[8]["FFname"] = "Litho/Digital/DSF/ASI";
                dtFF.Rows[9]["FFname"] = "Samples";

                CrystalDecisions.CrystalReports.Engine.TextObject txtObjSample;
                txtObjSample = cryrpt.ReportDefinition.ReportObjects["txtSample"] as TextObject;

                if (dtFF.Rows[9]["Free-Field-Char"].ToString() != "None")
                {
                    txtObjSample.Text = "";
                    txtObjSample.Border.BorderColor = Color.Orange;
                    txtObjSample.Text = dtFF.Rows[9]["Free-Field-Char"].ToString();
                }
                else
                {

                    txtObjSample.Border.LeftLineStyle = 0;
                    txtObjSample.Border.RightLineStyle = 0;
                    txtObjSample.Border.TopLineStyle = 0;
                    txtObjSample.Border.BottomLineStyle = 0;
                    txtObjSample.Text = "";
                }
            }
            catch (Exception ex) { }
            //also check if empty exists it is empty hideSubs it
            if (dtFF.Rows.Count != 0)
            {


                if (dtFF.Rows[0]["Free-Field-Char"].ToString() == "DSF")
                {

                    CrystalDecisions.CrystalReports.Engine.TextObject txtDSF;
                    txtDSF = cryrpt.ReportDefinition.ReportObjects["txtDSF"] as TextObject;
                    txtDSF.Text = "DSF";

                }

                cryrpt.Subreports["subJobFreeFields"].DataSourceConnections.Clear();
                cryrpt.Subreports["subJobFreeFields"].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subJobFreeFields";
                HideSubs(cryrpt, subFF);

            }

            //Console.WriteLine("Time for Job Free fields subrpt" + DateTime.Now.ToString());
            #endregion job free fields subRpt

            #region PO req subRpt
            string queryPOreq = "SELECT \"Req-Number\", \"Supplier-ID\", \"Supplier-Name\", \"Inventory-Item-ID\", \"Item-Desc\", \"Qty-Requisitioned\", \"Unit-Meas-Purchase\", \"Unit-Cost-Purchase\", \"Req-Status\" FROM PUB.PORequisition WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtPOreq = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOreq = new OdbcDataAdapter(queryPOreq, dbConn);
                adapPOreq.Fill(dtPOreq);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Req report";

                ErrorLog(error);
            }

            //running totals
            //recordCount += dtPOreq.Rows.Count;
           // itemCount += dtPOreq.Columns.Count;

            //current DT
           // recordCurrentCount += dtPOreq.Rows.Count;
           //itemCurrentCount += dtPOreq.Columns.Count;
            //  Console.WriteLine("PO Req subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
           // sqlCount++;
           // recordCurrentCount = 0;
            //itemCurrentCount = 0;

            //also check if empty exists it is empty hideSubs it
            if (dtPOreq.Rows.Count != 0)
            {
                cryrpt.Subreports["subPOReq"].DataSourceConnections.Clear();
                cryrpt.Subreports["subPOReq"].SetDataSource(dtPOreq);

            }
            else
            {

                string subPOreq = "subPOReq";
                HideSubs(cryrpt, subPOreq);

            }
            //  Console.WriteLine("Time for PO req subrpt" + DateTime.Now.ToString());
            #endregion PO req subRpt


            #region PO line info subRpt
            string queryPOLine = "SELECT \"PO-Number\", \"Line-Status\", \"Date-PO\", \"Date-Last-Receipt\", \"Item-Desc\", \"Qty-Received-Purchase\" FROM PUB.POLine WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtPOLine = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOLine = new OdbcDataAdapter(queryPOLine, dbConn);
                adapPOLine.Fill(dtPOLine);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Line report";

                ErrorLog(error);
            }
            //running totals
           // recordCount += dtPOLine.Rows.Count;
           // itemCount += dtPOLine.Columns.Count;

            //current DT
          //  recordCurrentCount += dtPOLine.Rows.Count;
          //  itemCurrentCount += dtPOLine.Columns.Count;
            // Console.WriteLine("PO Line subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
          //  sqlCount++;
           // recordCurrentCount = 0;
           // itemCurrentCount = 0;
            //also check if empty exists it is empty hideSubs it
            if (dtPOLine.Rows.Count != 0)
            {
                cryrpt.Subreports["subPOLine"].DataSourceConnections.Clear();
                cryrpt.Subreports["subPOLine"].SetDataSource(dtPOLine);

            }
            else
            {

                string subPOLine = "subPOLine";
                HideSubs(cryrpt, subPOLine);

            }
            // Console.WriteLine("Time for PO line subrpt" + DateTime.Now.ToString());
            #endregion Po Line info Subrpt

            #region ship to data subRpt
            string queryShipTo = "SELECT \"Ship-To-Name\", \"Ship-To-Address1\", \"Ship-To-Address2\", \"Ship-To-Address3\", \"Ship-To-City\", \"Ship-To-State\",\"Ship-To-Zip\",\"Ship-To-Attention\"" +
                ",\"Instructions\", \"Requested-Ship-Date\" ,\"Requested-Quantity\", \"Requested-Number-Packages\", FAX, Phone, \"Ship-To-Country\", \"Shipment-Method-ID\"  FROM PUB.JobShipTo WHERE \"Job-ID\" = " + jobNumber;

            DataTable dtShipTo = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapShipTo = new OdbcDataAdapter(queryShipTo, dbConn);
                adapShipTo.Fill(dtShipTo);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -ship to report";

                ErrorLog(error);
            }
            //running totals
           // recordCount += dtShipTo.Rows.Count;
          //  itemCount += dtShipTo.Columns.Count;

            //current DT
           // recordCurrentCount += dtShipTo.Rows.Count;
          //  itemCurrentCount += dtShipTo.Columns.Count;
            //  Console.WriteLine("Ship to subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
           // sqlCount++;
           // recordCurrentCount = 0;
           // itemCurrentCount = 0;

            //also check if empty exists it is empty hideSubs it
            if (dtShipTo.Rows.Count != 0)
            {
                cryrpt.Subreports["subShipTo"].DataSourceConnections.Clear();
                cryrpt.Subreports["subShipTo"].SetDataSource(dtShipTo);

            }
            else
            {

                string subShipTo = "subShipTo";
                HideSubs(cryrpt, subShipTo);

            }
            //  Console.WriteLine("Time for ship to data subrpt" + DateTime.Now.ToString());
            #endregion ship to data subRpt

            #region form spec subRpt
            string querySpecForm = "SELECT \"Form-ID\",  \"Form-Desc\", \"Number-Pages\", Width, Length, \"Number-forms\", \"Number-Out\", Imposition, Notes FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtSpecForm = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapSpecForm = new OdbcDataAdapter(querySpecForm, dbConn);
                adapSpecForm.Fill(dtSpecForm);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Spec Form report";

                ErrorLog(error);
            }
            //running totals
            //recordCount += dtSpecForm.Rows.Count;
           // itemCount += dtSpecForm.Columns.Count;

            //current DT
          //  recordCurrentCount += dtSpecForm.Rows.Count;
           // itemCurrentCount += dtSpecForm.Columns.Count;
            // Console.WriteLine("job specs subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
          //  sqlCount++;
          //  recordCurrentCount = 0;
          //  itemCurrentCount = 0;

            //also check if empty exists it is empty hideSubs it
            if (dtSpecForm.Rows.Count != 0)
            {
                cryrpt.Subreports["subFormSpecs"].DataSourceConnections.Clear();
                cryrpt.Subreports["subFormSpecs"].SetDataSource(dtSpecForm);

            }
            else
            {

                string subFormSpec = "subFormSpecs";
                HideSubs(cryrpt, subFormSpec);

            }

            // Console.WriteLine("Time for form specs subrpt " + DateTime.Now.ToString());
            #endregion form spec subRpt

            #region form Notes subrpt
            string queryFormNotes = "SELECT \"Form-ID\", Description FROM PUB.JobComments WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtFormNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFormNotes = new OdbcDataAdapter(queryFormNotes, dbConn);
                adapFormNotes.Fill(dtFormNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Form Notes report";

                ErrorLog(error);
            }
            //running totals
           // recordCount += dtFormNotes.Rows.Count;
            //itemCount += dtFormNotes.Columns.Count;

            //current DT
           // recordCurrentCount += dtFormNotes.Rows.Count;
           // itemCurrentCount += dtFormNotes.Columns.Count;
            // Console.WriteLine("Form notes subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
            //sqlCount++;
          //  recordCurrentCount = 0;
           // itemCurrentCount = 0;

            //also check if empty exists it is empty hideSubs it
            if (dtFormNotes.Rows.Count != 0)
            {
                cryrpt.Subreports["subFormNotes"].DataSourceConnections.Clear();
                cryrpt.Subreports["subFormNotes"].SetDataSource(dtFormNotes);

            }
            else
            {

                string subFormNotes = "subFormNotes";
                HideSubs(cryrpt, subFormNotes);

            }

            // Console.WriteLine("Time for form notes subrpt " + DateTime.Now.ToString());
            #endregion form notes subRpt

            #region prepress rpt
            string queryPrepress = "SELECT \"Form-ID\", \"Spec-Desc\", \"Work-Center-ID\", \"Operation-ID\", Quantity, Hours FROM PUB.JobSpecPrepress WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtPrepress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPrepress = new OdbcDataAdapter(queryPrepress, dbConn);
                adapPrepress.Fill(dtPrepress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Prepress report";

                ErrorLog(error);
            }
            //running totals
           // recordCount += dtPrepress.Rows.Count;
          //  itemCount += dtPrepress.Columns.Count;

            //current DT
           // recordCurrentCount += dtPrepress.Rows.Count;
          //  itemCurrentCount += dtPrepress.Columns.Count;
            //  Console.WriteLine("Prepresssubrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
         //   sqlCount++;
          //  recordCurrentCount = 0;
          //  itemCurrentCount = 0;

            //also check if empty exists it is empty hideSubs it
            if (dtPrepress.Rows.Count != 0)
            {
                cryrpt.Subreports["subPrepress"].DataSourceConnections.Clear();
                cryrpt.Subreports["subPrepress"].SetDataSource(dtPrepress);

            }
            else
            {

                string subPrepress = "subPrepress";
                HideSubs(cryrpt, subPrepress);

            }
            // Console.WriteLine("Time for prepress subrpt " + DateTime.Now.ToString());
            #endregion prepress rpt

            #region press rpt
            string queryPress = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtPress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPress = new OdbcDataAdapter(queryPress, dbConn);
                adapPress.Fill(dtPress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Press report";
                ErrorLog(error);
            }
            //running totals
           // recordCount += dtPress.Rows.Count;
           // itemCount += dtPress.Columns.Count;

            //current DT
          //  recordCurrentCount += dtPress.Rows.Count;
          //  itemCurrentCount += dtPress.Columns.Count;
            // Console.WriteLine("Press subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
          //  sqlCount++;
          //  recordCurrentCount = 0;
          //  itemCurrentCount = 0;
            //also check if empty exists it is empty hideSubs it
            if (dtPress.Rows.Count != 0)
            {
                cryrpt.Subreports["subPress"].DataSourceConnections.Clear();
                cryrpt.Subreports["subPress"].SetDataSource(dtPress);

            }
            else
            {

                string subPress = "subPress";
                HideSubs(cryrpt, subPress);

            }
            // Console.WriteLine("Time for press subrpt " + DateTime.Now.ToString());
            #endregion press rpt

            #region stock rpt
            string queryStock = "SELECT \"Form-ID\", \"Gross-Sheets\", \"Gross-Weight\",  Notes,  \"Inventory-Item-ID\", \"Inventory-Item-Desc\", \"Material-Type\", Length, Width FROM PUB.JobSpecMaterial WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtStock = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapStock = new OdbcDataAdapter(queryStock, dbConn);
                adapStock.Fill(dtStock);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report";

                ErrorLog(error);
            }

            //running totals
           // recordCount += dtStock.Rows.Count;
          //  itemCount += dtStock.Columns.Count;

            //current DT
           // recordCurrentCount += dtStock.Rows.Count;
          //  itemCurrentCount += dtStock.Columns.Count;
            // Console.WriteLine("stock subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
          //  sqlCount++;
          //  recordCurrentCount = 0;
          //  itemCurrentCount = 0;

            //need to get page-grain new query
            string queryPageGrain = "SELECT \"Page-Grain\" FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtPG = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPG = new OdbcDataAdapter(queryPageGrain, dbConn);
                adapPG.Fill(dtPG);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report -PG";

                ErrorLog(error);
            }
            //running totals
         //   recordCount += dtPG.Rows.Count;
         //   itemCount += dtPG.Columns.Count;

            //current DT
         //   recordCurrentCount += dtPG.Rows.Count;
          //  itemCurrentCount += dtPG.Columns.Count;
           //  Console.WriteLine("Stock subrpt - PageGrain field - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
           // sqlCount++;
          //  recordCurrentCount = 0;
           // itemCurrentCount = 0;


            dtStock.Columns.Add("Page-Grain");

            try
            {
                string val = dtPG.Rows[0]["Page-Grain"].ToString();

                foreach (DataRow dr in dtStock.Rows)
                {

                    dr["Page-Grain"] = val;

                }
            }
            catch (Exception ex) { }

            //also check if empty exists it is empty hideSubs it
            if (dtStock.Rows.Count != 0)
            {
                cryrpt.Subreports["subStock"].DataSourceConnections.Clear();
                cryrpt.Subreports["subStock"].SetDataSource(dtStock);

            }
            else
            {

                string subStock = "subStock";
                HideSubs(cryrpt, subStock);

            }
            // Console.WriteLine("Time for stock subrpt+PageGrain feild query " + DateTime.Now.ToString());
            #endregion stock rpt

            #region bindery rpt
            string queryBind = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\", \"Number-Out-for-Oper\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtBind = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapBind = new OdbcDataAdapter(queryBind, dbConn);
                adapBind.Fill(dtBind);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Bindery report";

                ErrorLog(error);
            }

            //running totals
            //recordCount += dtBind.Rows.Count;
           // itemCount += dtBind.Columns.Count;

            //current DT
           // recordCurrentCount += dtBind.Rows.Count;
          //  itemCurrentCount += dtBind.Columns.Count;
            //  Console.WriteLine("Bindery subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
            //sqlCount++;
           // recordCurrentCount = 0;
           // itemCurrentCount = 0;

            //also check if empty exists it is empty hideSubs it
            if (dtBind.Rows.Count != 0)
            {
                cryrpt.Subreports["subBindery"].DataSourceConnections.Clear();
                cryrpt.Subreports["subBindery"].SetDataSource(dtBind);

            }
            else
            {

                string subBindery = "subBindery";
                HideSubs(cryrpt, subBindery);

            }

            //  Console.WriteLine("Time for bindery subrpt " + DateTime.Now.ToString());
            #endregion bindery rpt

            #region bindery matts rpt
            string queryBindMatts = "SELECT \"Form-ID\", \"Material-Type\",  \"Inventory-Item-ID\", \"Qty-Gross-Costing\", \"Qty-Net-Costing\", \"Inventory-Item-Desc\" FROM PUB.JobSpecMaterial WHERE \"Job-ID\" =" + jobNumber;
            DataTable dtBindMatts = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapBindMatts = new OdbcDataAdapter(queryBindMatts, dbConn);
                adapBindMatts.Fill(dtBindMatts);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Bindery Matts report";

                ErrorLog(error);
            }

            //running totals
          //  recordCount += dtBindMatts.Rows.Count;
          //  itemCount += dtBindMatts.Columns.Count;

            //current DT
          //  recordCurrentCount += dtBindMatts.Rows.Count;
          //  itemCurrentCount += dtBindMatts.Columns.Count;
            //  Console.WriteLine("Bindery Matts subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
          //  sqlCount++;
           // recordCurrentCount = 0;
           // itemCurrentCount = 0;

            //also check if empty exists it is empty hideSubs it
            if (dtBindMatts.Rows.Count != 0)
            {
                cryrpt.Subreports["subBinderyMatts"].DataSourceConnections.Clear();
                cryrpt.Subreports["subBinderyMatts"].SetDataSource(dtBindMatts);

            }
            else
            {

                string subBinderyMatts = "subBinderyMatts";
                HideSubs(cryrpt, subBinderyMatts);

            }
            // Console.WriteLine("Time for bindery matts subrpt" + DateTime.Now.ToString());
            #endregion bindery matts rpt

            #region 810 notes
            string query810Notes = "SELECT \"Work-Center-ID\", Notes FROM PUB.ScheduleByJob WHERE \"Job-ID\" = " + jobNumber;

            DataTable dt810Notes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810 = new OdbcDataAdapter(query810Notes, dbConn);
                adap810.Fill(dt810Notes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - 810 notes report";

                ErrorLog(error);
            }
            //running totals
          //  recordCount += dt810Notes.Rows.Count;
          //  itemCount += dt810Notes.Columns.Count;

            //current DT
          //  recordCurrentCount += dt810Notes.Rows.Count;
           // itemCurrentCount += dt810Notes.Columns.Count;
            //  Console.WriteLine("810 Notes subrpt - This* sql statment/Datatable fill - Record Row Current: " + recordCurrentCount + " Elements current " + itemCurrentCount + "\nRunning Total:Record Rows: " + recordCount + " Elements per row: " + itemCount);
          //  sqlCount++;
            //  Console.WriteLine("Sql query count: "+sqlCount);
          //  recordCurrentCount = 0;
          //  itemCurrentCount = 0;

            //filter out all work center id's except 810
            for (int i = dt810Notes.Rows.Count - 1; i >= 0; i--)
            {

                DataRow dr = dt810Notes.Rows[i];
                if (!dr["Work-Center-ID"].ToString().Contains("810"))
                {
                    dr.Delete();
                }

            }
            dt810Notes.AcceptChanges();

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dt810Notes.Rows.Count != 0)
            {

                cryrpt.Subreports["sub810Notes"].DataSourceConnections.Clear();
                cryrpt.Subreports["sub810Notes"].SetDataSource(dt810Notes);

            }
            else
            {

                string sub810Notes = "sub810Notes";
                HideSubs(cryrpt, sub810Notes);

            }
            // Console.WriteLine("Time for 810 atg notes subrpt" + DateTime.Now.ToString());
            #endregion 810 notes

            #region estimateDetail

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;

            DataTable estimateIDTbl = new DataTable();
            DataTable estDetailLines = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId = new OdbcDataAdapter(estimateID, dbConn);
                adapEstId.Fill(estimateIDTbl);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl.Rows.Count == 0) { }

            else
            {

                string eID = estimateIDTbl.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines = new OdbcDataAdapter(estDetail, dbConn);
                    adapEstIdLines.Fill(estDetailLines);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines.Rows[i][1].ToString() == "" || estDetailLines.Rows[i][1].ToString() == "Main Component" || estDetailLines.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines.AcceptChanges();

                cryrpt.Subreports[4].DataSourceConnections.Clear();
                cryrpt.Subreports[4].SetDataSource(estDetailLines);

            }
            else
            {

                string subEstDetail = "subEstimateDetails";
                HideSubs(cryrpt, subEstDetail);

            }


            #endregion

            #region estimateDetail2

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID2 = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;

            DataTable estimateIDTbl2 = new DataTable();
            DataTable estDetailLines2 = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId2 = new OdbcDataAdapter(estimateID2, dbConn);
                adapEstId2.Fill(estimateIDTbl2);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl2.Rows.Count == 0) { }

            else
            {

                string eID2 = estimateIDTbl2.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail2 = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID2;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines2 = new OdbcDataAdapter(estDetail2, dbConn);
                    adapEstIdLines2.Fill(estDetailLines2);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines2.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines2.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines2.Rows[i][1].ToString() == "" || estDetailLines2.Rows[i][1].ToString() == "Main Component" || estDetailLines2.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines2.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines2.AcceptChanges();
                cryrpt.Subreports[5].DataSourceConnections.Clear();
                cryrpt.Subreports[5].SetDataSource(estDetailLines2);

            }
            else
            {

                string subEstDetail2 = "subEstimateDetails2";
                HideSubs(cryrpt, subEstDetail2);

            }


            #endregion

            #region estimateDetail3

            //need to grab the estimate detail, gunna need to do some manipulation to grab that estimateID
            string estimateID3 = "SELECT * FROM PUB.JobEstMerge WHERE \"Job-ID\" = " + jobNumber;
            DataTable estimateIDTbl3 = new DataTable();
            DataTable estDetailLines3 = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapEstId3 = new OdbcDataAdapter(estimateID3, dbConn);
                adapEstId3.Fill(estimateIDTbl3);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                ErrorLog(error);
            }

            //if empty do ntohing else, new query for that estimate ID
            if (estimateIDTbl3.Rows.Count == 0) { }

            else
            {

                string eID3 = estimateIDTbl3.Rows[0]["Estimate-ID"].ToString();

                //new query for that estimate id
                string estDetail3 = "SELECT \"Segment-ID\", Description FROM PUB.Segment WHERE \"Estimate-ID\" = " + eID3;

                try //to sql and fill adapter and DT
                {
                    OdbcDataAdapter adapEstIdLines3 = new OdbcDataAdapter(estDetail3, dbConn);
                    adapEstIdLines3.Fill(estDetailLines3);
                }
                catch (Exception ex)
                {

                    string error = ex + " : SQL error cannot load OdbcDataAdapter - estimate detail report";

                    ErrorLog(error);
                }

                //when two zero's are found replace with "-"  -> maybe

            }//end else

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (estDetailLines3.Rows.Count != 0)
            {

                //get rid of empty rows
                for (int i = estDetailLines3.Rows.Count - 1; i >= 0; i--)
                {

                    if (estDetailLines3.Rows[i][1].ToString() == "" || estDetailLines3.Rows[i][1].ToString() == "Main Component" || estDetailLines3.Rows[i][1].ToString() == "Finishing")
                    {


                        estDetailLines3.Rows[i].Delete();

                    }


                }//remove whitespace

                estDetailLines3.AcceptChanges();

                cryrpt.Subreports[6].DataSourceConnections.Clear();
                cryrpt.Subreports[6].SetDataSource(estDetailLines3);

            }
            else
            {

                string subEstDetail3 = "subEstimateDetails3";
                HideSubs(cryrpt, subEstDetail3);

            }


            #endregion

            #endregion sub report creation


            dbConn.Close();
            #endregion DB close connection



            #region display rpt

            //surpress billing section
            cryrpt.ReportDefinition.Sections["billingSection"].SectionFormat.EnableSuppress = true;

            CrystalReportViewer1.ReportSource = cryrpt;
            CrystalReportViewer1.RefreshReport();

            cryrpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "VVT Report");


        }
        #endregion Full ticket

    }

}

