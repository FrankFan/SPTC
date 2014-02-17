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
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            DoQueryCardBalance();
        }

        protected void DoQueryCardBalance()
        {
            string balance = string.Empty;
            string deadLine = string.Empty;

            string strCardNum = this.txtCardNo.Text.Trim();
            string postUrl = @"http://jtk.sptcc.com:8080/servlet";
            string postedData = "Card_id={0}&x=43&y=17&hiddentype=index&addr=114.80.133.7";
            postedData = string.Format(postedData, strCardNum);


            HttpHeader header = new HttpHeader()
            {
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                ContentType = "application/x-www-form-urlencoded",
                Method = "POST",
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36"
            };

            string location = string.Empty;
            CookieContainer cc = HttpHelper.GetCookie(postUrl, postedData, header, out location);

            //string location = "http://jtk.sptcc.com:8080/Member_balance.jsp?card_num=NDY4MDU1NzAzMDM=&time=MjAxNDAyMTQ=&card_balance=NDAyMA==";

            string html = HttpHelper.GetHtmlByCookie(location, cc, header);

            string pattern = @"<font class=""bodystyle"">\s*至\s*(?<date>.*?)\s*用户余额为\s*(?<result>.*?)\s*元\s*?";
            //@"<font class=""bodystyle"">\s*至\s*用户余额为\s*(?<result>.*?)\s*元\s*?";
            Match match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);

            while (match.Success)
            {
                balance = match.Groups["result"].Value;
                deadLine = match.Groups["date"].Value;
                match = match.NextMatch();
            }
            

            this.lblResult.Text = string.Format("截止到{0}用于余额为{1}", deadLine, balance);
            this.lblResult.ForeColor = Color.Red;

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Specify that the link was visited. 
            //this.linkLabel1.LinkVisited = true;

            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://www.weibo.com/OObigO");
        }
    }
}
