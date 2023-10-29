using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Drawing;
using System.IO;

namespace dog
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        string Teacher = "teacher";
        string[] Answer = new string[]
        {
                "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15",
                "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27",
                "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
                "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51",
                "52", "53", "54", "55", "56", "57", "58", "59", "60", "61", "62", "63",
                "64", "65", "66", "67", "68", "69", "70", "71", "72", "73", "74", "75"
        };
        protected void Page_Load(object sender, EventArgs e)
        {
            //Application[Account.Text]記錄個人資料 Account.Text
            string User = Request.Params["User"];       //紀錄前一個頁面Log.aspx的User
            bool relog = false;     //紀錄是否重新登入

            if (!IsPostBack)      //Page是否為首次載入，是為真
            {
                if (User != null)
                {
                    Account.Text = User.ToString();
                    password.Text = Application["password"].ToString();
                }
                else      //測試時避免未登入直接跳到WebForm11 以及避免用網址直接進入
                {
                    Response.Redirect("~/Log.aspx", true);      //導向登入畫面
                }

                Hashtable L = (Hashtable)Application["L"];    //取得公用變數
                foreach (var h in L.Keys)
                {
                    if (h.ToString() == User)     //避免重複登入
                    {
                        Response.Redirect("~/Log.aspx?User=" + User, true);     //名稱已經登入了 導向登入畫面
                    }
                }
                List<string> I = (List<string>)Application["I"];
                foreach (string s in I)
                {
                    if (s == User)       //是否在離線名單 是為true
                    {
                        I.Remove(s);        //離線名單移除名稱
                        HFclt.Value = "alert" + "," + "已重新連線";        //通知已重新連線
                        Panel_Student_CreateCube_Text();
                        relog = true;       //重新登入則Application[Account.Text]會有紀錄就不必匯入Application的基本資料
                        break;
                    }
                }

                Application.Lock();     //鎖定網站公用變數
                L.Add(Account.Text, DateTime.Now);    //將使用者時間加至Hashtable
                if (relog == false)     //是否重新登入  不是為true
                {
                    Application[Account.Text] = Account.Text;       //不是重新登入就要匯入Application的基本資料
                    Application[Account.Text + "grades"] = 0;
                    int C = int.Parse(Application["Cube"].ToString());      //取得Cube大小
                    Panel_Student_CreateCube_Text();        //建立冰狗版面
                }
                Application.UnLock();       //解除鎖定

                if (User == Teacher)        //老師登入
                {
                    Panel_Teacher.Visible = true;       //顯示老師操作畫面
                    ListBox1.Visible = true;        //顯示登入名單
                    ListBox2.Visible = true;        //顯示離線名單
                    Label1.Visible = true;
                    Label2.Visible = true;
                }
                else        //學生登入
                {
                    Panel_Student.Visible = true;       //學生操作畫面
                    ViewState["tempCube"] = -1;     //先給值 不然後面那個第一次取值的時候是null
                }
            }
            else        //每次操作時避免物件被刪除 
            {
                if (Application[Account.Text].ToString() == Teacher)        //使用者為老師
                {

                }
                else                                                        //使用者為學生
                {
                    if (ViewState["tempCube"].ToString() != Application["Cube"].ToString())     //如果兩個不相同 代表可重新更改數字
                    {
                        if (Application["Start"].ToString() != "true")      //但要在開始之前
                        {
                            Application[Account.Text + "M"] = null;     //會清除Application[Account.Text + "M"]的內容
                            Panel_Student_CreateCube_Text();       //建立輸入數字的TextBox
                        }
                    }
                }
            }

            if(Application[Account.Text].ToString() != Teacher)     //不是老師 true
            {
                if (Application["Start"].ToString() == "true")      //Application["Start"]為true 代表已經開始了
                {
                    if (Application[Account.Text + "M"] != null)        //在Application["Start"]為true之前就填好數字
                    {                                                   //代表資料有紀錄 重新連線也可使用
                        Panel_Student_CreateCube_Image();       //有資料就可以建立冰狗畫面
                    }
                    else        //Application[Account.Text + "M"]沒有資料 並且已經開始了 用亂數產生 再存入Application[Account.Text + "M"]
                    {
                        HFclt.Value = "alert" + "," + "已經開始囉!";     //沒有存資料或在開始後才連進來的提醒已經開始了

                        int Cube = int.Parse(Application["Cube"].ToString());       //取得Cube
                        int[] n = new int[Cube * Cube];     //用陣列儲存每個TextBox內的數字
                        int index = -1;     //搜尋數字是否重複
                        Random rd = new Random();
                        for (int i = 0; i < Cube * Cube; i++)
                        {
                            int m = rd.Next(1, 76);
                            index = Array.IndexOf(n, m);
                            n[i] = m;
                            if (index >= 0) i--;        //如果搜尋到重複的數字  就重新再做一遍
                        }
                        for (int i = 0; i < Cube * Cube; i++)
                        {
                            Application[Account.Text + "M"] += n[i] + "+";
                        }
                        Panel_Student_CreateCube_Image();
                    }
                    Btn_RandomNum.Enabled = false;      //隨機產生數字關閉
                    Btn_Clear.Enabled = false;      //清除版面關閉
                    Btn_Ready.Enabled = false;      //數字輸入完成關閉
                    Btn_CubeChanged.Enabled = false;     //當老師要改變大小 很長那個關閉
                    Label_grades.Visible = true;
                }

                if(Application["End"].ToString() == "true")     //結束訊息
                {
                    Label_grades_Record.Text = "上一輪的分數：" + Application[Account.Text + "grades"].ToString();     //紀錄上一輪的成績
                    Application.Lock();
                    Application[Account.Text + "AnswerQ"] = null;       //刪除用來判斷是否回答過的紀錄
                    Application[Account.Text + "M"] = null;     //刪除原本的球號
                    Application[Account.Text + "grades"] = 0;       //成績歸零
                    Application.UnLock(); 
                    ViewState["tempCube"] = -1;     //檢測是否相同Cube回復成初始值

                    Btn_RandomNum.Enabled = true;      //隨機產生數字開啟
                    Btn_Clear.Enabled = true;      //清除版面開啟
                    Btn_Ready.Enabled = true;      //數字輸入完成開啟
                    Btn_CubeChanged.Enabled = false;     //當老師要改變大小 很長那個開啟
                    Label_grades.Visible = false;   //分數關閉
                }
            }

            if (Application[Account.Text + "M"] != null)        //在Application["Start"]為true之前就填好數字
            {                                                   //代表資料有紀錄 重新連線也可使用
                Panel_Student_CreateCube_Image();       //有資料就可以建立冰狗狗
            }

            if(Application["Win"].ToString() == "true")
            {
                Btn_NextQ.Enabled = false;
                Btn_LockAns.Enabled = false;
            }
        }

        private void Panel_Student_CreateCube_Text()
        {
            int Cube = int.Parse(Application["Cube"].ToString());
            Panel_Student_Cube.Controls.Clear();
            Panel_Student_Cube_Update.Controls.Clear();
            for (int i = 0; i < Cube; i++)
            {
                for (int j = 0; j < Cube; j++)
                {
                    TextBox T = new TextBox();
                    T.ID = "T" + (i * Cube + j).ToString();
                    T.Width = 420 / Cube;
                    T.Height = 420 / Cube;
                    Panel_Student_Cube.Controls.Add(T);     //建立TextBox
                }
                Panel_Student_Cube.Controls.Add(new LiteralControl("<br>"));     //物件於Panel內換行用
            }
        }
        private void Panel_Student_CreateCube_Image(int x = -1)       //需要建立 Label 代表 Application[Account.Text+"M"]有資料 
        {                                                             //引數是用來判斷哪個冰狗球中了
            if(x != -1)     //x不等於-1代表有球號相同
            {
                string[] temp = Application[Account.Text + "M"].ToString().Split('+');      //取得自己的球號
                Application[Account.Text + "M"] = "";
                for(int i = 0; i < temp.Length - 1; i++)
                {
                    if(x == i)      
                        Application[Account.Text + "M"] += temp[i] + "_Circle" + "+";       //把名稱改成有圈起來的圖
                    else
                        Application[Account.Text + "M"] += temp[i] + "+";       //不是就維持原樣
                }
            }
            int Cube = int.Parse(Application["Cube"].ToString());
            Panel_Student_Cube.Controls.Clear();
            Panel_Student_Cube_Update.Controls.Clear();

            string[] M = Application[Account.Text + "M"].ToString().Split('+');     //例：Application[Account.Text + "M"] = 15 + 7 + 12...
            
            for (int i = 0; i < Cube; i++)
            {
                for (int j = 0; j < Cube; j++)
                {
                    System.Web.UI.WebControls.Image I = new System.Web.UI.WebControls.Image();
                    I.ID = "I" + (i * Cube + j).ToString();
                    I.ImageUrl = "Num" + M[i * Cube + j] + ".gif";      //圖片 = Num1.gif
                    I.BorderStyle = BorderStyle.Solid;
                    I.Width = 420 / Cube;
                    I.Height = 420 / Cube;
                    Panel_Student_Cube_Update.Controls.Add(I);     //建立TextBox
                }
                Panel_Student_Cube_Update.Controls.Add(new LiteralControl("<br>"));     //物件於Panel內換行用
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            Application.Lock();     //鎖定
            Hashtable L = (Hashtable)Application["L"];      //取得線上名單
            if (L[Account.Text] == null)       //Hashtable是否有儲存使用者，沒有為ture
            {
                L.Add(Account.Text, DateTime.Now);        //重新儲存teache
            }
            else    //有為false
            {
                L[Account.Text] = DateTime.Now;       //更新使用者Hashtable的時間記錄
            }
            Application.UnLock();       //解鎖
            
            if (Account.Text == Teacher)        //老師端更新ListBox
            {
                ListBox1.Items.Clear();     //清除線上名單的內容
                ListBox2.Items.Clear();     //清除離線名單的內容
                foreach (var i in L.Keys)
                {
                    if (i.ToString() != Account.Text)       //是否為老師的名稱，不是為ture
                    {
                        ListBox1.Items.Add(i.ToString());       //加至ListBox最後一個
                    }
                    else
                    {
                        ListBox1.Items.Insert(0, i.ToString());     //將老師加至ListBox第一個
                    }
                }

                List<string> I = (List<string>)Application["I"];        //取得離線名單
                foreach (string i in I)
                {
                    ListBox2.Items.Add(i);
                }
            }

            if(Application["Ball"] != null && Application["Ball"].ToString() != "")
            {
                Label3.Text = "出現過的球號：" + Application["Ball"].ToString();
                string[] ball = Application["Ball"].ToString().Split('、');
                Label4.Text = "第" + (ball.Length - 1).ToString() + "題" + "   當前球號：" + ball[ball.Length - 2];
                Label4.BackColor = Color.White;
            }
            else
            {
                Label3.Text = "出現過的球號：";
                Label4.Text = "";
                Lbl_TrueOrFalse.Text = "";
            }

            if(Application[Account.Text].ToString() == Teacher)     //定時更新學生成績
            {
                string filePath = System.AppDomain.CurrentDomain.BaseDirectory + "Grades" + Application["Times"].ToString() + ".txt";
                if (!File.Exists(filePath))
                {
                    FileStream fs = File.Create(filePath);
                    fs.Close();
                }
                else
                {
                    File.Delete(filePath); 
                    FileStream fs = File.Create(filePath);
                    fs.Close();
                }
                
                StreamWriter sw = new StreamWriter(filePath);
                foreach (var i in L.Keys)
                {
                    if (i.ToString() == Teacher) continue;
                    string grades = Application[i.ToString() + "grades"].ToString();
                    sw.WriteLine(i.ToString() + "：" + grades + "分");
                }
                List<string> I = (List<string>)Application["I"];        //取得離線名單
                sw.WriteLine("以下為離線名單");
                foreach (string i in I)
                {
                    if (i.ToString() == Teacher) continue;
                    string grades = Application[i.ToString() + "grades"].ToString();
                    sw.WriteLine(i.ToString() + "：" + grades + "分");
                }
                sw.WriteLine();
                sw.WriteLine(DateTime.Now.ToString());
                sw.Flush();
                sw.Close();
            }

            if(Application["Win"].ToString() == "true")
            {
                if(Account.Text == Teacher)
                {
                    Btn_NextQ.Enabled = false;       //下一題按鈕關閉
                    Btn_LockAns.Enabled = false;     //鎖定開啟
                }
                
                Label4.Text = "有人賓果了";
                Label4.BackColor = Color.Yellow;
            }
        }

        protected void Btn3_Click(object sender, EventArgs e)
        {
            Btn3.Enabled = false;
            Btn5.Enabled = true;
            Btn7.Enabled = true;
            Application["Cube"] = 3;        //3x3的大小
            Application["End"] = "false";
        }

        protected void Btn5_Click(object sender, EventArgs e)
        {
            Btn3.Enabled = true;
            Btn5.Enabled = false;
            Btn7.Enabled = true;
            Application["Cube"] = 5;        //5x5的大小
            Application["End"] = "false";
        }

        protected void Btn7_Click(object sender, EventArgs e)
        {
            Btn3.Enabled = true;
            Btn5.Enabled = true;
            Btn7.Enabled = false;
            Application["Cube"] = 7;        //7x7的大小
            Application["End"] = "false";
        }

        protected void Btn_RandomNum_Click(object sender, EventArgs e)
        {
            if (int.Parse(Application["Cube"].ToString()) == 0)     //當Application["Cube"]為零時
                return;

            Btn_Clear_Click(sender, e);     //清除版面數字
            int Cube = int.Parse(Application["Cube"].ToString());       //取得Cube
            int[] n = new int[Cube * Cube];     //用陣列儲存每個TextBox內的數字
            int index = -1;     //搜尋數字是否重複
            Random rd = new Random();
            
            for (int i = 0; i < Cube * Cube; i++)
            {
                int m = rd.Next(1, 10);
                index = Array.IndexOf(n, m);
                n[i] = m;
                if (index >= 0) i--;        //如果搜尋到重複的數字  就重新再做一遍
            }
            foreach (Control c in Panel_Student_Cube.Controls)
            {
                if (c is TextBox)
                {
                    int num = int.Parse(c.ID.Substring(1));     //TextBox的ID為  "T"+數字  取得後面的數字編號  用來當作陣列的指標
                    ((TextBox)c).Text = n[num].ToString();      //把用來陣列內的數字放入TextBox
                }
            }
        }

        protected void Btn_Clear_Click(object sender, EventArgs e)
        {       
            foreach(Control c in Panel_Student_Cube.Controls)
            {
                if(c is TextBox)
                {
                    ((TextBox)c).Text = "";
                }
            }
        }

        protected void Btn_Ready_Click(object sender, EventArgs e)
        {
            if (Application["Start"].ToString() == "true")      //開始時不能使用
            {
                return;
            }
            if (NumberError())      //有錯誤為true
            {
                return;
            }

            Application[Account.Text + "M"] = "";
            foreach (Control c in Panel_Student_Cube.Controls)
            {
                if (c is TextBox)
                {
                    Application[Account.Text + "M"] += ((TextBox)c).Text + "+";     //檢查完成 沒有錯誤時 記錄當前版面
                                                                                    //可用來重新登入時 重製版面
                }
            }
            Panel_Student_CreateCube_Image();
            Btn_RandomNum.Enabled = false;
            Btn_Clear.Enabled = false;
            Btn_Ready.Enabled = false;
            Btn_CubeChanged.Enabled = true;
            ViewState["tempCube"] = Application["Cube"];        //用ViewState紀錄準備時的Cube
        }

        private bool NumberError()
        {
            int n;      //儲存轉換成數字的TextBox.Text

            int Cube = int.Parse(Application["Cube"].ToString());       //取得版面大小 可知道陣列長度
            int[] repeat = new int[Cube * Cube];      //存入TextBox內的數字 用來檢查是否重複  
            int i = 0;      //紀錄目前TextBox是哪一個  可用來當放入repeat的指標
            int index;      //紀錄是否重複 

            bool error = false;     //n是否有問題
            foreach (Control c in Panel_Student_Cube.Controls)
            {
                if (c is TextBox)
                {
                    if (!int.TryParse(((TextBox)c).Text, out n))        //是否能轉換成數字  不能為true
                    {
                        ((TextBox)c).BackColor = System.Drawing.Color.Red;      //背景設為紅色
                        error = true;
                    }
                    else        //能轉換成數字再判斷範圍
                    {
                        if (n >= 76 || n <= 0)      //超出範圍為true
                        {
                            ((TextBox)c).BackColor = System.Drawing.Color.Red;      //背景設為紅色
                            error = true;
                        }
                        else        //未超出範圍
                        {
                            index = Array.IndexOf(repeat, n);
                            if(index != -1)     //重複true
                            {
                                repeat[i] = n;      //數值存入repeat
                                ((TextBox)c).BackColor = System.Drawing.Color.Red;      //背景設為紅色
                                i++;        //下一個TextBox  以及下一個repeat的指標
                                error = true;
                            }
                            else        //沒有重複
                            {
                                repeat[i] = n;
                                ((TextBox)c).BackColor = System.Drawing.Color.White;
                                i++;
                            }
                        }
                    }
                }
            }
            if (error) return true;
            else return false;
        }

        protected void BtnCreatCube_Click(object sender, EventArgs e)
        {
            //多於的部分
            int c;
            if (int.TryParse(Txt_Cube.Text, out c))
            {
                if (c > 0)
                {
                    if (c < 2147483647)
                    {
                        //if (c % 2 == 1)
                        {
                            Btn3.Enabled = true;
                            Btn5.Enabled = true;
                            Btn7.Enabled = true;
                            Btn_CreatCube.Text = Txt_Cube.Text + "x" + Txt_Cube.Text + "(輸入數字後點擊)";
                            Application["Cube"] = Txt_Cube.Text;
                        }
                        //else
                        {
                            //HFclt.Value = "alert" + "," + "輸入範圍需是基數";
                        }
                    }
                }
                else
                {
                    HFclt.Value = "alert" + "," + "輸入範圍需大於0";
                }
            }
            else
            {
                HFclt.Value = "alert" + "," + "輸入必須是數字";
            }
        }

        protected void Btn_CubeChanged_Click(object sender, EventArgs e)
        {
            if(ViewState["tempCube"].ToString() != Application["Cube"].ToString())  //比較準備時的Cube跟現在的Cube
            {                                                                       //如果不同 可以重新填數字
                Btn_RandomNum.Enabled = true;       //隨機產生數字開啟
                Btn_Clear.Enabled = true;       //清除版面開啟
                Btn_Ready.Enabled = true;       //數字輸入完成開啟
                Btn_CubeChanged.Enabled = false;        //當老師要改變大小 很長那個關閉
                Panel_Student_CreateCube_Text();
            }
            else
            {
                HFclt.Value = "alert" + "," + "老師應該沒有更改大小喔";
                Btn_RandomNum.Enabled = false;      //隨機產生數字關閉
                Btn_Clear.Enabled = false;      //清除版面關閉
                Btn_Ready.Enabled = false;      //數字輸入完成關閉
                Btn_CubeChanged.Enabled = true;     //當老師要改變大小 很長那個開啟
            }
        }

        protected void Btn_Start_Click(object sender, EventArgs e)
        {
            if (Application["Cube"].ToString() == "0")
            {
                HFclt.Value = "alert" + "," + "目前Cube大小為0";
                return;
            }
            Application["Start"] = "true";        //表示開始測驗 會關閉學生除了確定答案以外的操作
            Application["Lock"] = "false";      //開始就會有第一題 解除"鎖定答題"
            Btn_Start.Enabled = false;      //開始按鈕關閉
            Btn_NextQ.Enabled = true;       //下一題按鈕開啟
            Btn_LockAns.Enabled = true;     //鎖定開啟
            Btn_End.Enabled = true;
            Btn3.Enabled = false;       //三個Cube大小按鈕關閉
            Btn5.Enabled = false;
            Btn7.Enabled = false;

            Txt_Cube.Enabled = false;       //測試的兩個物件關閉
            Btn_CreatCube.Enabled = false;

            RandomBall();       //隨機抽球號
        }

        private void RandomBall()
        {
            string[] Ball = new string[0];      //記錄抽過的球號
            if (Application["Ball"] != null)
            {
                Ball = Application["Ball"].ToString().Split('、');      //取得抽過的球號
            }
            Random rd = new Random();
            int BallNum = 0;        //記錄抽到的球號
            int index = 0;      //搜尋是否重複
            while (index != -1)
            {
                BallNum = rd.Next(1, 10);
                index = Array.IndexOf(Ball, BallNum.ToString());
            }
            Application["Ball"] += BallNum.ToString() + "、";      //紀錄球號
        }

        protected void Btn_Ans_Click(object sender, EventArgs e)
        {
            if(Application["Start"].ToString() != "true")
            {
                Txt_Ans.Text = "";
                return;
            }
            if (Application["Lock"].ToString() == "true")
            {
                HFclt.Value = "alert" + "," + "已經不能回答了";
                Txt_Ans.Text = "";
                return;
            }
            if (Application[Account.Text + "AnswerQ"] != null)       //有值代表不是第一題
            {
                if (Application[Account.Text + "AnswerQ"].ToString() == Application["Ball"].ToString())      //兩者相同代表回答過問題了
                {
                    HFclt.Value = "alert" + "," + "已經回答過了";
                    Txt_Ans.Text = "";
                    return;
                }
            }

            string[] Num = Application[Account.Text + "M"].ToString().Split('+');       //分解自己的冰狗盤數字
            string[] ball = Application["Ball"].ToString().Split('、');       //取得現在的球號
            int index = Array.IndexOf(Num, ball[ball.Length - 2]);     //檢查現在的冰狗盤是否有跟球號相同的數字

            if (Txt_Ans.Text == ball[ball.Length - 2])      //答案是對的(測試時答案是題號)
            {
                int n = int.Parse(Application[Account.Text + "grades"].ToString()) + 1;     //取得現在的分數並加一
                if (index != -1)     //有相同球號為true
                {
                    Panel_Student_CreateCube_Image(index);
                    n = ChkLine(index, n);     //檢查是否有連線    第一個引數取得中的球號 第二個為現在分數
                }
                Application[Account.Text + "grades"] = n;       //紀錄分數
                Label_grades.Text = "分數：" + Application[Account.Text + "grades"].ToString();
                Lbl_TrueOrFalse.Text = "答對了";
                if (ChkBingo()) Application["Win"] = "true";    //賓果時紀錄
            }
            else        //答案是錯的
            {
                Lbl_TrueOrFalse.Text = "答錯了";
            }
            Txt_Ans.Text = "";
            Application[Account.Text + "AnswerQ"] = Application["Ball"].ToString();     //回答完就會記錄全部的球號
        }

        private bool ChkBingo()
        {
            int Cube = int.Parse(Application["Cube"].ToString());       //取得Cube大小
            string[] Circle = Application[Account.Text + "M"].ToString().Split('+');        //取得自己的所有球號
                                                                                            //有被圈起來有_Circle
                                                                                            //例：7 + 9_Circle + 54 + 12 + 8_Circle...
            int Bingo = 0;
            for (int i = 0; i < Circle.Length; i++)
            {
                Circle[i] = Circle[i] + "_";       //補一下_ 不然後面會超過陣列長度
            }
            for(int i = 0; i < Cube * Cube; i++)
            {
                string[] temp = Circle[i].Split('_');
                if (temp[1] == "Circle") Bingo++;
            }
            if (Bingo == Cube * Cube) return true;
            else return false;
        }

        private int ChkLine(int index, int n)
        {
            int Cube = int.Parse(Application["Cube"].ToString());       //取得Cube大小
            string[] Circle = Application[Account.Text + "M"].ToString().Split('+');        //取得自己的所有球號
                                                                                            //有被圈起來有_Circle
                                                                                            //例：7 + 9_Circle + 54 + 12 + 8_Circle...
            for(int i = 0; i < Circle.Length; i++)
            {
                Circle[i] = Circle[i] + "_";       //補一下_ 不然後面會超過陣列長度
            }

            int ii;
            int Line = 0;

            ii = index / Cube;      //可知道是哪個橫排 可取得最左邊的指標
            for(int i = 0; i < Cube; i++)       //檢查橫排
            {
                string[] temp = Circle[ii * Cube + i].Split('_');
                if (temp[1] == "Circle")
                {
                    Line += 1;
                    if (Line == Cube) n += 3;       //都有加三分
                }
                else
                {
                    Line = 0;
                    break;
                }
            }
            Line = 0;

            ii = index % Cube;      //可知道是哪個直排 可取得最上面的指標
            for (int i = 0; i < Cube; i++)       //檢查直排
            {
                string[] temp = Circle[ii + i * Cube].Split('_');
                if (temp[1] == "Circle")
                {
                    Line += 1;
                    if (Line == Cube) n += 3;       //同上
                }
                else
                {
                    Line = 0;
                    break;
                }
            }
            Line = 0;


            if (index % (Cube + 1) == 0)        //檢查是否在左上到右下
            {
                for (int i = 0; i < Cube; i++)       //檢查左上到右下   i=0 就可以代表第一個指標 接續指標就+Cube+1
                {
                    string[] temp = Circle[i * (Cube + 1)].Split('_');
                    if (temp[1] == "Circle")
                    {
                        Line += 1;
                        if (Line == Cube) n += 3;
                    }
                    else
                    {
                        Line = 0;
                        break;
                    }
                }
            }
            Line = 0;

            if (index % (Cube - 1) == 0)
            {
                for (int i = 0; i < Cube; i++)       //檢查右上到左下   Cube-1 代表第一個指標 接續指標就+Cube-1
                {
                    string[] temp = Circle[(Cube - 1) + i * (Cube - 1)].Split('_');
                    if (temp[1] == "Circle")
                    {
                        Line += 1;
                        if (Line == Cube) n += 3;
                    }
                    else
                    {
                        Line = 0;
                        break;
                    }
                }
            }
            Line = 0;

            return n;
        }

        protected void Btn_LockAns_Click(object sender, EventArgs e)
        {
            Application["Lock"] = "true";
            Btn_LockAns.Text = "已鎖定答題";
        }

        protected void Btn_NextQ_Click(object sender, EventArgs e)
        {
            if (Application["Win"].ToString() == "true")
                return;

            Application["Lock"] = "false";
            RandomBall();
            Btn_LockAns.Text = "鎖定答題";
        }

        protected void Btn_End_Click(object sender, EventArgs e)
        {
            Application.Lock();
            Application["Cube"] = 0;        //開啟時不設定Cube大小

            Application["Start"] = "false";       //老師是否開始冰狗狗設為false

            Application["Lock"] = "true";      //開始的Lock為true 直到老師開始按鈕啟動

            Application["Ball"] = "";

            Application["Times"] = int.Parse(Application["Times"].ToString()) + 1;

            Application["End"] = "true";        //結束這局

            Application["Win"] = "false";       //有賓果的人 通知關閉
            Application.UnLock();

            Btn_Start.Enabled = true;      //開始按鈕開啟
            Btn_NextQ.Enabled = false;       //下一題按鈕關閉
            Btn_LockAns.Enabled = false;     //鎖定開啟
            Btn_End.Enabled = false;        //結束 關閉
            Btn3.Enabled = true;       //三個Cube大小按鈕開啟
            Btn5.Enabled = true;
            Btn7.Enabled = true;

            Txt_Cube.Enabled = true;       //測試的兩個物件關閉
            Btn_CreatCube.Enabled = true;

            List<string> I = (List<string>)Application["I"];        //用老師的網站將離線的人的資料清除
            foreach (string i in I)
            {
                Application.Lock();
                Application[i + "AnswerQ"] = null;       //刪除用來判斷是否回答過的紀錄
                Application[i + "M"] = null;     //刪除原本的球號
                Application[i + "grades"] = 0;       //成績歸零
                Application.UnLock();
            }
        }

        protected void Btn_Timer_Click(object sender, EventArgs e)
        {
            if (Btn_Timer.Text == "Timer停止")        //用於最後一場的冰狗狗 離線名單可能會有很多多餘的名字 暫停後
            {
                Timer1.Enabled = false;
                Btn_Timer.Text = "Timer繼續";
            }
            else
            {
                Timer1.Enabled = true;
                Btn_Timer.Text = "Timer停止";
            }
        }
    }
}