using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace Allegro.Framework.Service
{
    public class Rest: IRest
    {
        public WebHeaderCollection HttpHeaders { get; set; }

        private const string _contentType = "application/json";

        public TResponse PostRequest<TResponse>(string url, object parameter) where TResponse : class
        {
            var jsonData = JsonConvert.SerializeObject(parameter);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers = HttpHeaders;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(jsonData);
                streamWriter.Flush();
                streamWriter.Close();
            }

            return GetStreamReaderResponse<TResponse>(httpWebRequest, "POST");
        }

        public TResponse GetRequest<TResponse>(string url, object parameter = null) where TResponse : class
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = _contentType;
            httpWebRequest.Method = "GET";

            var jsonData = JsonConvert.SerializeObject(parameter);
            if (parameter != null)
                url += ConvertObjectToQueryString(parameter);

            return GetStreamReaderResponse<TResponse>(httpWebRequest, "GET");
        }

        private TResponse GetStreamReaderResponse<TResponse>(HttpWebRequest httpWebRequest, string url, object parameter = null) where TResponse : class
        {
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                if (typeof(TResponse).IsValueType || typeof(TResponse).FullName == "System.String")
                {
                    return (TResponse)Convert.ChangeType(result, typeof(TResponse));
                }
                else
                {
                    return JsonConvert.DeserializeObject<TResponse>(result);
                }
            }
        }

        private string ConvertObjectToQueryString<TParameter>(TParameter parameter) where TParameter : class
        {
            var properties = typeof(TParameter).GetProperties();
            var query = string.Empty;
            foreach (var property in properties)
            {
                if (string.IsNullOrEmpty(query))
                    query += $"?{property.Name}={property.GetValue(parameter)}";
                else
                    query += $"&{property.Name}={property.GetValue(parameter)}";
            }

            return query;
        }
    }
}
