using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

public class TestHttpPost : MonoBehaviour
{

    private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

    private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
    {
        return true; //总是接受     
    }

    public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, Encoding charset)
    {
        HttpWebRequest request = null;
        //HTTPSQ请求  
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
        request = WebRequest.Create(url) as HttpWebRequest;
        request.ProtocolVersion = HttpVersion.Version10;
        request.Method = "POST";
        //request.ContentType = "application/x-www-form-urlencoded";
        request.ContentType = "application/json";
        request.UserAgent = DefaultUserAgent;
        //如果需要POST数据     
        if (!(parameters == null || parameters.Count == 0))
        {
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            foreach (string key in parameters.Keys)
            {
                if (i > 0)
                {
                    buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                }
                else
                {
                    buffer.AppendFormat("{0}={1}", key, parameters[key]);
                }
                i++;
            }
            byte[] data = charset.GetBytes(buffer.ToString());
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }
        return request.GetResponse() as HttpWebResponse;
    }

    void TestHttpPostMsg()
    {
        string url = "http://game.hdiandian.com/wxbackstage/memberAccount/spend";
        //string url = "https://192.168.1.101/httpOrg/create";
        Encoding encoding = Encoding.GetEncoding("utf-8");
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("memberId", "93124");
        parameters.Add("account", "100");
        //parameters.Add("authuser", "*****");
        //parameters.Add("authpass", "*****");
        //parameters.Add("orgkey", "*****");
        //parameters.Add("orgname", "*****");
        HttpWebResponse response = CreatePostHttpResponse(url, parameters, encoding);
        //打印返回值  
        Stream stream = response.GetResponseStream();   //获取响应的字符串流  
        StreamReader sr = new StreamReader(stream); //创建一个stream读取流  
        string html = sr.ReadToEnd();   //从头读到尾，放到字符串html  
        //Console.WriteLine(html);
        Debug.Log("html == " + html);
    }

    private void Start()
    {
        TestHttpPostMsg();
    }
}
