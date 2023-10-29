using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Collections;
using Microsoft.VisualBasic;
using System.Threading;

namespace dog
{
    public class Global : System.Web.HttpApplication
    {
        Thread Th;

        protected void Application_Start(object sender, EventArgs e)
        {
            Hashtable HT = new Hashtable();     //儲存線上名單
            Application["L"] = HT;

            List<string> Lst = new List<string>();      //儲存離線名單
            Application["I"] = Lst;

            Application["Cube"] = 0;        //開啟時不設定Cube大小

            Application["Start"] = "false";       //老師是否開始冰狗狗設為false

            Application["Lock"] = "true";      //開始的Lock為true 直到老師開始按鈕啟動

            Application["Times"] = 1;

            Application["End"] = "false";

            Application["Win"] = "false";

            Th = new Thread(chkList);
            Th.IsBackground = true;
            Th.Start();
        }

        private void chkList()
        {
            while (true)
            {
                Hashtable L = (Hashtable)Application["L"];      //線上名單
                List<string> I = (List<string>)Application["I"];      //離線名單
                if (L.Count > 0)
                {
                    Application.Lock();
                    Hashtable G = new Hashtable();      //備份線上名單
                    foreach(var h in L.Keys)
                    {
                        if (h.ToString() != "")
                        {
                            DateTime t = (DateTime)L[h];
                            double s = new TimeSpan(DateTime.Now.Ticks - t.Ticks).TotalSeconds;
                            if ((int)s <= 2)     //逾時兩秒視為離線
                            {
                                G.Add(h, t);
                            }
                            else      //
                                I.Add(h.ToString());                            
                        }
                    }
                    Application["L"] = G;
                    Application["I"] = I;
                    Application.UnLock();
                }
                System.Threading.Thread.Sleep(100);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            Th.Abort();
        }
    }
}