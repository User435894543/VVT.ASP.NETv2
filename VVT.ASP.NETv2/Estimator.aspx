<%@ Page Title="Estimator" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Estimator.aspx.cs" Inherits="VVT.ASP.NETv2.Estimator" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

<!DOCTYPE html>

<html>

    <!-- CSS Styling
         Place between html tags at top here
        -->
    <style>
        .labelAndInput {
            display:inline;
            text-align:right;
        }

        .endLine {

            text-align:right;
            margin-bottom: 1px;
            display:block;
            content: "";
        }

    </style>



<body>

          <div class="jumbotron">
        <h1>&nbsp;</h1>
        <p class="lead" style="font-size: 50px; color: #229F7A;">Viso Request For Quote</p>

              <!-- Customer ID-->
              </div>
              <div class =" labelAndInput">
              <asp:Label ID="Label1" runat="server" Text="Customer ID*:" style ForeColor="#FF3300"></asp:Label>
               <asp:DropDownList ID="DropDownList1" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged"></asp:DropDownList>
                 </div>
</body>
    
    <!-- customer name-->
    <br>
             
              <div class =" labelAndInput">
              <asp:Label ID="Label2" runat="server" Text="Customer Name:*" style ForeColor="#FF3300"></asp:Label>
               <asp:DropDownList ID="DropDownList2" runat="server"></asp:DropDownList>
                 </div>
            
        </br>

    <!-- Sales Person -->
        <br>
             
              <div class =" labelAndInput">
              <asp:Label ID="Label3" runat="server" Text="Sales Person*:" style ForeColor="#FF3300"></asp:Label>
                   <asp:DropDownList ID="DropDownList3" runat="server"></asp:DropDownList>
                 </div>
            
        </br>


    <!-- Short Description-->
        <br>
             
              <div class =" labelAndInput">
              <asp:Label ID="Label4" runat="server" Text="Short Description*:" style ForeColor="#FF3300"></asp:Label>
                 </div>
            
        <asp:TextBox ID="TextBox13" runat="server"></asp:TextBox>
            
        </br>

    <!-- new, rerun, rerun with changes textbox use inline CSS-->
   <br>
       <div class =" labelAndInput">
       <asp:CheckBox ID="CheckBox1" runat="server" Text=" New" TextAlign="Left" style="margin-right: 15px" ForeColor="#FF3300" />
       <asp:CheckBox ID="CheckBox2" runat="server" Text=" ReRun" TextAlign="Left" style="margin-right: 15px" ForeColor="#FF3300"/>
       <asp:CheckBox ID="CheckBox3" runat="server" Text="ReRun with Changes" TextAlign="Left" style="margin-right: 15px" ForeColor="#FF3300"/>
       <asp:Label ID="Label5" runat="server" Text="If rerun, last job#: " style="margin-right: 1px"></asp:Label>
           <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
           </div>
   </br>

        <br>

             <asp:Label ID="Label22" runat="server" Text="Viso to Mail*:" style ForeColor="#FF3300"></asp:Label>
            <asp:DropDownList ID="DropDownList4" runat="server"></asp:DropDownList>

                             <asp:Label ID="Label23" runat="server" Text="FSC*:" style ForeColor="#FF3300"></asp:Label>
            <asp:DropDownList ID="DropDownList11" runat="server"></asp:DropDownList>

                                         <asp:Label ID="Label24" runat="server" Text="DSF*:" style ForeColor="#FF3300"></asp:Label>
            <asp:DropDownList ID="DropDownList12" runat="server"></asp:DropDownList>
    </br>

    <!-- sales classes-->
    <br>
    <div class =" labelAndInput">
       <asp:Label ID="Label6" runat="server" Text="Sales Class ID 1*" style="margin-right: 15px" ForeColor="#FF3300"></asp:Label>
         <asp:DropDownList ID="DropDownList5" runat="server" style="margin-right: 15px"></asp:DropDownList>

               <asp:Label ID="Label7" runat="server" Text="Sales Class ID 2" style="margin-right: 15px"></asp:Label>
         <asp:DropDownList ID="DropDownList6" runat="server" style="margin-right: 15px"></asp:DropDownList>

               <asp:Label ID="Label8" runat="server" Text="Sales Class ID 3" style="margin-right: 15px"></asp:Label>
         <asp:DropDownList ID="DropDownList7" runat="server" style="margin-right: 15px"></asp:DropDownList>

               <asp:Label ID="Label9" runat="server" Text="Sales Class ID 4" style="margin-right: 15px"></asp:Label>
         <asp:DropDownList ID="DropDownList8" runat="server"></asp:DropDownList>

        </div>
    </br>


    <!-- quantities-->
        <br>
    <div class =" labelAndInput">
       <asp:Label ID="Label10" runat="server" Text="Quantity 1*" style="margin-right: 1px" ForeColor="#FF3300"></asp:Label>
        <asp:TextBox ID="TextBox2" runat="server" style="margin-right: 15px" Width="64px"></asp:TextBox>

               <asp:Label ID="Label11" runat="server" Text="Quantity 2" style="margin-right: 1px"></asp:Label>
         <asp:TextBox ID="TextBox3" runat="server" style="margin-right: 15px" Width="64px"></asp:TextBox>

               <asp:Label ID="Label12" runat="server" Text="Quantity 3" style="margin-right: 1px" Width="64px"></asp:Label>
         <asp:TextBox ID="TextBox4" runat="server" style="margin-right: 15px" Width="64px"></asp:TextBox>

               <asp:Label ID="Label13" runat="server" Text="Quantity 4" style="margin-right: 1px"></asp:Label>
        <asp:TextBox ID="TextBox5" runat="server" style="margin-right: 15px" Width="64px"></asp:TextBox>

                       <asp:Label ID="Label14" runat="server" Text="Quantity 5" style="margin-right: 1px"></asp:Label>
        <asp:TextBox ID="TextBox6" runat="server" style="margin-right: 15px" Width="64px"></asp:TextBox>
        </div>
    </br>



    <!-- estimate description-->
    <br>
       <div class =" labelAndInput">
    <asp:Label ID="Label15" runat="server" Text="Estimate description*:" style="margin-right: 1px" ForeColor="#FF3300"></asp:Label>
         <asp:TextBox ID="TextBox7" runat="server" style="margin-right: 15px" Width="535px" Height="153px"></asp:TextBox>
           </div>
        <br/>



        <!-- special notes-->
    <br>
       <div class =" labelAndInput">
    <asp:Label ID="Label16" runat="server" Text="Special Notes:" style="margin-right: 1px"></asp:Label>
         <asp:TextBox ID="TextBox8" runat="server" style="margin-right: 15px" Width="535px" Height="153px"></asp:TextBox>
           </div>
        <br/>



           <!-- email section-->

   <style>
       br {
           line-height:0;
           margin:0;
       }
        </style>

    <p>
        <body>
       <div class ="labelAndInput">
    <asp:Label ID="Label17" runat="server" Text="To:*" style="margin-right: 1px" ForeColor="#FF3300"></asp:Label>
         <asp:TextBox ID="TextBox9" runat="server" style="margin-right: 10px" Width="145px" Height="20px"></asp:TextBox>
           </div>


   
       <div class ="labelAndInput">
    <asp:Label ID="Label18" runat="server" Text="CC:" style="margin-right: 1px"></asp:Label>
         <asp:DropDownList ID="DropDownList9" runat="server"></asp:DropDownList>
           </div>

                   <div class ="labelAndInput">
    <asp:Label ID="Label19" runat="server" Text="From:*" style="margin-right: 1px" ForeColor="#FF3300"></asp:Label>
         <asp:DropDownList ID="DropDownList10" runat="server"></asp:DropDownList>
           </div>

                   <div class ="labelAndInput">
    <asp:Label ID="Label20" runat="server" Text="Subject Line:*" style="margin-right: 1px" ForeColor="#FF3300"></asp:Label>
         <asp:TextBox ID="TextBox12" runat="server" style="margin-right: 10px" Width="145px" Height="20px"></asp:TextBox>
           </div>
            </body>

        <asp:Button ID="Button2" runat="server" Text="Add Attatchment" style="margin-right: 10px" />
        <asp:Button ID="Button3" runat="server" Text="Preview Email" />
        <br>
        <asp:Button ID="Button1" runat="server" Text="Send Email" OnClick="Button1_Click" />
        <asp:Label ID="Label21" runat="server"></asp:Label>
        </p>



</html>
    </asp:Content>