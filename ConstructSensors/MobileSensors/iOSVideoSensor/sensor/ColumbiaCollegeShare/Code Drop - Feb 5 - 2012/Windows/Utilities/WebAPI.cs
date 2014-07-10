using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Utilities
{
    public class WebResult
    {
        public HttpStatusCode StatusCode;
        public HttpWebResponse Response;
        public string ContentText;
    }

    public static class WebAPI
    {
        public const string DefaultUserAgent = "User-Agent: Mozilla/4.0 (compatible; MSIE 5.01; Windows NT 5.0)";
        public const string ApplicationJson = "application/json";

        public static HttpWebRequest BuildRequest(string url, string method, IEnumerable<KeyValuePair<string, string>> headers, string data)
        {
            string postData = null;
            if (string.IsNullOrEmpty(data) == false)
            {
                if (string.Compare(method, "GET", true) == 0)
                {
                    url = url.TrimEnd('?') + "?" + data;
                }
                else
                {
                    postData = data;
                }
            }

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.CreateDefault(new Uri(url));
            request.Method = method;
            foreach (KeyValuePair<string, string> header in headers)
            {
                // some headers have to be modified via specific properties
                if (string.Compare(header.Key, HttpRequestHeader.Accept.ToString(), true) == 0)
                {
                    request.Accept = header.Value;
                }
                else
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (postData != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = bytes.Length;
            }

            return request;
        }

        public static HttpWebRequest BuildRequest(string url, string method)
        {
            return BuildRequest(url, method, GetStandardHeaders("*/*"), null);
        }

        public static HttpWebRequest BuildJsonRequest(string url, string method)
        {
            return BuildJsonRequest(url, method, null);
        }

        public static HttpWebRequest BuildJsonRequest(string url, string method, string data)
        {
            return BuildRequest(url, method, GetStandardHeaders(ApplicationJson), data);
        }

        public static HttpWebRequest BuildRequest(string url)
        {
            return BuildRequest(url, "GET", GetStandardHeaders("*/*"), null);
        }

        public static KeyValuePair<string, string>[] GetStandardHeaders(string accept)
        {
            return new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string,string>( HttpRequestHeader.UserAgent.ToString(), DefaultUserAgent ),
                new KeyValuePair<string,string>( HttpRequestHeader.Accept.ToString(), accept ),
            };
        }

        public static WebResult SendWebRequest(HttpWebRequest request, string postData)
        {
            WebResult result = new WebResult();
            try
            {
                if (postData != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(postData);
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(bytes, 0, bytes.Length);
                    dataStream.Close();
                }

                result.Response = (HttpWebResponse)request.GetResponse();
                result.StatusCode = result.Response.StatusCode;

                using (StreamReader sr = new StreamReader(result.Response.GetResponseStream()))
                {
                    result.ContentText = sr.ReadToEnd();
                }
            }
            catch (WebException wex)
            {
                if (wex.Message.Contains("404"))
                {
                    result.StatusCode = HttpStatusCode.NotFound;
                }

                using (StreamReader sr = new StreamReader(wex.Response.GetResponseStream()))
                {
                    result.ContentText = sr.ReadToEnd();
                }
            }
            catch
            {
                return null;
            }

            return result;
        }

        public static WebResult SendWebRequest(string url, string method, string postData)
        {
            HttpWebRequest request = BuildRequest(url, method, GetStandardHeaders("*/*"), postData);
            return SendWebRequest(request, postData);
        }

        public static WebResult SendJsonWebRequest(string url, string method, string postData)
        {
            HttpWebRequest request = BuildRequest(url, method, GetStandardHeaders(ApplicationJson), postData);
            return SendWebRequest(request, postData);
        }


    }
}
