<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolCaseServicesWebForm.aspx.cs" Inherits="SolcaseUtility.SolCaseServicesWebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Server S-BL-KC01 Solcase Utilities Page.</title>
    <link href="css/SiteLAF.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="SolCaseServices" runat="server">
        <div style="font-family:Arial, Helvetica, sans-serif" class="w3-container">
            <div class="w3-panel w3-metro-light-green">
                <h2>Solcase Client Documents Viewer.</h2>
                <asp:Image ID="Image2" runat="server" Height="23px" ImageAlign="Right" ImageUrl="~/images/BL-NameAndIcon.jpg" />
                <asp:TextBox ID="txtBoxClientId" runat="server" ToolTip="Entere a valid Client Id (6 Digits)"></asp:TextBox>
                <asp:Button CssClass="Button" ID="Button1" runat="server" Text="Get Client Docs" OnClick="Button1_Click" Width="129px" Height="20px" />
                <asp:Button CssClass="Button" ID="Button2" runat="server" Text="Download" OnClick="Button2_Click" Width="105px" Height="20px" />
            </div>
            <hr />
            <div id="div_xml" class="w3-metro-orange" runat="server"></div>
            <div id="div_clientName" class="w3-metro-light-blue" runat="server"></div>
            <div id="div_matterDesc" class="w3-metro-light-blue" runat="server"></div>
        </div>
        <div>
            <table><tr><td>
            <asp:TreeView ID="TreeViewMatterList" 
                NodeStyle-CssClass="treeNode"
                RootNodeStyle-CssClass="rootNode"
                LeafNodeStyle-CssClass="leafNode"
                OnSelectedNodeChanged="TreeViewMatterList_SelectedNodeChanged" runat="server">
            </asp:TreeView>
            </td><td>    
            <asp:GridView ID="GridViewClientDocs" runat="server" CssClass="mygrdContent" PagerStyle-CssClass="pager" HeaderStyle-CssClass="header" RowStyle-CssClass="rows" 
                AllowPaging="False" AllowSorting="True" OnSelectedIndexChanged="GridViewClientDocs_SelectedIndexChanged" OnSorting="GridViewClientDocs_OnSort">
            </asp:GridView>
            </td></tr></table>
        </div>        
    </form>
    </body>
</html>
