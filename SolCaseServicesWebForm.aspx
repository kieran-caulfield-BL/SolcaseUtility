<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolCaseServicesWebForm.aspx.cs" Inherits="SolcaseUtility.SolCaseServicesWebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #TextArea1 {
            width: 775px;
            height: 195px;
        }
    </style>
</head>
<body>
    <form id="SolCaseServices" runat="server">
        <div>
            <p>Server S-BL-KC01 Solcase Utilities Page.</p>
            <a href="SolCaseService.asmx?op=getClientDocs">Go to Solcase Service - Get Client</a>
            <br />
            <asp:Button ID="Button1" runat="server" Text="Get Client Docs" OnClick="Button1_Click" />
        <br />
        </div>
        
        <p>
            <asp:TextBox ID="TextBox1" runat="server" Height="243px" Width="670px"></asp:TextBox>
        </p>
        
    </form>
</body>
</html>
