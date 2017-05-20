using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace AquaBot
{
    // http://safebooru.org/index.php?page=help&topic=dapi

    public class Safebooru : SearchImage
    {
        private readonly string BaseURL;
        private readonly string SearchURL;

        private readonly string TagsName;
        private readonly string PageName;
        private readonly string LimitName;

        public Safebooru()
        {
            this.BaseURL = new Uri("http://safebooru.org/index.php?page=dapi&s=post&q=index").GetComponents(UriComponents.StrongAuthority, UriFormat.Unescaped);
            if (this.BaseURL[this.BaseURL.Length - 1] == '/')
                this.BaseURL = this.BaseURL.Substring(0, this.BaseURL.Length - 1);

            this.SearchURL = "http://safebooru.org/index.php?page=dapi&s=post&q=index";
            this.TagsName = "tags";
            this.PageName = "pid";
            this.LimitName = "limit";
        }

        internal override Uri RequestURL(SearchOption option)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append(this.SearchURL);
            if (this.SearchURL.IndexOf('?') < 0)
                sbQuery.Append('?');
            else
                sbQuery.Append('&');

            if (!String.IsNullOrEmpty(option.Tags))
                sbQuery.AppendFormat("{0}={1}&", TagsName, Uri.EscapeUriString(option.Tags));

            if (option.Page != null)
                sbQuery.AppendFormat("{0}={1}&", PageName, option.Page.Value);

            if (option.Limit != null)
                sbQuery.AppendFormat("{0}={1}&", LimitName, option.Limit.Value);

            if (sbQuery[sbQuery.Length - 1] == '&')
                sbQuery.Remove(sbQuery.Length - 1, 1);

            if (sbQuery[sbQuery.Length - 1] == '?')
                sbQuery.Remove(sbQuery.Length - 1, 1);

            return new Uri(sbQuery.ToString());
        }

        internal override IList<ImageInfo> ParseData(string body, SearchOption option)
        {
            return this.ParseDataXml(body, option);
        }

        internal override int ParsePostCount(string body)
        {
            return this.ParsePostCountXml(body);
        }

        private int ParsePostCountXml(string body)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(body);

            XmlNodeList postNodes = xmlDocument.GetElementsByTagName("posts");

            return postNodes[0].GetInt32("count");
        }

        private IList<ImageInfo> ParseDataXml(string body, SearchOption option)
        {
            List<ImageInfo> lstResult = new List<ImageInfo>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(body);

            XmlNodeList postNodes = xmlDocument.GetElementsByTagName("post");

            foreach (XmlNode postNode in postNodes)
            {
                ImageInfo info = new ImageInfo()
                {
                    ImageId = postNode.GetString("id"),

                    Source = postNode.GetString("source"),

                    OrigUrl = postNode.GetString("file_url"),
                    OrigFileSize = postNode.GetInt64("file_size"),
                    OrigWidth = postNode.GetInt32("width"),
                    OrigHeight = postNode.GetInt32("height"),

                    SampleUrl = postNode.GetString("sample_url"),
                    SampleFileSize = postNode.GetInt64("sample_file_size"),
                    SampleWidth = postNode.GetInt32("sample_width"),
                    SampleHeight = postNode.GetInt32("sample_height"),

                    ThumbUrl = postNode.GetString("preview_url"),
                    ThumbWidth = postNode.GetInt32("preview_width"),
                    ThumbHeight = postNode.GetInt32("preview_height")
                };
                this.CheckURL(info);

                lstResult.Add(info);
            }

            return lstResult.AsReadOnly();
        }

        private void CheckURL(ImageInfo info)
        {
            if (info.OrigUrl.StartsWith("//"))
                info.OrigUrl = "http:" + info.OrigUrl;
            if (info.SampleUrl.StartsWith("//"))
                info.SampleUrl = "http:" + info.SampleUrl;
            if (info.ThumbUrl.StartsWith("//"))
                info.ThumbUrl = "http:" + info.ThumbUrl;

            if (info.OrigUrl != null && !info.OrigUrl.StartsWith("http://") && !info.OrigUrl.StartsWith("https://"))
                info.OrigUrl = this.BaseURL + info.OrigUrl;

            if (info.SampleUrl != null && !info.SampleUrl.StartsWith("http://") && !info.SampleUrl.StartsWith("https://"))
                info.SampleUrl = this.BaseURL + info.SampleUrl;

            if (info.ThumbUrl != null && !info.ThumbUrl.StartsWith("http://") && !info.ThumbUrl.StartsWith("https://"))
                info.ThumbUrl = this.BaseURL + info.ThumbUrl;
        }
    }
}