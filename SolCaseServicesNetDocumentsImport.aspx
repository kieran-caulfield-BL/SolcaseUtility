<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolCaseServicesNetDocumentsImport.aspx.cs" Inherits="SolcaseUtility.SolCaseServicesNetDocumentsImport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Server S-BL-KC01 Solcase to NetDocuments Import Page.</title>
    <link href="css/SiteLAF.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="SolCaseServicesNDImport" runat="server">
        <div style="font-family:Arial, Helvetica, sans-serif" class="w3-container">
            <div class="w3-panel w3-metro-light-blue">
                <h2>Solcase History Docs Export to nD Import.</h2>
                <asp:TextBox ID="txtBoxMatterId" runat="server" ToolTip="Enter a valid Matter Code (6 Digits '-' 6 Digits)"></asp:TextBox>
                <h4>Net Documents Workspace Reference.</h4>
                <asp:TextBox ID="txtBoxNetDocsClient" runat="server" ToolTip="Enter a valid Net Documents Client Code (must be numeric)"></asp:TextBox><asp:TextBox ID="TxtBoxnetDocsMatter" runat="server" ToolTip="Enter a valid Net Documents Matter Code (must be Numeric)"></asp:TextBox>
                <asp:Button CssClass="Button" ID="Button11" runat="server" Text="Get History Docs List" OnClick="Button11_Click" Width="129px" Height="20px" />
                <asp:Button CssClass="Button" ID="Button12" runat="server" Text="Download" OnClick="Button12_Click" Width="105px" Height="20px" />
            </div>
            <hr />
            <div id="div_xml" class="w3-metro-grey" runat="server"></div>
            <div id="div_matterDesc" class="w3-metro-light-blue" runat="server"></div>
            <div id="div_debug" class="w3-metro-light-yellow" runat="server"></div>
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
