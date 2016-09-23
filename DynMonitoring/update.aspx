<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="update.aspx.cs" Inherits="DynMonitoring.update" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, minimum-scale=1, maximum-scale=1, user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="LKQPush" runat="server" Text="LKQPush" OnClick="LKQPush_Click" />
            <asp:Button ID="IAAGet" runat="server" Text="IAAGet" OnClick="IAAGet_Click" />
            <asp:Button ID="LKQGet" runat="server" Text="LKQGet" OnClick="LKQGet_Click" />
        </div>
    </form>
</body>
</html>
