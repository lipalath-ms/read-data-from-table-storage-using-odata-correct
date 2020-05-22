using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace ODatafilter
{
    /// <summary>
    /// HttpHelper
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// Set TimeOut
        /// </summary>
        public static Int32 HttpRequestTimeOut = 10000;

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="url">url</param>
        /// <returns></returns>
        public static HttpResponseData Get(String url)
        {
            return Invoke<Object>("GET", url, null, null);
        }
        public static HttpResponseData GetForOData(String url)
        {
            return InvokeForOData<Object>("GET", url, null, null);
        }
        /// <summary>
        /// GET
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="Id">param</param>
        /// <returns></returns>
        public static HttpResponseData Get(String url, Object Id)
        {
            return Invoke<Object>("GET", url, Id, null);
        }

        /// <summary>
        /// POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="Id">param（optional）</param>
        /// <param name="Data">Data {json format}</param>
        /// <returns></returns>
        public static HttpResponseData Post<T>(String url, Object Id, T Data)
        {
            return Invoke("POST", url, Id, Data);
        }

        /// <summary>
        /// PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>

        /// <param name="url">url</param>

        /// <param name="Data">Data {json format}</param>

        /// <returns></returns>

        public static HttpResponseData Put<T>(String url, T Data)
        {
            return Invoke("PUT", url, null, Data);
        }

        /// <summary>
        /// PUT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">yrl</param>
        /// <param name="Id">URL（optional）</param>
        /// <param name="Data">Data {json format}（optional）</param>
        /// <returns></returns>
        public static HttpResponseData Put<T>(String url, Object Id, T Data)
        {
            return Invoke("PUT", url, Id, Data);
        }

        /// <summary>
        /// DELETE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="Id">param（optional）</param>
        /// <param name="Data">Data {json format}（optional）</param>
        /// <returns></returns>
        public static HttpResponseData Delete<T>(String url, Object Id, T Data)
        {
            return Invoke<Object>("DELETE", url, Id, Data);
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Method"></param>
        /// <param name="url"></param>
        /// <param name="Id"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        private static HttpResponseData Invoke<T>(String Method, String url, Object Id, T Data)
        {
            HttpResponseData Response = new HttpResponseData()
            {
                Code = HttpStatusCode.RequestTimeout,
                Data = String.Empty,
                Message = String.Empty,
            };
            try
            {
                String PostParam = String.Empty;
                if (Data != null)
                {
                    PostParam = Data.ToString();//Newtonsoft.Json.JsonConvert.SerializeObject(Data);
                }
                byte[] postData = Encoding.UTF8.GetBytes(PostParam);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url + (Id == null ? "" : '/' + Id.ToString())));
                request.Method = Method;
                request.ServicePoint.Expect100Continue = false;
                request.Timeout = HttpRequestTimeOut;
                request.ContentType = "application/json";
                request.ContentLength = postData.Length;
                if (postData.Length > 0)
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(postData, 0, postData.Length);
                    }
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Response.Code = response.StatusCode;
                    using (StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        Response.Data = stream.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Message = ex.Message;
            }
            return Response;
        }

        private static HttpResponseData InvokeForOData<T>(String Method, String url, Object Id, T Data)
        {
            HttpResponseData Response = new HttpResponseData()
            {
                Code = HttpStatusCode.RequestTimeout,
                Data = String.Empty,
                Message = String.Empty,
            };
            try
            {
                String PostParam = String.Empty;
                if (Data != null)
                {
                    PostParam = Data.ToString();//Newtonsoft.Json.JsonConvert.SerializeObject(Data);
                }
                byte[] postData = Encoding.UTF8.GetBytes(PostParam);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url + (Id == null ? "" : '/' + Id.ToString())));
                request.Method = Method;
                request.ServicePoint.Expect100Continue = false;
                request.Timeout = HttpRequestTimeOut;
                request.ContentType = "application/json";
                request.ContentLength = postData.Length;
                DateTime now = DateTime.UtcNow;
                request.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                request.Headers.Add("x-ms-version", "2019-07-07");
                request.Headers.Add("Accept", "application/json;odata=fullmetadata");
                if (postData.Length > 0)
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(postData, 0, postData.Length);
                    }
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Response.Code = response.StatusCode;
                    using (StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        Response.Data = stream.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Message = ex.Message;
            }
            return Response;
        }
        /// <summary>
        /// Http Response
        /// </summary>
        public class HttpResponseData
        {
            /// <summary>
            /// Http StatusCode
            /// </summary>
            public HttpStatusCode Code { get; set; }
            /// <summary>
            /// Response Data
            /// </summary>
            public String Data { get; set; }
            /// <summary>
            /// Error Message
            /// </summary>
            public String Message { get; set; }
        }
    }
}