using System;
using System.Web;

namespace MailRu.Cloud.WebApi
{
    public sealed class MailRuCloudPath
    {
        private static readonly MailRuCloudPath _empty = new MailRuCloudPath(string.Empty);
        private static readonly MailRuCloudPath _root  = new MailRuCloudPath("/");

        private readonly string _path;

        public MailRuCloudPath(
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


        public MailRuCloudPath GetElementsWithoutSuffix(
            string suffix
            )
        {
            if (suffix == null)
            {
                throw new ArgumentNullException("suffix");
            }

            return
                new MailRuCloudPath(
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


        public string GetLastElement(
            )
        {
            if (!_path.Contains("/"))
            {
                return
                    string.Empty;
            }
            if (_path.EndsWith("/"))
            {
                return
                    string.Empty;
            }

            return
                _path.Substring(_path.LastIndexOf("/") + 1);
        }

        public MailRuCloudPath CutLastElement(
            )
        {
            if (!_path.Contains("/"))
            {
                return
                    this;
            }

            return
                new MailRuCloudPath(
                    _path.Substring(0, _path.LastIndexOf("/") + 1)
                );
        }

        public MailRuCloudPath SafelyAppend(
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
                new MailRuCloudPath(
                    _path + suffix
                    );
        }

        public MailRuCloudPath Combine(
            string suffix
            )
        {
            if (suffix == null)
            {
                throw new ArgumentNullException("suffix");
            }

            return
                new MailRuCloudPath(
                    _path.EndsWith("/") ? _path + suffix : _path + "/" + suffix
                    );
        }

        public static MailRuCloudPath Empty
        {
            get
            {
                return
                    _empty;
            }
        }

        public static MailRuCloudPath Root
        {
            get
            {
                return
                    _root;
            }
        }
    }
}
