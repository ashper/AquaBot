using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AquaBot
{
    public abstract class SearchImage
    {
        public abstract int EngineID { get; }

        public async Task<IList<ImageInfo>> Search(SearchOption option)
        {
            Uri uriRequest = this.RequestURL(option);
            byte[] uriBody = this.RequestBody(option);

            HttpWebRequest wReq = WebRequest.Create(this.RequestURL(option)) as HttpWebRequest;
            wReq.Method = (uriBody != null) ? "POST" : "GET";
            if (uriBody != null)
            {
                wReq.ContentType = "application/x-www-form-urlencoded";
                (await wReq.GetRequestStreamAsync()).Write(uriBody, 0, uriBody.Length);
            }

            HttpWebResponse wRes = await wReq.GetResponseAsync() as HttpWebResponse;

            string body = Helper.StringFromStream(wRes.GetResponseStream());

            return this.ParseData(body, option);
        }

        public async Task<ImageInfo> SearchRandom(SearchOption option)
        {
            option.Limit = 1;
            Uri uriRequest = this.RequestURL(option);
            byte[] uriBody = this.RequestBody(option);

            HttpWebRequest wReq = WebRequest.Create(this.RequestURL(option)) as HttpWebRequest;
            wReq.Method = (uriBody != null) ? "POST" : "GET";
            if (uriBody != null)
            {
                wReq.ContentType = "application/x-www-form-urlencoded";
                (await wReq.GetRequestStreamAsync()).Write(uriBody, 0, uriBody.Length);
            }

            HttpWebResponse wRes = await wReq.GetResponseAsync() as HttpWebResponse;

            string body = Helper.StringFromStream(wRes.GetResponseStream());

            int postCount = this.ParsePostCount(body);

            Random postPicker = new Random();
            var chosenPost = postPicker.Next(0, postCount - 1);

            option.Page = chosenPost;

            wReq = WebRequest.Create(this.RequestURL(option)) as HttpWebRequest;
            wReq.Method = (uriBody != null) ? "POST" : "GET";
            if (uriBody != null)
            {
                wReq.ContentType = "application/x-www-form-urlencoded";
                (await wReq.GetRequestStreamAsync()).Write(uriBody, 0, uriBody.Length);
            }

            wRes = await wReq.GetResponseAsync() as HttpWebResponse;

            body = Helper.StringFromStream(wRes.GetResponseStream());

            return this.ParseData(body, option).First();
        }

        internal abstract Uri RequestURL(SearchOption option);

        internal abstract byte[] RequestBody(SearchOption option);

        internal abstract IList<ImageInfo> ParseData(string body, SearchOption option);

        internal abstract int ParsePostCount(string body);
    }
}