using ProxyTester.Proxy;
using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ProxyTester.Proxys
{
    public class UseProxy
    {
        private string _proxyString;
        public bool Ready = false;
        public bool Success = false;
        public ProxyItem ProxyItem;
        private ListView _listView;
        private int _id = -2;
        private Dispatcher _dispatcher;

        public UseProxy(string proxyString, ListView listView, Dispatcher dispatcher)
        {
            _proxyString = proxyString;
            _listView = listView;
            _dispatcher = dispatcher;

            GenerateRow();
        }

        private bool Validate()
        {
            if (_proxyString is null || _proxyString.Split(":").Length < 3) return false;

            return true;
        }

        private void GenerateRow()
        {
            if (!Validate()) throw new InvalidProxyException("Provided string is invalid");

            string[] splittedProxy = _proxyString.Split(":");

            _id = _listView.Items.Add(ProxyItem.CreateProxyItem(splittedProxy[0], splittedProxy[1], splittedProxy[2], splittedProxy[3], "pending", ""));
        }

        public void UpdateRow()
        {
            _listView.Items[_id] = ProxyItem;
        }

        public void UpdateRowThreadSafe()
        {
            if (_id < 0 || ProxyItem is null) return;

            _dispatcher.BeginInvoke(new Action(delegate ()
            {
                UpdateRow();
            }));
        }

        public void Run(string destination)
        {
            ProxyThread proxyThread = new(this, destination);

            ThreadPool.SetMaxThreads(10, 10);

            ProxyItem.Status = "testing";
            UpdateRow();

            ThreadPool.QueueUserWorkItem(proxyThread.Start);
        }
    }
}
