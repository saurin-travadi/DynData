<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="DynMonitoring.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width, minimum-scale=1, maximum-scale=1, user-scalable=no">
    <meta name="apple-mobile-web-app-capable" content="yes">
    <title></title>
    <style>
        a {
            display: block;
        }
    </style>
    <script type="text/javascript">
        function getLogs(date) {

            PageMethods.GetLogs(date, onSucess, onError);

            function onSucess(result) {
                document.getElementById('logs').innerHTML = result.replace('~', '<br />');
            }

            function onError(result) {
                document.getElementById('logs').innerHTML += 'Cannot process your request at the moment, please try later.';
            }

            return false;
        }


    </script>
</head>
<body>

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
        </asp:ScriptManager>
        
    </form>
    <div id="logs">
        </div>
</body>
</html>
