using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Drawing;

namespace dog
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        string Teacher = "teacher";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string User = Request.Params["User"];
                if (User != null)
                {
                    HFclt.Value = "userLog" + "," + "名稱已登入";
                    Account.Text = User;
                    password.Text = Application["password"].ToString();
                }
            }
        }

        protected void Btn_login_Click(object sender, EventArgs e)
        {
            string User;

            if (string.IsNullOrWhiteSpace(Account.Text))    //如果帳號未輸入或空白
            {
                HFclt.Value = "userLog" + "," + "請輸入名稱";  //HFclt為傳給自己的HiddenField
                return;
            }
            if (string.IsNullOrWhiteSpace(password.Text))       //如果密碼未輸入或空白
            {
                HFclt.Value = "userLog" + "," + "請輸入密碼";  //HFclt為傳給自己的HiddenField
                return;
            }

            if (Account.Text == Teacher)     //老師登入
            {
                User = Account.Text;       //紀錄老師登入
                Application.Lock();     //鎖定網站公用變數able
                Application["password"] = password.Text;    //將老師設定的密碼放入公用變數password
                Application.UnLock();   //解除鎖定
            }
            else    //學生登入
            {
                if (Application["password"] == null)   //老師尚未設定密碼
                {
                    HFclt.Value = "userLog" + "," + "老師尚未設定密碼";
                    return;
                }
                if (password.Text != Application["password"].ToString())    //密碼是否輸入正確
                {
                    HFclt.Value = "userLog" + "," + "密碼輸入錯誤";
                    return;
                }
                User = Account.Text;       //紀錄學生登入
            }
            Account.Enabled = false;
            password.Enabled = false;
            Btn_login.Text = "已登入";
            Btn_login.Enabled = false;
            Response.Redirect("~/WebForm1.aspx?User=" + User, true);        //轉移頁面至WebForm1.aspx  傳遞User
        }
    }
}