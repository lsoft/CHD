using System;
using System.Net;

namespace MailRu.Cloud.WebApi.Connection
{
    internal class Requisites
    {
        public string Login
        {
            get;
            private set;
        }

        public string Password
        {
            get;
            private set;
        }
        
        public CookieContainer Cookies
        {
            get;
            private set;
        }

        public WebProxy Proxy
        {
            get;
            private set;
        }

        public string AuthToken
        {
            get;
            private set;
        }

        public Requisites(
            string login, 
            string password, 
            CookieContainer cookies, 
            WebProxy proxy, 
            string authToken
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
            if (authToken == null)
            {
                throw new ArgumentNullException("authToken");
            }

            Login = login;
            Password = password;
            Cookies = cookies;
            Proxy = proxy;
            AuthToken = authToken;
        }
    }
}