using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vanish.Studios
{
    public partial class SpaceMouse : Form
    {
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        Server MainServer;
        double initialX;
        double initialY;
        double initialZ;
        double x = 1000;
        double y = 1000;
        double z = 1000;
        bool left;
        bool right;
        int mouseLeft = 0;
        int mouseRight = 0;
        bool start = false;
        public SpaceMouse()
        {
            InitializeComponent();
            MainServer = new Server();
            MainServer.OnRequestChanged += ParseRequest;
            Spark.UI.Console.Show();
            Spark.UI.Console.WriteLn("Server Initialized");
        }
        protected override void WndProc(ref Message m)
        {
            if (!MainWindow.Update(ref m))
            {
                base.WndProc(ref m);
            }
        }
        private void TitleBarButtonClick(int ButtonIndex)
        {
            if (ButtonIndex == 0)
            {
                ToggleServer();
            }
        }
        public void ToggleServer()
        {
            if (MainWindow.Buttons[0].Text == "Start")
            {
                MainWindow.Buttons[0].Text = "Stop";
                MainWindow.Buttons[0].BackColor = Color.Yellow;
                Start();
            }
            else if (MainWindow.Buttons[0].Text == "Stop")
            {
                MainWindow.Buttons[0].Text = "Start";
                MainWindow.Buttons[0].BackColor = Color.Green;
                Stop();
            }
        }
        public void Start()
        {
            Spark.UI.Console.WriteLn("Server Starting");
            MainServer.Start();
        }
        public void Stop()
        {
            Spark.UI.Console.WriteLn("Server Stopping");
            MainServer.Stop();
        }
        public void ParseRequest(string Request)
        {
            try
            {
                start = true;
                string Parsed = Request.Substring(Request.LastIndexOf("/") + 1);
                string xCoordinate = Parsed.Substring(0, Parsed.IndexOf("?"));
                Parsed = Parsed.Substring(Parsed.IndexOf("?") + 1);
                string yCoordinate = Parsed.Substring(0, Parsed.IndexOf("?"));
                Parsed = Parsed.Substring(Parsed.IndexOf("?") + 1);
                string zCoordinate = Parsed.Substring(0, Parsed.IndexOf("?"));
                Parsed = Parsed.Substring(Parsed.IndexOf("?") + 1);
                string leftClick = Parsed.Substring(0, Parsed.IndexOf("?"));
                Parsed = Parsed.Substring(Parsed.IndexOf("?") + 1);
                string rightClick = Parsed.Substring(0, Parsed.IndexOf("?"));
                Parsed = Parsed.Substring(Parsed.IndexOf("?") + 1);
                string scrollClick = Parsed;
                xCoordinate = xCoordinate.Substring(xCoordinate.IndexOf(":") + 1);
                yCoordinate = yCoordinate.Substring(yCoordinate.IndexOf(":") + 1);
                zCoordinate = zCoordinate.Substring(zCoordinate.IndexOf(":") + 1);
                leftClick = leftClick.Substring(leftClick.IndexOf(":") + 1);
                rightClick = rightClick.Substring(rightClick.IndexOf(":") + 1);
                if (x == 1000)
                {
                    initialX = Convert.ToDouble(xCoordinate);
                }
                if (y == 1000)
                {
                    initialY = Convert.ToDouble(yCoordinate);
                }
                if (z == 1000)
                {
                    initialZ = Convert.ToDouble(zCoordinate);
                }
                x = Convert.ToDouble(xCoordinate) - initialX;
                y = Convert.ToDouble(yCoordinate) - initialY;
                z = Convert.ToDouble(zCoordinate) - initialZ;
                x /= 5;
                y /= 5;
                z /= 5;
                left = Convert.ToBoolean(leftClick);
                right = Convert.ToBoolean(rightClick);
                if (mouseLeft == 0)
                {
                    if (left == true)
                    {
                        mouseLeft = 1; //MOUSEDOWN
                    }
                }
                else if (mouseLeft == 1)
                {
                    if (left == false)
                    {
                        mouseLeft = 2; //MOUSEUP
                    }
                }
                if (mouseRight == 0)
                {
                    if (right == true)
                    {
                        mouseRight = 1; //MOUSEDOWN
                    }
                }
                else if (mouseRight == 1)
                {
                    if (right == false)
                    {
                        mouseRight = 2; //MOUSEUP
                    }
                }
            }
            catch (Exception e)
            {
                Spark.UI.Console.WriteLn("ERROR");
            }
        }
        private void UITimerTick(object sender, EventArgs e)
        {
            l_ip_info.Text = MainServer.GetIP();
            l_ip_info.Refresh();
            if (MainServer.On == true)
            {
                l_status_info.Text = "Running";
            }
            else
            {
                l_status_info.Text = "Stopped";
            }
            l_status_info.Refresh();
            l_request_info.Text = MainServer.LastRequest;
            l_request_info.Refresh();
            l_x_info.Text = "" + x;
            l_x_info.Refresh();
            l_y_info.Text = "" + y;
            l_y_info.Refresh();
            l_z_info.Text = "" + z;
            l_z_info.Refresh();
            l_left_info.Text = "" + mouseLeft;
            l_left_info.Refresh();
            l_right_info.Text = "" + mouseRight;
            l_right_info.Refresh();
            if (MainServer.Error)
            {
                UITimer.Stop();
                MessageBox.Show("Access is denied");
                this.Close();
            }
            if (MainServer.On)
            {
                Point pt = GetCursorPosition();
                int lox = (int)(pt.X + (x * -10));
                int loy = (int)(pt.Y + (y * +10));
                uint flagsLeft = 0;
                uint flagsRight = 0;
                if (mouseLeft == 1)
                {
                    flagsLeft = MOUSEEVENTF_LEFTDOWN;
                }
                else if (mouseLeft == 2)
                {
                    flagsLeft = MOUSEEVENTF_LEFTUP;
                    mouseLeft = 0;
                }
                else if (mouseLeft == 0)
                {
                    flagsLeft = 0;
                }
                if (mouseRight == 1)
                {
                    flagsRight = MOUSEEVENTF_RIGHTDOWN;
                }
                else if (mouseRight == 2)
                {
                    flagsRight = MOUSEEVENTF_RIGHTUP;
                    mouseRight = 0;
                }
                else if (mouseRight == 0)
                {
                    flagsRight = 0;
                }
                SetCursorPos(lox, loy);
                mouse_event(flagsLeft | flagsRight, (uint)lox, (uint)loy, 0, 0);
            }
        }
    }
}
