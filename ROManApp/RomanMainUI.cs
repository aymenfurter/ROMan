using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;


namespace ROManApplication
{
    public partial class RomanMainUI : Form
    {
        Process[] processListLastRefreshed = null;
        ArrayList processItems = new ArrayList();

        RomanMainUI form = null;
        public RomanMainUI()
        {
            InitializeComponent();
            form = this;
        }


        void imageboxClickFocus(object sender, EventArgs e)
        {
            PictureBox box = (PictureBox)sender;

            int activationIndex = 0;
            foreach (ProcessItem theprocess in processItems)
            {

                if (theprocess.tag.Equals(box.Text))
                {
                    IntPtr p = theprocess.process.MainWindowHandle;
                    SetForegroundWindow(p);
                }
                activationIndex = activationIndex + 1;
            }
        }

        private void refresh(object sender, EventArgs e)
        {
            processListLastRefreshed = Process.GetProcesses();
            Process mainProcess = Process.GetCurrentProcess();
            IntPtr mainP = mainProcess.MainWindowHandle;
            int processID = 0;
            foreach (Process theprocess in processListLastRefreshed)
            {
                if (theprocess.ProcessName.Contains("Ragexe") || theprocess.ProcessName.Contains("iro"))
                {
                    processID = processID + 1;
                    ProcessItem item = new ProcessItem();
                    item.process = theprocess;
                    item.Name = theprocess.ProcessName + " " + processID;
                    item.DisplayMember = theprocess.ProcessName + " " + processID;
                    item.tag = processID.ToString();

                    IntPtr p = theprocess.MainWindowHandle;
                    SetForegroundWindow(p);
                    System.Threading.Thread.Sleep(100);
                    var proc = theprocess;
                    var rect = new User32.Rect();
                    User32.GetWindowRect(proc.MainWindowHandle, ref rect);

                    int width = 200;
                    int height = 50;

                    var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                    Graphics graphics = Graphics.FromImage(bmp);
                    graphics.CopyFromScreen(rect.left + 5, rect.top + 28, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
                    {
                        PictureBox P = new PictureBox();
                    
                        P.Image = bmp;
                        P.Size = new Size(250, 50);
                        P.Text = processID.ToString();

                        P.Location = new System.Drawing.Point(300, ((processID-1) * 60) + 30);
                        form.Controls.Add(P);
                        P.Parent = form;
                        P.MouseClick += new MouseEventHandler(imageboxClickFocus);
                    }

                    processItems.Add(item);
                }
                
            }
            SetForegroundWindow(mainP);
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
  
        private void killAll(object sender, EventArgs e)
        {
            foreach (ProcessItem theprocess in processItems)
            {
                try
                {
                    theprocess.process.Kill();
                }
                catch
                {

                }
            }
        }
    }
    class ProcessItem
    {
        public Process process;
        public String Name { get; set; }
        public String DisplayMember { get; set; }
        public String tag { get; set; }
    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, Keys wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
    }

    public class User32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
    }

}