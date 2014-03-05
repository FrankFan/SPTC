/*
* Http处理请求类
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace SPTC_Query
{
    public class HttpHelper
    {
        public static string HttpGet(string url, string postedData)
        {
            string urlParam = string.Empty;
            if (string.IsNullOrEmpty(postedData))
            {
                urlParam = "";
            }
            else
            {
                urlParam = "?" + postedData;
            }

            //create a http request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            //get http response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //get stream
            Stream responseStream = response.GetResponseStream();
            //通过StreamReader读取Stream流中的内容
            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
            //将流转化成字符串
            string html = reader.ReadToEnd();

            //close resource
            responseStream.Close();
            reader.Close();

            return html;
        }

        /// <summary>
        /// 获取cookie
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="postedData"></param>
        /// <param name="header"></param>
        /// <returns></returns>
        public static CookieContainer GetCookie(string loginUrl, string postedData, HttpHeader header,out string location)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            CookieContainer cc = new CookieContainer();

            try
            {
                //准备发起请求
                request = (HttpWebRequest)WebRequest.Create(loginUrl);
                request.Method = header.Method;
                request.ContentType = header.ContentType;
                byte[] postDataByte = Encoding.UTF8.GetBytes(postedData);
                request.ContentLength = postDataByte.Length;
                request.CookieContainer = cc;
                request.KeepAlive = true;
                request.AllowAutoRedirect = false;

                //提交请求
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postDataByte, 0, postDataByte.Length);
                requestStream.Close();

                //接收响应
                response = (HttpWebResponse)request.GetResponse();
                response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);

                WebHeaderCollection whc = response.Headers;

                location = response.Headers["Location"];

                CookieCollection cookieCollection = response.Cookies;

                cc.Add(cookieCollection);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return cc;
        }

        /// <summary>
        /// Get方式获携带cookie获取页面html
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <param name="cc">cookie</param>
        /// <param name="header">请求Header对象</param>
        /// <returns>页面源代码html</returns>
        public static string GetHtmlByCookie(string url, CookieContainer cc, HttpHeader header)
        {
            string html = string.Empty;
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader streamReader = null;
            Stream responseStream = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = cc;
                request.ContentType = header.ContentType;
                //request.ServicePoint.ConnectionLimit = header.MaxTry;
                request.Referer = url;
                request.Accept = header.Accept;
                request.UserAgent = header.UserAgent;
                request.Method = "GET";

                //发起请求，得到Response
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                streamReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312"));
                html = streamReader.ReadToEnd();


            }
            catch (Exception ex)
            {
                if (request != null)
                    request.Abort();
                if (response != null)
                    response.Close();
                return string.Empty;
            }
            finally
            {
                //关闭各种资源

                streamReader.Close();
                responseStream.Close();
                request.Abort();
                response.Close();
            }


            return html;
        }
    }
}
