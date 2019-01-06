using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OriginDLBooster
{
    public partial class Form1 : Form
    {
        bool isPatched = false;
        string IniFile = "";
        
        void InitialCheck()
        {
            string ClientPath = "";
            ClientPath = (string)Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Origin", "ClientPath", "");
            if (ClientPath.Equals(""))
                ClientPath = (string)Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Origin", "ClientPath", "");
            if (ClientPath.Equals(""))
            {
                System.Windows.Forms.MessageBox.Show("Origin not detected. Please reinstall origin");
                Close();
            }
            ClientPath = ClientPath.Substring(0, ClientPath.LastIndexOf('\\'));
            IniFile = ClientPath + "\\EACore.ini";

            Process[] processList = Process.GetProcessesByName("origin");
            if(processList.Length > 0)
            {
                processList = Process.GetProcessesByName("origin");
                System.Windows.Forms.MessageBox.Show("Please turn off origin at taskbar.");
                Close();
            }
        }

        bool PatchCheck()
        {
            bool isEnv = false, isCdn = false;
            string[] lines = System.IO.File.ReadAllLines(IniFile);

            foreach (string line in lines)
            {
                if (line.Contains("EnvironmentName=production"))
                    isEnv = true;

                if (line.Contains("CdnOverride=akamai"))
                    isCdn = true;
            }

            isPatched = isCdn && isEnv;
            if (isPatched)
                this.button1.Text = "Deactivate!";
            else
                this.button1.Text = "Activate!";
            return isPatched;
        }

        public Form1()
        {
            InitialCheck();
            InitializeComponent();
            PatchCheck();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isPatched)
            {
                System.IO.File.WriteAllText(IniFile, "");

                if (!PatchCheck()) {
                    System.Windows.Forms.MessageBox.Show("Booster deactivated!", "Created by @YukihoAA");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Cannot control booster.");
                    Close();
                }
            }
            else
            {
                System.IO.File.WriteAllText(IniFile, "[connection]\r\n" +
                "    EnvironmentName=production\r\n\r\n" +
                "[Feature]\r\n" +
                "    CdnOverride=akamai");

                if (PatchCheck())
                {
                    System.Windows.Forms.MessageBox.Show("Booster activated!", "Created by @YukihoAA");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Cannot control booster.");
                    Close();
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer", "https://github.com/YukihoAA");
        }
    }
}
