using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mugen.Network
{
    public static class Network
    {
        //public static string Get(string uri)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //    using (Stream stream = response.GetResponseStream())
        //    using (StreamReader reader = new StreamReader(stream))
        //    {
        //        return reader.ReadToEnd();
        //    }
        //}
        //public static async Task<string> GetAsync(string uri)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        //    using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
        //    using (Stream stream = response.GetResponseStream())
        //    using (StreamReader reader = new StreamReader(stream))
        //    {
        //        return await reader.ReadToEndAsync();
        //    }
        //}
        //public static string Post(string uri, string data, string contentType, string method = "POST")
        //{
        //    byte[] dataBytes = Encoding.UTF8.GetBytes(data);

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        //    request.ContentLength = dataBytes.Length;
        //    request.ContentType = contentType;
        //    request.Method = method;

        //    using (Stream requestBody = request.GetRequestStream())
        //    {
        //        requestBody.Write(dataBytes, 0, dataBytes.Length);
        //    }

        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //    using (Stream stream = response.GetResponseStream())
        //    using (StreamReader reader = new StreamReader(stream))
        //    {
        //        return reader.ReadToEnd();
        //    }
        //}
        //public static async Task<string> PostAsync(string uri, string data, string contentType, string method = "POST")
        //{
        //    byte[] dataBytes = Encoding.UTF8.GetBytes(data);

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        //    request.ContentLength = dataBytes.Length;
        //    request.ContentType = contentType;
        //    request.Method = method;

        //    using (Stream requestBody = request.GetRequestStream())
        //    {
        //        await requestBody.WriteAsync(dataBytes, 0, dataBytes.Length);
        //    }

        //    using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
        //    using (Stream stream = response.GetResponseStream())
        //    using (StreamReader reader = new StreamReader(stream))
        //    {
        //        return await reader.ReadToEndAsync();
        //    }
        //}

        //// Returns JSON string
        //public static string RestGET(string url)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    try
        //    {
        //        WebResponse response = request.GetResponse();
        //        using (Stream responseStream = response.GetResponseStream())
        //        {
        //            StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
        //            return reader.ReadToEnd();
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        WebResponse errorResponse = ex.Response!;
        //        using (Stream responseStream = errorResponse.GetResponseStream())
        //        {
        //            StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
        //            String errorText = reader.ReadToEnd();
        //            // log errorText
        //            Console.WriteLine("Error : Server acces denied !");

        //        }
        //        //throw;
        //        return "ERROR";
        //    }
        //}
    }
}
