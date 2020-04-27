<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolCaseServicesWebForm.aspx.cs" Inherits="SolcaseUtility.SolCaseServicesWebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Server S-BL-KC01 Solcase Utilities Page.</title>
    <link href="css/SiteLAF.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="SolCaseServices" runat="server">
        <div>
            <p>Server S-BL-KC01 Solcase Utilities Page.</p>

            <asp:TextBox ID="txtBoxClientId" runat="server" ToolTip="Entere a valid Client Id (6 Digits)"></asp:TextBox>
            <asp:Button CssClass="Button" ID="Button1" runat="server" Text="Get Client Docs" OnClick="Button1_Click" Width="105px" Height="20px" />

            <br />
            <br />
        </div>
       
        <br />
        <div id="div_xml" runat="server"></div>
                
        <asp:GridView ID="GridViewClientDocs" runat="server" CssClass="mygrdContent" PagerStyle-CssClass="pager" HeaderStyle-CssClass="header" RowStyle-CssClass="rows" 
            AllowPaging="False" AllowSorting="True">
        </asp:GridView>
                
    </form>
    </body>
</html>
