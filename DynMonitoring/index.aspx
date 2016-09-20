<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="DynMonitoring.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function getLogs() {

            var date = getUrlVars()["d"];
            if (date == undefined) date = "";
            PageMethods.GetLogs(date, onSucess, onError);

            function onSucess(result) {
                document.getElementById('logs').innerHTML = result.replace('~','<br />');
            }

            function onError(result) {
                document.getElementById('logs').innerHTML += 'Cannot process your request at the moment, please try later.';
            }
        }

        function getUrlVars() {
            var vars = [], hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
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
