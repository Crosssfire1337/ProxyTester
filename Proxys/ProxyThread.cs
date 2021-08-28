using ProxyTester.Proxys;
using System;
using System.Diagnostics;
using System.Net;

namespace ProxyTester.Proxy
{
    public class ProxyThread
    {
        private readonly Stopwatch _stopWatch = new();
        public ProxyItem ProxyItem { get; private set; }
        public UseProxy Proxy { get; }
        public Uri Destination { get; }

        public ProxyThread(UseProxy proxy, string destinationUrl)
        {
            Proxy = proxy;
            Destination = new Uri(destinationUrl);
        }

        public void Start(object _state)
        {
            _stopWatch.Start();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Destination);

            WebProxy webProxy = new(Destination, false, new string[0], new NetworkCredential(ProxyItem.User, ProxyItem.Pass));
            httpWebRequest.Proxy = webProxy;
            httpWebRequest.Timeout = 3000;

            try
            {
                WebResponse webResponse = httpWebRequest.GetResponse();
                ProxyItem.Status = "success";
            }
            catch (WebException)
            {
                ProxyItem.Status = "failed";
            }

            _stopWatch.Stop();

            if (ProxyItem.Status == "success")
                ProxyItem.Speed = _stopWatch.ElapsedMilliseconds.ToString() + "ms";

            _stopWatch.Reset();

            Proxy.UpdateRowThreadSafe();
        }

    }
}
