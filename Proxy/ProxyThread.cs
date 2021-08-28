using System;
using System.Diagnostics;
using System.Net;

namespace ProxyTester.Proxy
{
    class ProxyThread
    {
        private Proxy _proxy;
        private Uri _url;
        private Stopwatch _stopWatch = new Stopwatch();

        public ProxyThread(Proxy proxy, string url)
        {
            _proxy = proxy;
            _url = new Uri(url);
        }

        public void Start(Object _state)
        {
            _stopWatch.Start();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(_url);

            WebProxy webProxy = new WebProxy(_url, false, new string[0], new NetworkCredential(_proxy.User, _proxy.Pass));
            httpWebRequest.Proxy = webProxy;
            httpWebRequest.Timeout = 3000;

            try
            {
                WebResponse webResponse = httpWebRequest.GetResponse();
                _proxy.ProxyItem.Status = "success";
            }
            catch (WebException)
            {
                _proxy.ProxyItem.Status = "failed";
            }

            _stopWatch.Stop();

            if (_proxy.ProxyItem.Status == "success")
                _proxy.ProxyItem.Speed = _stopWatch.ElapsedMilliseconds.ToString() + "ms";

            _stopWatch.Reset();

            _proxy.UpdateRowThreadSafe();
        }

    }
}
