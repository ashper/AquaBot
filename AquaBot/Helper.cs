using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace AquaBot
{
    internal static class Helper
    {
        private static readonly DateTime DateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0);

        public static DateTime ParseDateTime(long date)
        {
            return DateTime.SpecifyKind(DateTime1970.AddSeconds(date), DateTimeKind.Local);
        }

        private static readonly string DateFormat = "ddd MMM dd HH:mm:ss zzz yyyy";

        public static DateTime ParseDateTime(string data)
        {
            return DateTime.ParseExact(data, DateFormat, CultureInfo.InvariantCulture);
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

        public static IList<string> MakeTagList(string tags)
        {
            if (String.IsNullOrEmpty(tags))
                return null;
            else
                return new List<string>(tags.Trim().Split(' ')).AsReadOnly();
        }

        public static Ratings ToRating(string rating)
        {
            switch (rating)
            {
                case "s": return Ratings.Safe;
                case "q": return Ratings.Questionable;
                case "e": return Ratings.Explicit;
            }

            return Ratings.All;
        }

        public static bool CompareRating(Ratings searchRating, string targetRating)
        {
            return CompareRating(searchRating, ToRating(targetRating));
        }

        public static bool CompareRating(Ratings searchRating, Ratings targetRating)
        {
            return targetRating <= searchRating;
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

        public static DateTime GetDateTime(this XmlNode node, params string[] key)
        {
            XmlAttribute attr;
            int n;
            long l;
            for (int i = 0; i < key.Length; ++i)
            {
                attr = node.Attributes[key[i]];

                if (attr != null)
                    if (int.TryParse(attr.Value, out n))
                        return ParseDateTime((long)n);
                    else if (long.TryParse(attr.Value, out l))
                        return ParseDateTime(l);
                    else
                        return ParseDateTime(attr.Value);
            }

            return DateTime.MinValue;
        }

        public static IList<string> GetTagList(this XmlNode node, params string[] key)
        {
            return MakeTagList(node.GetString(key));
        }
    }
}