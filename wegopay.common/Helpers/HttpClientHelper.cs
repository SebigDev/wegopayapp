using Flurl.Http.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace wegopay.common.Helpers
{
    public class ProxyHttpClientHandler
    {
        public static HttpClientHandler ProxyHandler(string address)
        {
            var proxyHandler = new HttpClientHandler
            {
                Proxy = string.IsNullOrEmpty(address) ? null : new WebProxy(new Uri(address), BypassOnLocal: false),
                UseProxy = string.IsNullOrEmpty(address) ? false : true,
                DefaultProxyCredentials = System.Net.CredentialCache.DefaultNetworkCredentials,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            return proxyHandler;
        }
    }

    public class ProxyHttpClientFactory : DefaultHttpClientFactory
    {
        private static string Host;

        public ProxyHttpClientFactory(string address)
        {
            Host = address;
        }

        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientHandler
            {
                Proxy = string.IsNullOrEmpty(Host) ? null : new WebProxy(new Uri(Host), BypassOnLocal: false),
                UseProxy = string.IsNullOrEmpty(Host) ? false : true,
                DefaultProxyCredentials = System.Net.CredentialCache.DefaultNetworkCredentials,
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };
        }
    }
}
