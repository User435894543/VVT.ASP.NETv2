<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VVT.aspx.cs" Inherits="VVT.ASP.NETv2.VVT" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>

<head>
	<style>
		.flex { display: flex;
            width: 384px;
        }
		.mr { margin-right: 1em; }
	    #Button1 {
        }
	    #Text1 {
            width: 181px;
        }
	</style>
</head>
<body>
	<form id="form1" runat="server">

	<div class="flex">
		<span style="font-size: 30px; color: #229F7A;" class="mr">VVT</span>
		<span style="font-size: 13px;" class="mr">Version 1.6</span>
	</div>
    <p>
        Enter Job Number:</p>
    <p>
                <asp:TextBox ID="jobNumTextBox" runat="server" Width="144px"></asp:TextBox>
        </p>
    <p>

  
        <!-- scripts for button clicks-->
          <script>
              function printTicketBttn() {
                  let labelElement = document.getElementById("printjobTicketLbl");
                  var d = new Date();
                  var n = d.toLocaleString([], { hour12: true });
                  labelElement.innerHTML = "...Running:     "+n;
              }
          </script>

          <script>
              function prePressTicketBttn_Click1() {
                  let labelElement = document.getElementById("prePressLbl");
                  var d = new Date();
                  var n = d.toLocaleString([], { hour12: true });
                  labelElement.innerHTML = "...Running:     " + n;
              }
          </script>

                  <script>
                      function mailTicketBttn_Click1() {
                          let labelElement = document.getElementById("mailLbl");
                          var d = new Date();
                          var n = d.toLocaleString([], { hour12: true });
                          labelElement.innerHTML = "...Running:     " + n;
                      }
                  </script>

                      <script>
                          function shipTicketBttn_Click1() {
                              let labelElement = document.getElementById("shipLbl");
                              var d = new Date();
                              var n = d.toLocaleString([], { hour12: true });
                              labelElement.innerHTML = "...Running:     " + n;
                          }
                      </script>

                                            <script>
                                                function poTicketBttn_Click1() {
                                                    let labelElement = document.getElementById("poLbl");
                                                    var d = new Date();
                                                    var n = d.toLocaleString([], { hour12: true });
                                                    labelElement.innerHTML = "...Running:     " + n;
                                                }
                                            </script>

        
                                            <script>
                                                function fullTicketBttn_Click1() {
                                                    let labelElement = document.getElementById("fullLbl");
                                                    var d = new Date();
                                                    var n = d.toLocaleString([], { hour12: true });
                                                    labelElement.innerHTML = "...Running:     " + n;
                                                }
                                            </script>

        <asp:Button ID="prePressTicketBttn" runat="server" Height="26px" Text="Pre Press Ticket" Width="136px" OnClientClick="prePressTicketBttn_Click1();" OnClick="prePressTicketBttn_Click1"/>


          <asp:Label ID="prePressLbl" runat="server" CssClass="lblStatus"></asp:Label>


        </p>
        <asp:Button ID="mailingTickedBttn" runat="server" Text="Mailing Ticket" Height="26px" Width="136px" OnClientClick="mailTicketBttn_Click1();" OnClick="mailTicketBttn_Click"/>

  

        <asp:Label ID="mailLbl" CssClass="lblStatus" runat="server"></asp:Label>

  

        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" AutoDataBind="true" HasRefreshButton="True" ToolPanelView="None" />


        <p>
            <asp:Button ID="shipTicket" runat="server" Text="Bindery/Shipping Ticket" Height="26px" Width="158px" OnClientClick="shipTicketBttn_Click1();" OnClick="shipTicketBttn_Click"/>

  

            <asp:Label ID="shipLbl" runat="server" Text="" CssClass="lblStatus"></asp:Label>

  

        </p>


        <p>
            <asp:Button ID="poTicket" runat="server" Text="PO Ticket" Height="26px" Width="136px" OnClientClick="poTicketBttn_Click1();" OnClick="poTicketBttn_Click"/>
            
            <asp:Label ID="poLbl" runat="server" Text="" CssClass="lblStatus"></asp:Label>

  

        </p>


        <p>
            <asp:Button ID="fullTicket" runat="server" Text="Full Ticket" Height="26px" Width="136px" OnClientClick="fullTicketBttn_Click1();" OnClick="fullTicketBttn_Click" />

  <asp:Label ID="fullLbl" CssClass="lblStatus" runat="server"></asp:Label>

        </p>


        <p>
        <asp:Button ID="printJobTicketBttn" runat="server" OnClick="printJobTicketBttn_Click" OnClientClick="printTicketBttn();" Text="Print Job Ticket" />

                    <asp:Label ID="printjobTicketLbl" CssClass="lblStatus" runat="server"></asp:Label>

  

        </p>


    </form>
</body>


</html>
