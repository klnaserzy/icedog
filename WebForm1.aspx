<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="dog.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <script type="text/javascript">
        var T;      //紀錄timer
        function init() {
            T = setInterval("timer()", 80);        //時間用100附近會重複訊息(未知)
        }

        function timer() {  //javascript的timer
            if (document.getElementById("HFclt").value == "") return;     //HFclt沒收到訊息，HFclt為傳給自己的HiddenField
            var Msg = document.getElementById("HFclt").value;      //取得訊息
            document.getElementById("HFclt").value = "";      //刪除HFclt的資料
            var Mesg = Msg.split(",");
            var Cmd = Mesg[0];
            var Str = Mesg[1];

            switch (Cmd) {
                case "alert":   //有問題時 通知使用者
                    alert(Str);
                    break;
            }
        }
    </script>
</head>
<body onload="init()" style="height: 243px" onunload="jscript:__doPostBack('Button1','')">
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            
        </div>
        名稱：<asp:Label ID="Account" runat="server" Text="紀錄Account的Label"></asp:Label>
        <br />
        密碼：<asp:Label ID="password" runat="server" Text="紀錄password的Label"></asp:Label>
        <asp:Button ID="Btn_login" runat="server" Text="已登入" Enabled="False" />
        <br />
        <br />
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Timer ID="Timer1" runat="server" Interval="50" OnTick="Timer1_Tick">
                </asp:Timer>
                <asp:HiddenField ID="HFclt" runat="server" />
                <asp:Label ID="Label2" runat="server" Text="線上名單" Visible="False"></asp:Label>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" Text="離線名單" Visible="False"></asp:Label>
                <br />
                <asp:ListBox ID="ListBox1" runat="server" Height="323px" Visible="False" Width="260px"></asp:ListBox>
                &nbsp;&nbsp;&nbsp;
                <asp:ListBox ID="ListBox2" runat="server" Height="323px" Visible="False" Width="260px"></asp:ListBox>
                <br />
                <asp:Label ID="Label3" runat="server" Text="出現過的球號："></asp:Label>
                <br />
                <asp:Label ID="Label4" runat="server"></asp:Label>
                <br />
                <asp:Panel ID="Panel_Student_Cube_Update" runat="server">
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:Panel ID="Panel_Teacher" runat="server" Visible="False">
            <asp:Button ID="Btn3" runat="server" Text="3 x 3" Height="60px" OnClick="Btn3_Click" Width="80px" />
            &nbsp;<asp:Button ID="Btn5" runat="server" Height="60px" OnClick="Btn5_Click" Text="5 x 5" Width="80px" />
            &nbsp;<asp:Button ID="Btn7" runat="server" Height="60px" OnClick="Btn7_Click" Text="7 x 7" Width="80px" />
            &nbsp;<asp:TextBox ID="Txt_Cube" runat="server" Height="55px" Width="80px"></asp:TextBox>
            &nbsp;<asp:Button ID="Btn_CreatCube" runat="server" Height="60px" OnClick="BtnCreatCube_Click" Text="? x ?(輸入數字後點擊)" Width="176px" />
            <br />
            <asp:Button ID="Btn_Start" runat="server" Height="60px" Text="開始" Width="80px" OnClick="Btn_Start_Click" />
            &nbsp;<asp:Button ID="Btn_NextQ" runat="server" Height="90px" Text="下一題" Width="150px" Enabled="False" OnClick="Btn_NextQ_Click" />
            &nbsp;<asp:Button ID="Btn_LockAns" runat="server" Height="90px" Text="鎖定答題" Width="160px" Enabled="False" OnClick="Btn_LockAns_Click" />
            <br />
            <asp:Button ID="Btn_End" runat="server" Height="60px" Text="結束" Width="80px" Enabled="False" OnClick="Btn_End_Click" />
            &nbsp;<br />
            <br />
            <br />
            <asp:Button ID="Btn_Timer" runat="server" Height="47px" OnClick="Btn_Timer_Click" Text="Timer停止" Width="86px" />
        </asp:Panel>
        <asp:Panel ID="Panel_Student" runat="server" Visible="False">
            <br />
            <asp:TextBox ID="Txt_Ans" runat="server" Height="34px" Width="457px"></asp:TextBox>
            <asp:Button ID="Btn_Ans" runat="server" Height="40px" OnClick="Btn_Ans_Click" Text="確定答案" Width="95px" />
            <asp:Label ID="Label_grades" runat="server" Text="分數：0" Visible="False"></asp:Label>
            <br />
            <asp:Label ID="Lbl_TrueOrFalse" runat="server"></asp:Label>
            <br />
            <asp:Label ID="Label_grades_Record" runat="server"></asp:Label>
            <br />
            方塊內請輸入1~75<asp:Panel ID="Panel_Student_Cube" runat="server">
            </asp:Panel>
            <asp:Button ID="Btn_RandomNum" runat="server" Height="60px" OnClick="Btn_RandomNum_Click" Text="隨機產生數字" Width="110px" />
            &nbsp;
            <asp:Button ID="Btn_Clear" runat="server" Height="60px" OnClick="Btn_Clear_Click" Text="清除版面" Width="110px" />
            <br />
            <br />
            &nbsp;<asp:Button ID="Btn_Ready" runat="server" Height="60px" OnClick="Btn_Ready_Click" Text="數字輸入完成" Width="110px" />
            &nbsp;
            <asp:Button ID="Btn_CubeChanged" runat="server" Enabled="False" Height="60px" OnClick="Btn_CubeChanged_Click" Text="當老師要更改大小 但已經按輸入完成時點擊" Width="330px" />
            要刪除版面內容或老師改變大小時，請點擊清除版面。<br />
        </asp:Panel>
    </form>
</body>
</html>
