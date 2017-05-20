using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AquaBot
{
    internal static class Helper
    {
        public static IList<string> MakeTagList(string tags)
        {
            if (String.IsNullOrEmpty(tags))
                return null;
            else
                return new List<string>(tags.Trim().Split(' ')).AsReadOnly();
        }

        public static string GetString(this XmlNode node, params string[] key)
        {
            XmlAttribute attr;
            for (int i = 0; i < key.Length; ++i)
            {
                attr = node.Attributes[key[i]];
                if (attr != null)
                    return attr.Value;
            }

            return null;
        }

        public static int GetInt32(this XmlNode node, params string[] key)
        {
            XmlAttribute attr;
            for (int i = 0; i < key.Length; ++i)
            {
                attr = node.Attributes[key[i]];
                if (attr != null)
                    return Convert.ToInt32(attr.Value);
            }

            return 0;
        }

        public static long GetInt64(this XmlNode node, params string[] key)
        {
            XmlAttribute attr;
            for (int i = 0; i < key.Length; ++i)
            {
                attr = node.Attributes[key[i]];
                if (attr != null)
                    return Convert.ToInt64(attr.Value);
            }

            return 0;
        }

        public static string StringFromStream(Stream stream)
        {
            string r;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buff = new byte[4096];
                int read;

                while ((read = stream.Read(buff, 0, 4096)) > 0)
                    memoryStream.Write(buff, 0, read);
                memoryStream.Flush();

                r = Encoding.UTF8.GetString(memoryStream.ToArray());

                memoryStream.Dispose();
            }

            return r;
        }
    }
}