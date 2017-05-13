using System;
using System.Net;

namespace AquaBot
{
    internal class ApiObject
    {
        public HttpWebRequest hwReq;
        public ApiResult asyncApi;
        public IAsyncResult asyncHttp;
        public AsyncCallback callback;
        public SearchOption option;
        public object userState;
        public string Body;
    }
}