<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="SolCaseServicesNetDocumentsImport.aspx.cs" Inherits="SolcaseUtility.SolCaseServicesNetDocumentsImport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Server S-BL-KC01 Solcase to NetDocuments Import Page.</title>
    <link href="css/SiteLAF.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .mygrdContent {}
    </style>
</head>
<body>
    <form id="SolCaseServicesNDImport" method="post" runat="server" enctype="multipart/form-data" action="SolCaseServicesNetDocumentsImport.aspx">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
        <div style="font-family:Arial, Helvetica, sans-serif" class="w3-container">
        Actionstep File to Upload to Server <input id="oFile" type="file" runat="server" name="oFile" />
        <asp:button id="btnUpload" type="submit" text="Upload" runat="server" OnClick="btnUpload_Click"></asp:button>
        <asp:Panel ID="frmConfirmation" Visible="False" Runat="server">
            <asp:Label id="lblUploadResult" Runat="server"></asp:Label>  
            
        </asp:Panel>
        <!--div style="font-family:Arial, Helvetica, sans-serif" class="w3-container">-->

            <div class="w3-panel w3-metro-light-blue">
                <asp:UpdatePanel runat="server" id="UpdatePanel" updatemode="Conditional">        
                <Triggers>
                    <asp:AsyncPostBackTrigger controlid="UploadGrid" EventName="SelectedIndexChanged" />
                </Triggers>
                <ContentTemplate> 
                    <div style="font-family:Arial, Helvetica, sans-serif; width: 100%; height: 400px; overflow: scroll" class="w3-container">
                    <asp:GridView ID="UploadGrid"  runat="server" AutoGenerateSelectButton="True" CssClass="mygrdContent" PagerStyle-CssClass="pager" HeaderStyle-CssClass="header" RowStyle-CssClass="rows" 
                AllowPaging="False" AllowSorting="True" OnSelectedIndexChanged="UploadGrid_SelectedIndexChanged" AutoPostBack="True" Height="62px" Width="594px" />
                    </div>
                    <asp:TextBox ID="txtBoxMatterId" runat="server" ToolTip="Enter a valid Matter Code (6 Digits '-' 6 Digits)"></asp:TextBox>
                    <asp:TextBox ID="txtBoxNetDocsClient" runat="server" ToolTip="Enter a valid Net Documents Client Code (must be numeric)"></asp:TextBox>
                    <asp:TextBox ID="TxtBoxnetDocsMatter" runat="server" ToolTip="Enter a valid Net Documents Matter Code (must be Numeric)"></asp:TextBox>
                 </ContentTemplate>
                </asp:UpdatePanel>
                
             </div>  
             <div class="w3-panel w3-metro-light-green">
                <asp:TextBox ID="txtBoxCommand" runat="server" Width="563px" ToolTip="Copy and Paste to the command line to run ndImport for your file"></asp:TextBox>
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
