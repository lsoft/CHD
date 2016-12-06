using System;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Text;

namespace MailRu.Cloud.WebApi.Connection.Factory
{
    public class ConnectionFactory : IConnectionFactory
    {
        public IConnection OpenConnection(
            string login,
            string password
            )
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            var cookies = new CookieContainer();

            WebProxy proxy = null;
            if (new DefaultProxySection().Enabled)
            {
                proxy = WebProxy.GetDefaultProxy();
            }

            var authToken = Authentificate(
                login,
                password,
                cookies,
                proxy
                );

            var req = new Requisites(
                login,
                password,
                cookies,
                proxy,
                authToken
                );

            var result = new Connection(
                req
                );

            return
                result;
        }

        private string Authentificate(
            string login,
            string password,
            CookieContainer cookies,
            WebProxy proxy
            )
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            if (cookies == null)
            {
                throw new ArgumentNullException("cookies");
            }
            if (proxy == null)
            {
                throw new ArgumentNullException("proxy");
            }

            string reqString = string.Format("Login={0}&Domain={1}&Password={2}", login, ConstSettings.Domain, password);
            byte[] requestData = Encoding.UTF8.GetBytes(reqString);
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/cgi-bin/auth", ConstSettings.AuthDomen));
            request.Proxy = proxy;
            request.CookieContainer = cookies;
            request.Method = "POST";
            request.ContentType = ConstSettings.DefaultRequestType;
            request.Accept = ConstSettings.DefaultAcceptType;
            request.UserAgent = ConstSettings.UserAgent;

            using (Stream s = request.GetRequestStream())
            {
                s.Write(requestData, 0, requestData.Length);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new InvalidOperationException("Cannot connect to Cloud Api");
                    }

                    if (cookies.Count <= 0)
                    {
                        throw new InvalidOperationException("Cannot connect to Cloud Api");
                    }
                    
                    EnsureSdcCookie(
                        cookies,
                        proxy
                        );

                    var authToken = GetAuthToken(
                        cookies,
                        proxy
                        );

                    if (string.IsNullOrEmpty(authToken))
                    {
                        throw new InvalidOperationException("Invalid login or password");
                    }

                    return
                        authToken;
                }
            }
        }

        /// <summary>
        ///     Retrieve SDC cookies.
        /// </summary>
        private void EnsureSdcCookie(
            CookieContainer cookies,
            WebProxy proxy
            )
        {
            if (cookies == null)
            {
                throw new ArgumentNullException("cookies");
            }
            if (proxy == null)
            {
                throw new ArgumentNullException("proxy");
            }

            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/sdc?from={1}/home", ConstSettings.AuthDomen, ConstSettings.CloudDomain));
            request.Proxy = proxy;
            request.CookieContainer = cookies;
            request.Method = "GET";
            request.ContentType = ConstSettings.DefaultRequestType;
            request.Accept = ConstSettings.DefaultAcceptType;
            request.UserAgent = ConstSettings.UserAgent;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new InvalidOperationException("Cannot connect to Cloud Api");
                }
            }

        }

        /// <summary>
        ///     Get authorization token.
        /// </summary>
        /// <returns>True or false result operation.</returns>
        private string GetAuthToken(
            CookieContainer cookies,
            WebProxy proxy
            )
        {
            if (cookies == null)
            {
                throw new ArgumentNullException("cookies");
            }
            if (proxy == null)
            {
                throw new ArgumentNullException("proxy");
            }

            string authToken = null;

            var uri = new Uri(string.Format("{0}/api/v2/tokens/csrf", ConstSettings.CloudDomain));
            var request = (HttpWebRequest)WebRequest.Create(uri.OriginalString);
            request.Proxy = proxy;
            request.CookieContainer = cookies;
            request.Method = "GET";
            request.ContentType = ConstSettings.DefaultRequestType;
            request.Accept = "application/json";
            request.UserAgent = ConstSettings.UserAgent;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    authToken = JsonParser.Parse(Helper.ReadResponseAsText(response), PObject.Token) as string;
                }
            }

            return
                authToken;
        }
    }
}