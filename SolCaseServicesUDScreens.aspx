<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolCaseServicesUDScreens.aspx.cs" Inherits="SolcaseUtility.SolCaseServicesUDScreens" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head12" runat="server">
    <title>Server S-BL-KC01 Solcase User Define Screens.</title>
    <link href="css/SiteLAF.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .mygrdContent {}
    </style>
</head>
<body>
    <form id="SolCaseUDScreens" runat="server">
        <div style="font-family:Arial, Helvetica, sans-serif" class="w3-container">
            <div class="w3-panel w3-metro-light-green">
                <h2>Solcase User Defined Screens Viewer.</h2>
                <asp:Image ID="Image3" runat="server" Height="23px" ImageAlign="Right" ImageUrl="~/images/BL-NameAndIcon.jpg" />
                <asp:TextBox ID="txtBoxMatterId" runat="server" ToolTip="Enter a valid Matter Code (6 Digits '-' 6 Digits)"></asp:TextBox>
                <asp:Button CssClass="Button" ID="Button11" runat="server" Text="Get UD Screens" OnClick="Button11_Click" Width="129px" Height="20px" />
                <asp:Button CssClass="Button" ID="Button12" runat="server" Text="Download" OnClick="Button12_Click" Width="105px" Height="20px" />
            </div>
            <hr />
            <div id="div_xml2" class="w3-metro-grey" runat="server"></div>
            <div id="div_matterDesc2" class="w3-metro-light-blue" runat="server"></div>
        </div>
        <div>
            <table><tr><td>
                &nbsp;</td><td>    
            <asp:GridView ID="GridViewUDScreens" runat="server" CssClass="mygrdContent" PagerStyle-CssClass="pager" HeaderStyle-CssClass="header" RowStyle-CssClass="rows" 
                AllowPaging="False" AllowSorting="True" OnSelectedIndexChanged="GridViewUDScreens_SelectedIndexChanged" OnSorting="GridViewUDScreens_OnSort" OnRowDataBound="GridViewUDScreens_RowDataBound" Width="1093px">
            </asp:GridView>
            </td></tr></table>
        </div>        
    </form>
    </body>
</html>
