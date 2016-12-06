using System;
using System.Web;

namespace MailRu.Cloud.WebApi
{
    public class ServerPath
    {
        private static readonly ServerPath _empty = new ServerPath(string.Empty);
        private static readonly ServerPath _root  = new ServerPath("/");

        private readonly string _path;

        public ServerPath(
            string path
            )
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            if (!string.IsNullOrEmpty(path))
            {
                _path = path;
            }
            else
            {
                _path = "/";
            }
        }


        public ServerPath GetElementsWithoutSuffix(
            string suffix
            )
        {
            if (suffix == null)
            {
                throw new ArgumentNullException("suffix");
            }

            return
                new ServerPath(
                    _path.Substring(0, _path.LastIndexOf(suffix))
                );
        }

        public string GetPath()
        {
            return
                _path;
        }

        public string GetUrlEncodedPath()
        {
            return
                HttpUtility.UrlEncode(_path);
        }

        public override string ToString()
        {
            return
                GetPath();
        }


        public ServerPath GetLastElement(
            )
        {
            if (!_path.Contains("/"))
            {
                return
                    this;
            }

            return
                new ServerPath(
                    _path.Substring(0, _path.LastIndexOf("/") + 1)
                );
        }

        public ServerPath SafelyAppend(
            string suffix
            )
        {
            if (suffix == null)
            {
                throw new ArgumentNullException("suffix");
            }

            if (_path.EndsWith(suffix))
            {
                return
                    this;
            }

            return
                new ServerPath(
                    _path + suffix
                    );
        }

        public ServerPath Combine(
            string suffix
            )
        {
            if (suffix == null)
            {
                throw new ArgumentNullException("suffix");
            }

            return
                new ServerPath(
                    _path.EndsWith("/") ? _path + suffix : _path + "/" + suffix
                    );
        }

        public static ServerPath Empty
        {
            get
            {
                return
                    _empty;
            }
        }

        public static ServerPath Root
        {
            get
            {
                return
                    _root;
            }
        }
    }
}
