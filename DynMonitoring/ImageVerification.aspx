<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImageVerification.aspx.cs" Inherits="DynMonitoring.ImageVerification" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="ImageVerification.js" type="text/javascript"></script>
    <style>
        div{
            padding-top:10px;
        }

        .divstyle {
            white-space: nowrap;
            width: 100%;
        }

        .divstyle img {
                width: 20%;
        }
    </style>
    <title></title>
</head>
<body onload="GetStockDetails()">
    <form id="formimagecheck" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
            </asp:ScriptManager>
            <div>
                <asp:Label ID="lblstockNo" runat="server" Text=""></asp:Label>
            </div>
            <div class="divstyle" id="imageset1">
                <asp:Image ID="Image1" runat="server" />
                <asp:Image ID="Image2" runat="server" />
                <asp:Image ID="Image3" runat="server" />
                <asp:Image ID="Image4" runat="server" />
                <asp:Image ID="Image5" runat="server" />
            </div>
            <div class="divstyle" id="imageset2">
                <asp:Image ID="Image6" runat="server" />
                <asp:Image ID="Image7" runat="server" />
                <asp:Image ID="Image8" runat="server" />
                <asp:Image ID="Image9" runat="server" />
                <asp:Image ID="Image10" runat="server" />
            </div>
            <div>
                <asp:Button ID="btnpass" runat="server" Text="Pass" OnClientClick="PassFailClick(1)" />
                <asp:Button ID="btnfail" runat="server" Text="Fail" OnClientClick="PassFailClick(0)" />
            </div>
        </div>        
    </form>
</body>
</html>
