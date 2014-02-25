using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;

namespace SPTC_Query
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //设置窗体禁止拖动大小
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //设置窗体禁止最大化
            this.MaximizeBox = false;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            DoQueryCardBalance();
        }

        /// <summary>
        /// 查询交通卡余额方法
        /// </summary>
        protected void DoQueryCardBalance()
        {
            string balance = string.Empty;
            string deadLine = string.Empty;

            string strCardNum = this.txtCardNo.Text.Trim();
            //准备发起post请求的url
            string postUrl = @"http://jtk.sptcc.com:8080/servlet";
            string postedData = "Card_id={0}&x=43&y=17&hiddentype=index&addr=127.0.0.1";
            postedData = string.Format(postedData, strCardNum);

            //Http Header对象
            HttpHeader header = new HttpHeader()
            {
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                ContentType = "application/x-www-form-urlencoded",
                Method = "POST",
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36"
            };

            string location = string.Empty;

            //获得该网站的cookie
            CookieContainer cc = HttpHelper.GetCookie(postUrl, postedData, header, out location);

            //通过location的url获取结果页面的html
            string html = HttpHelper.GetHtmlByCookie(location, cc, header);

            //使用正则表达式匹配结果数据
            string pattern = @"<font class=""bodystyle"">\s*至\s*(?<date>.*?)\s*用户余额为\s*(?<result>.*?)\s*元\s*?";
            Match match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);

            //使用正则匹配
            while (match.Success)
            {
                balance = match.Groups["result"].Value;
                deadLine = match.Groups["date"].Value;
                //不要忘记.NextMatch()
                match = match.NextMatch();
            }
            

            //将查询结果展示到界面
            this.lblResult.Text = string.Format("截止到{0}用于余额为{1}", deadLine, balance);
            this.lblResult.ForeColor = Color.Red;

        }

        /// <summary>
        /// 超链接点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            //this.linkLabel1.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://www.cnblogs.com/fanyong");
        }
    }
}
