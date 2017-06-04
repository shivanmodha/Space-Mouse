using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using System.Text;

namespace Vanish.Studios
{
    class Server
    {
        public Thread Thread;
        public HttpListener Listener;
        public bool On = false;
        public bool Error = false;
        public string LastRequest = "";
        public delegate void OnRequestChangedEvent(string Request);
        public event OnRequestChangedEvent OnRequestChanged;
        public Server()
        {
            Initialize();
        }
        public void Initialize()
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add("http://localhost:5000/");
            Listener.Prefixes.Add("http://" + GetIP() + "/");
            Thread = new Thread(thread);
        }
        public void Start()
        {
            Thread.Start();
        }
        public void Stop()
        {
            Listener.Stop();
            On = false;
            Thread = new Thread(thread);
        }
        public string GetIP()
        {
            IPHostEntry ip = Dns.GetHostEntry(Dns.GetHostName());
            if (ip.AddressList.Length >= 3)
            {
                return ip.AddressList[2].ToString();
            }
            else
            {
                return ip.AddressList[1].ToString();
            }
        }
        private string GetName()
        {
            IPHostEntry ip = Dns.GetHostEntry(Dns.GetHostName());
            return ip.HostName;
        }
        public void thread()
        {
            try
            {
                Listener.Start();
                On = true;
                while (On)
                {
                    try
                    {
                        HttpListenerContext _context = Listener.GetContext();
                        string request = _context.Request.Url.AbsoluteUri;
                        if (!request.EndsWith(".ico"))
                        {
                            LastRequest = _context.Request.Url.AbsoluteUri;
                            OnRequestChanged(LastRequest);
                        }
                        byte[] _response = Encoding.UTF8.GetBytes("Vanish Studios");
                        _context.Response.OutputStream.Write(_response, 0, _response.Length);
                        _context.Response.KeepAlive = false;
                        _context.Response.Close();
                    }
                    catch (Exception e)
                    {
                        string result = e.Message;
                    }
                }
            }
            catch (Exception e)
            {
                Error = true;
                string result = e.Message;
                Spark.UI.Console.WriteLn(result);
            }
        }
    }
}
