namespace ProxyTester.Proxy
{
    public class ProxyItem
    {
        public ProxyItem(string ip, int port, string user, string pass, string status, string speed)
        {
            IP = ip;
            Port = port;
            User = user;
            Pass = pass;
            Status = status;
            Speed = speed;
        }

        public string IP { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string Status { get; set; }
        public string Speed { get; set; }

        public static ProxyItem CreateProxyItem(string Ip, string Port, string User, string Pass, string Status, string Speed)
        {
            return new ProxyItem(Ip, int.Parse(Port), User, Pass, Status, Speed);
        }
    }
}