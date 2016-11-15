<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserAuth.aspx.cs" Inherits="DynMonitoring.UserAuth" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .box {
            border: .5px solid silver;
            margin-left: 80px;
            margin-top: 30px;
            padding: 10px;
            width: 200px;
        }
    </style>
</head>
<body>
    <form id="FormUserAuth" runat="server">
        <div class="box">
            <span>Enter Your Name :</span>
            <input type="text" id="txtName" />
            <input type="button" id="btnsubmit" value="Submit"  onclick="submit_click()" />
        </div>
    </form>
    <script type="text/javascript">
        function submit_click() {
            var userName = document.getElementById("txtName").value.trim();
            if (userName == "") {
                alert("Please enter a name")
                return;
            }
            localStorage.setItem("userName", userName);
            window.location.href = "ImageVerification.aspx";
        }
    </script>
</body>
</html>
