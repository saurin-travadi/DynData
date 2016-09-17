<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="DynMonitoring.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function getLogs() {

            PageMethods.GetLogs("", onSucess, onError);

            function onSucess(result) {
                document.getElementById('logs').innerHTML = result.replace('~','<br />');
            }

            function onError(result) {
                document.getElementById('logs').innerHTML += 'Cannot process your request at the moment, please try later.';
            }
        }
    </script>
</head>
<body onload="getLogs()">

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true">
        </asp:ScriptManager>
        <div id="logs">
        </div>
    </form>
</body>
</html>
