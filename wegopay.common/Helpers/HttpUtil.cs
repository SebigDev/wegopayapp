using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace wegopay.common.Helpers
{
    public class HttpUtil
    {
        private readonly HttpClient _client;

        public HttpUtil()
        {
            var webProxy = new WebProxy(new Uri("http://172.27.4.3:80"), BypassOnLocal: false);
            var proxyHttpClientHandler = new HttpClientHandler
            {
                Proxy = webProxy,
                UseProxy = false, //bool.Parse(_settings.IsLive) ? false : true,
                DefaultProxyCredentials = System.Net.CredentialCache.DefaultNetworkCredentials,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            _client = new HttpClient(proxyHttpClientHandler);
        }

        private async Task<T> PostContent<T>(string url, object payload, bool isAsync)
        {
            try
            {
                string rsp = string.Empty;
                string data = JsonConvert.SerializeObject(payload);
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                };

                var response = new HttpResponseMessage();
                if (isAsync)
                    response = await _client.SendAsync(request);
                else
                    response = _client.SendAsync(request).Result;

                rsp = await response.Content.ReadAsStringAsync();// response.Content;
                T res = JsonConvert.DeserializeObject<T>(rsp);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return default(T);
        }

        public T Post<T>(string url, object payload)
        {
            return PostContent<T>(url, payload, false).Result;
        }

        public async Task<T> PostAsync<T>(string url, object payload)
        {
            return await PostContent<T>(url, payload, true);
        }

        private async Task<dynamic> PostContent(string url, object payload, bool isAsync)
        {
            try
            {
                string rsp = string.Empty;
                string data = JsonConvert.SerializeObject(payload);
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(data, Encoding.UTF8, "application/json")
                };

                var response = new HttpResponseMessage();
                if (isAsync)
                    response = await _client.SendAsync(request);
                else
                    response = _client.SendAsync(request).Result;

                rsp = await response.Content.ReadAsStringAsync();// response.Content;
                    var res = JsonConvert.DeserializeObject(rsp);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public dynamic Post(string url, object payload)
        {
            return PostContent(url, payload, false).Result;
        }

        public async Task<dynamic> PostAsync(string url, object payload)
        {
            return await PostContent(url, payload, true);
        }
    }
}
