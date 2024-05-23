using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.VisualBasic;
using System.IO;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing.Text;
using System.Net.Mail;
using System.Net;
using System.Data;

namespace VVT.ASP.NETv2
{
    public partial class Estimator : System.Web.UI.Page
    {


        //globals
        DataTable newTbl = new DataTable();
        DataTable dtTest = new DataTable();
        DataTable salesTbl = new DataTable();

        DataTable custTbl = new DataTable();

        DataTable csrNames = new DataTable();
        List<string> csrList = new List<string>();

        List<string> emailList = new List<string>();

        MailMessage mail = new MailMessage();
        SmtpClient SmtpServer = new SmtpClient("smtp.visographic.com");

        List<string> list = new List<string>();

        List<string> list2 = new List<string>();

        string visoMail = "";

        string mailBuilder = "";

        protected void Page_Load(object sender, EventArgs e)
        {

            //fill listboxes with datatables
            //string to open connection to DB
            string connectString = "DSN=Progress11;uid=bob;pwd=Orchard";


            //try inserting record row into dataset2
            using (OdbcConnection dbConn = new OdbcConnection(connectString))
            {

                try  //to open connection
                {
                    dbConn.Open();

                }
                catch (Exception)
                {
                    Console.WriteLine("No connect");
                }


                //is connection open? T/F
                Console.WriteLine("Connection is: " + dbConn.State);

                //dispaly info about the connection
                Console.WriteLine("Connection Information:");
                Console.WriteLine("\tConnection String:" +
                                  dbConn.ConnectionString);
                Console.WriteLine("\tConnection Timeout:" +
                                  dbConn.ConnectionTimeout);
                Console.WriteLine("\tDatabase:" +
                                  dbConn.Database);
                Console.WriteLine("\tDataSource:" +
                                  dbConn.DataSource);
                Console.WriteLine("\tDriver:" +
                                  dbConn.Driver);
                Console.WriteLine("\tServerVersion:" +
                                  dbConn.ServerVersion);


                //yes and nos drop downs {4,11,12}
                DropDownList4.Items.Add("Yes");
                DropDownList4.Items.Add("No");
                DropDownList4.DataBind();

                DropDownList11.Items.Add("Yes");
                DropDownList11.Items.Add("No");
                DropDownList11.DataBind();

                DropDownList12.Items.Add("Yes");
                DropDownList12.Items.Add("No");
                DropDownList12.DataBind();


                FillSalesClassIDDropDownLists(dbConn);
                FillCustomerIDandNameDropDownLists(dbConn);
                FillSalesAgentDropDownLists(dbConn);
                FillEmailDropDownLists(dbConn);
            }//end connection
        }//end page load




        //functions

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

        }//end error log

        //fill sales class id dropdowns
        public void FillSalesClassIDDropDownLists(OdbcConnection dbConn)
        {

            //string to query
            string que = "SELECT * FROM PUB.SalesClass";

            //fill Adapter
            OdbcDataAdapter adapSales = new OdbcDataAdapter(que, dbConn);
            //fill datatable
            adapSales.Fill(salesTbl);

            list = salesTbl.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("Sales-Class-Desc")).ToList();

            //Sales Class ID 1 dropDownList
            DropDownList5.DataSource = list;
            DropDownList5.SelectedIndex = -1;
            DropDownList5.DataBind();

            //Sales Class ID 2 dropDownList
            DropDownList6.DataSource = list;
            DropDownList6.DataBind();

            //Sales Class ID 3 dropDownList
            DropDownList7.DataSource = list;
            DropDownList7.DataBind();

            //Sales Class ID 4 dropDownList
            DropDownList8.DataSource = list;
            DropDownList8.DataBind();
        }//end fill sales class ID

        //fill customer name and id drop down
        public void FillCustomerIDandNameDropDownLists(OdbcConnection dbConn)
        {

            String queryTest3 = "SELECT * FROM PUB.cust WHERE \"System-ID\" = \'Viso\'";


            OdbcDataAdapter adapTest = new OdbcDataAdapter(queryTest3, dbConn);

            //fill datatable
            adapTest.Fill(dtTest);


            newTbl = dtTest.Copy();
            newTbl.Columns.Add("thirdCol");

            foreach (DataRow dr in newTbl.Rows)
            {

                dr["thirdCol"] = dr["cust-name"].ToString() + " - " + dr["Cust-code"].ToString();

            }


            newTbl.AcceptChanges();
            //ID
            list = newTbl.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("cust-name")).ToList();
            DropDownList2.DataSource = list;
            DropDownList2.SelectedIndex = -1;
            DropDownList2.DataBind();
            //name
            list2 = newTbl.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("Cust-code")).ToList();
            DropDownList1.DataSource = list2;
            DropDownList1.SelectedIndex = -1;
            DropDownList1.DataBind();


        }//end customer name drop down fill


        public void FillSalesAgentDropDownLists(OdbcConnection dbConn)
        {
            //grab customer data
            String queryCSR = "SELECT \"Sales-Agent-Name\", \"Sales-agent-id\" FROM PUB.\"sales-agent\" WHERE \"Record-Active\" = " + 1 + " AND \"System-ID\"= \'Viso\'";


            OdbcDataAdapter adapCSR = new OdbcDataAdapter(queryCSR, dbConn);

            //fill datatable
            adapCSR.Fill(csrNames);

            csrList = csrNames.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("Sales-Agent-Name")).ToList();
            DropDownList3.DataSource = csrList;
            DropDownList3.DataBind();
        }//end sales agent dropdown

        public void FillEmailDropDownLists(OdbcConnection dbConn)
        {

            DataTable email = new DataTable();

            String queryEmail = "SELECT \"E-Mail-Address\" FROM PUB.\"sales-agent\" WHERE \"Record-Active\" =\'1\'";


            OdbcDataAdapter adapEmail = new OdbcDataAdapter(queryEmail, dbConn);

            //fill datatable
            adapEmail.Fill(email);

            emailList = email.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("E-Mail-Address")).ToList();

            email.Clear();

            //now get csr emails
            String queryEmailCSR = "SELECT \"E-Mail-Address\" FROM PUB.CSR WHERE \"Record-Active\" =\'1\'";


            OdbcDataAdapter adapEmailCSR = new OdbcDataAdapter(queryEmailCSR, dbConn);

            //fill datatable
            adapEmailCSR.Fill(email);

            foreach (DataRow dr in email.Rows)
            {

                emailList.Add(dr["E-Mail-Address"].ToString());

            }

            emailList = emailList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();


            List<string> emailList2 = new List<string>(emailList);
            emailList.Add("bobjr@visographic.com");
            emailList2.Add("bobjr@visographic.com");

            DropDownList9.DataSource = emailList;
            DropDownList9.SelectedIndex = -1;
            DropDownList9.DataBind();


            DropDownList10.DataSource = emailList2;
            DropDownList10.SelectedIndex = -1;
            DropDownList10.DataBind();


        }//end email drop down 

        //send email button
        protected void Button1_Click(object sender, EventArgs e)
        {

            bool sent = true;

            //customer id                                   //customer name                                 //sales person...
            if (DropDownList1.SelectedValue.ToString() == "" || DropDownList2.SelectedValue.ToString() == "" || DropDownList3.SelectedValue.ToString() == "" || TextBox13.Text.ToString() == "" || DropDownList5.SelectedValue.ToString() == "" || DropDownList9.SelectedValue.ToString() == "" || DropDownList10.SelectedValue.ToString() == "" || DropDownList4.SelectedValue.ToString() == "" || DropDownList11.SelectedValue.ToString() == "" || DropDownList12.SelectedValue.ToString() == "" || DropDownList5.SelectedValue.ToString() == "" || (CheckBox1.Checked == false && CheckBox2.Checked == false && CheckBox3.Checked == false) || TextBox2.Text.ToString() == "" || TextBox7.Text.ToString() == "" || TextBox12.Text.ToString() == "")
            {

                if (DropDownList1.SelectedValue.ToString() == "")
                {
                    Label21.Text = "- Email failed - Missing Customer ID";
                }

                if (DropDownList2.SelectedValue.ToString() == "")
                {
                    Label21.Text = "- Email failed - Missing Customer Name";
                }

                if (DropDownList3.SelectedValue.ToString() == "")
                {
                    Label21.Text = "- Email failed - Missing Sales Person";
                }
                if (TextBox13.Text.ToString() == "")
                {
                    Label21.Text = "- Email failed - Missing Short Description";
                }
                if (DropDownList5.SelectedValue.ToString() == "")
                {
                    Label21.Text = "- Email failed - Missing Sales Class ID 1";
                }
                if (DropDownList11.SelectedValue.ToString() == "")
                {
                    Label21.Text = "- Email failed - Missing FCS Yes/No";
                }
                if (DropDownList12.SelectedValue.ToString() == "")
                {
                    Label21.Text = "- Email failed - Missing DSF Yes/No";
                }
                if (TextBox2.Text.ToString() == "")
                {
                    Label21.Text = "- Email failed - Missing  Qty 1";
                }
                if (TextBox7.Text.ToString() == "")
                {
                    Label21.Text = "- Email failed - Missing Estimate Description";
                }
                if (TextBox12.Text.ToString() == "")
                {
                    Label21.Text = "- Email failed - Missing Subject Line";
                }

                if (CheckBox1.Checked == false && CheckBox2.Checked == false && CheckBox3.Checked == false)
                {
                    Label21.Text = "- Email failed - Missing Checkbox: New, Rerun or Rerun with changes";
                }

                if (DropDownList7.SelectedValue.ToString() == "")
                {
                    Label21.Text = "- Email failed - Missing From for email";
                }



            }

            else if ((CheckBox2.Checked || CheckBox3.Checked) && TextBox1.Text.ToString() == "") { Label21.Text = "- Email failed - Missing job number for rerun"; }

            else
            {

                try
                {

                    //checks went thru, send email
                    mail.IsBodyHtml = true;

                    mail.Subject = TextBox12.Text + " : " + TextBox13.Text.ToString();

                    //split to and from up

                    string toAddress = TextBox9.Text;

                    string[] toAdd = toAddress.Split(';');



                    if (toAdd.Length > 1)
                    {
                        for (int x = 0; x < toAdd.Length; x++)
                        {

                            mail.To.Add(toAdd[x]);


                        }
                    }
                    else
                    {

                        mail.To.Add(toAddress);

                    }



                    mailBuilder += "<b>" + DropDownList1.Text + " - " + DropDownList2.Text;
                    mailBuilder += "<br>" + DropDownList3.Text;


                    if (CheckBox1.Checked)
                    {

                        mailBuilder += "<br><br><font color=\"Fuchsia\">New, Rerun or Changes: New</font>";

                    }
                    if (CheckBox2.Checked)
                    {

                        mailBuilder += "<br><br><font color=\"Fuchsia\">New, Rerun or Changes: Rerun - Last Job #: " + TextBox1.Text + " </font>";

                    }

                    if (CheckBox3.Checked)
                    {

                        mailBuilder += "<br><br><font color=\"Fuchsia\">New, Rerun or Changes: Changes - Last Job #: " + TextBox1.Text + "</font>";

                    }

                    //viso to mail
                    if (DropDownList4.Text == "Yes")
                    {

                        mailBuilder += "<br><b><font color=\"Green\">Viso to mail: Yes</b></font>";

                    }
                    else
                    {

                        mailBuilder += "<br><br><b><font color=\"Green\">Viso to mail: No</b></font>";

                    }

                    //FSC info
                    if (DropDownList11.Text == "Yes")
                    {

                        mailBuilder += "<br><font color=\"Green\">FSC Information: Yes</font>";

                    }
                    else
                    {

                        mailBuilder += "<br><br><font color=\"Green\">FSC Information: No</font>";

                    }



                    if (DropDownList12.Text == "Yes")
                    {

                        mailBuilder += "<br><br><font color=\"DarkBlue\">DSF: Yes (No prepress and no delivery)</font>";

                    }

                    else
                    {

                        mailBuilder += "<br><br><font color=\"DarkBlue\">DSF: No</font>";

                    }


                    //sales id boxes 


                    mailBuilder += "<br><br>Sales Class ID: " + DropDownList5.Text;


                    if (DropDownList6.Text != "")
                    {
                        mailBuilder += "<br>Sales Class ID 2: " + DropDownList6.Text;
                    }

                    if (DropDownList7.Text != "")
                    {
                        mailBuilder += "<br>Sales Class ID 3: " + DropDownList7.Text;
                    }

                    if (DropDownList8.Text != "")
                    {
                        mailBuilder += "<br>Sales Class ID 4: " + DropDownList8.Text;
                    }


                    string mainNotes = "\n" + TextBox7.Text;

                    mainNotes = mainNotes.Replace("\n", "<br>");


                    //main body

                    mailBuilder += "<br><font color=\"Blue\">" + mainNotes + "</font>";




                    //have all 5 qty's
                    if (TextBox2.Text != "" && TextBox3.Text != "" && TextBox4.Text != "" && TextBox5.Text != "" && TextBox6.Text != "")
                    {

                        //format all quantities
                        string qty1Format = String.Format("{0:n0}", Convert.ToInt32(TextBox2.Text));
                        string qty2Format = String.Format("{0:n0}", Convert.ToInt32(TextBox3.Text));
                        string qty3Format = String.Format("{0:n0}", Convert.ToInt32(TextBox4.Text));
                        string qty4Format = String.Format("{0:n0}", Convert.ToInt32(TextBox5.Text));
                        string qty5Format = String.Format("{0:n0}", Convert.ToInt32(TextBox6.Text));


                        mailBuilder += "<br><br><font color=\"Fuchsia\">Quantities: " + qty1Format + " - " + qty2Format + " - " + qty3Format + " - " + qty4Format + " - " + qty5Format + "</font>";
                    }

                    //4 qty's
                    if (TextBox2.Text != "" && TextBox3.Text != "" && TextBox4.Text != "" && TextBox5.Text != "" && TextBox6.Text == "")
                    {


                        string qty1Format = String.Format("{0:n0}", Convert.ToInt32(TextBox2.Text));
                        string qty2Format = String.Format("{0:n0}", Convert.ToInt32(TextBox3.Text));
                        string qty3Format = String.Format("{0:n0}", Convert.ToInt32(TextBox4.Text));
                        string qty4Format = String.Format("{0:n0}", Convert.ToInt32(TextBox5.Text));


                        mailBuilder += "<br><br><font color=\"Fuchsia\">Quantities: " + qty1Format + " - " + qty2Format + " - " + qty3Format + " - " + qty4Format + "</font>";
                    }

                    //3 qty's
                    if (TextBox2.Text != "" && TextBox3.Text != "" && TextBox4.Text != "" && TextBox5.Text == "" && TextBox6.Text == "")
                    {

                        string qty1Format = String.Format("{0:n0}", Convert.ToInt32(TextBox2.Text));
                        string qty2Format = String.Format("{0:n0}", Convert.ToInt32(TextBox3.Text));
                        string qty3Format = String.Format("{0:n0}", Convert.ToInt32(TextBox4.Text));

                        mailBuilder += "<br><br><font color=\"Fuchsia\">Quantities: " + qty1Format + " - " + qty2Format + " - " + qty3Format + "</font>";
                    }

                    //2 qty's
                    if (TextBox2.Text != "" && TextBox3.Text != "" && TextBox4.Text == "" && TextBox5.Text == "" && TextBox6.Text == "")
                    {

                        string qty1Format = String.Format("{0:n0}", Convert.ToInt32(TextBox2.Text));
                        string qty2Format = String.Format("{0:n0}", Convert.ToInt32(TextBox3.Text));

                        mailBuilder += "<br><br><font color=\"Fuchsia\">Quantities: " + qty1Format + " - " + qty2Format + "</font>";
                    }

                    //1 qty's
                    if (TextBox2.Text != "" && TextBox3.Text == "" && TextBox4.Text == "" && TextBox5.Text == "" && TextBox6.Text == "")
                    {

                        string qty1Format = String.Format("{0:n0}", Convert.ToInt32(TextBox2.Text));

                        mailBuilder += "<br><br><font color=\"Fuchsia\">Quantities: " + qty1Format + "</font>";
                    }

                    if (DropDownList9.Text != "")
                    {
                        mail.CC.Add(DropDownList9.Text);
                    }


                    mail.Body = mailBuilder;

                    SmtpServer.Port = 2525;

                    //or 587 port number or 2525


                    //for whatever reason does not work sometimes?
                    //          kjacosben@visographic.com     wood234StockA2**
                    //          visoexpress@visographic.com     Visographic1

                    SmtpServer.Timeout = 50000;





                    //may neeed everyones emails and passwords
                    string fromAddress = DropDownList10.Text;
                    mail.From = new MailAddress(fromAddress.ToString());

                    SmtpServer.UseDefaultCredentials = false;

                    //if statements based on comboBox4

                    if (DropDownList10.Text == "emusial@visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("emusial@visographic.com", "Visographic1");
                    }

                    if (DropDownList10.Text == "bobjr@visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("bobjr@visographic.com", "Champion1987#");
                    }

                    if (DropDownList10.Text == "tpaolini@visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("tpaolini@visographic.com", "PRTnLAM22024!!");
                    }

                    if (DropDownList10.Text == "DNavigato@VISOgraphic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("DNavigato@VISOgraphic.com", "Navigato23!");
                    }


                    if (DropDownList10.Text == "NMarch@Visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("NMarch@Visographic.com", "#NickyViso062092");
                    }


                    if (DropDownList10.Text == "Bob@visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("Bob@visographic.com", "14018040Rvd");

                    }


                    if (DropDownList10.Text == "JCardelli@Visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("JCardelli@Visographic.com", "!luvCubbies23##");
                    }

                    if (DropDownList10.Text == "DJoswiak@Visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("DJoswiak@Visographic.com", "jawS#007");
                    }
                    if (DropDownList10.Text == "RickEbel@visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("RickEbel@visographic.com", "Alsace504f09!");
                    }

                    if (DropDownList10.Text == "jmarch@visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("jmarch@visographic.com", "Jmarch42759");
                    }

                    if (DropDownList10.Text == "jhernandez@visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("jhernandez@visographic.com", "Zokie123");
                    }


                    if (DropDownList10.Text == "kjacobsen@visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("kjacobsen@visographic.com", "wood234Stock2A**");
                    }

                    if (DropDownList10.Text == "JRojek@Visographic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("JRojek@Visographic.com", "BennyMeme1!!");
                    }

                    if (DropDownList10.Text == "DKnowles@VisoGraphic.com")
                    {

                        SmtpServer.Credentials = new System.Net.NetworkCredential("DKnowles@VisoGraphic.com", "1232Milne$$");
                    }


                    mail.CC.Add(DropDownList10.Text);

                    SmtpServer.Send(mail);



                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                    sent = false;

                    Label21.Text = "Email failed to send: "+s;
                }
                

                if (sent == true)
                {
                    Label21.Text = "Email sent";

                    mailBuilder = "";

                }
            }//end else
 }//end send email



        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string csrID = "";
            foreach (DataRow s in dtTest.Rows)
            {

                if (DropDownList1.Text == (s["cust-name"].ToString()))
                {

                    DropDownList2.Text = s["Cust-code"].ToString();

                    csrID = s["CSR-ID"].ToString();

                }

            }


            foreach (DataRow s in dtTest.Rows)
            {

                if (DropDownList2.Text.Contains(s["Cust-code"].ToString()))
                {
                    foreach (DataRow dr in csrNames.Rows)
                    {
                        if (s["Sales-agent-id"].ToString() == dr["Sales-agent-id"].ToString())
                        {

                            DropDownList3.Text = dr["Sales-Agent-Name"].ToString();

                        }

                    }
                }


            }//end foreach
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TextBox12.Text = DropDownList1.Text;


            foreach (DataRow s in dtTest.Rows)
            {

                if (DropDownList1.Text == (s["Cust-code"].ToString()))
                {

                    DropDownList2.Text = s["cust-name"].ToString();
                }


            }//end foreach


            foreach (DataRow s in dtTest.Rows)
            {

                if (DropDownList1.Text.Contains(s["Cust-code"].ToString()))
                {
                    foreach (DataRow dr in csrNames.Rows)
                    {
                        if (s["Sales-agent-id"].ToString() == dr["Sales-agent-id"].ToString())
                        {

                            DropDownList3.Text = dr["Sales-Agent-Name"].ToString();

                        }

                    }
                }


            }//end foreach


        }
    }//end class
}//end namespace

