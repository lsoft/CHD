using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using MailRu.Cloud.WebApi.Shard;
using Newtonsoft.Json.Linq;

namespace MailRu.Cloud.WebApi
{
    /// <summary>
    ///     JSON parser to object.
    /// </summary>
    internal static class JsonParser
    {
        /// <summary>
        ///     Main parse function JSON context.
        /// </summary>
        /// <param name="response">Response as text, included JSON.</param>
        /// <param name="parseObject">Object type to parsing.</param>
        /// <param name="param">Additional parameters.</param>
        /// <returns>Parsed object.</returns>
        public static object Parse(string response, PObject parseObject, object param = null)
        {
            if (string.IsNullOrEmpty(response))
            {
                throw new ArgumentNullException("Response text is null or empty.");
            }

            //// Cancellation token.
            if (response == "7035ba55-7d63-4349-9f73-c454529d4b2e")
            {
                return null;
            }

            var parsedJObject = JObject.Parse(response);

            var httpStatusCode = (int)parsedJObject["status"];
            if (httpStatusCode != (int)HttpStatusCode.OK)
            {
                throw new HttpListenerException(httpStatusCode);
            }

            switch (parseObject)
            {
                case PObject.Token:
                    return
                        (string)parsedJObject["body"]["token"];

                case PObject.Entry:
                    var filesCount = (int)parsedJObject["body"]["count"]["files"];
                    var foldersCount = (int)parsedJObject["body"]["count"]["folders"];
                    var files = new List<File>();
                    var folders = new List<Folder>();
                    var entryPath = (string)parsedJObject["body"]["home"];
                    foreach (JToken item in parsedJObject["body"]["list"])
                    {
                        var type = (string)item["type"];
                        var name = (string)item["name"];
                        var size = (long)item["size"];
                        var path = (string)item["home"];
                        string weblink = string.Empty;
                        if (item["weblink"] != null)
                        {
                            weblink = ConstSettings.PublishFileLink + (string)item["weblink"];
                        }

                        if (type == "folder")
                        {
                            folders.Add(
                                new Folder
                                {
                                    NumberOfFolders = (int)item["count"]["folders"],
                                    NumberOfFiles = (int)item["count"]["files"],
                                    Size = size,
                                    FullPath = new MailRuCloudPath(path),
                                    Name = name,
                                    PublicLink = weblink
                                });
                        }
                        else if (type == "file")
                        {
                            files.Add(
                                new File
                                {
                                    Size = size,
                                    FullPath = new MailRuCloudPath(path),
                                    Name = name,
                                    Hash = (string)item["hash"],
                                    PublicLink = weblink,
                                    PrimaryName = name,
                                    PrimarySize = size
                                });
                        }
                    }
                    return
                        new Entry(foldersCount, filesCount, folders, files, entryPath);

                case PObject.Shard:
                    var shardType = param as string;
                    JToken selectedShard = (parsedJObject["body"][shardType] as JArray).First();
                    
                    return
                        new ShardInfo
                        {
                            Type = GetEnumFromDescription<ShardType>(param as string),
                            Count = (int)selectedShard["count"],
                            Url = (string)selectedShard["url"]
                        };

                case PObject.BodyAsString:
                    return
                        (string)parsedJObject["body"];
            }

            return null;
        }

        /// <summary>
        ///     Get object description attribute.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="enumerationValue">Object value.</param>
        /// <returns>Description attribute.</returns>
        public static string GetEnumDescription<T>(this T enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return enumerationValue.ToString();
        }

        /// <summary>
        ///     Get object from description attribute.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="description">Description attribute.</param>
        /// <returns>Recognized object.</returns>
        public static T GetEnumFromDescription<T>(string description)
        {
            Type type = typeof(T);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException();
            }

            foreach (FieldInfo field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(
                    field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }

            throw new ArgumentException("Not found.", "description");
        }
    }
}