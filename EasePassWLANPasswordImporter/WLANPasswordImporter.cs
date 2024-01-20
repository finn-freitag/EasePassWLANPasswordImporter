using EasePassExtensibility;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePassWLANPasswordImporter
{
    public class WLANPasswordImporter : IPasswordImporter // Source: https://github.com/finn-freitag/WLANKeyReader/
    {
        public string SourceName => "WLAN interface";

        public Uri SourceIcon => Icon.GetIconUri();

        Dictionary<string, string> wlanAccess = new Dictionary<string, string>();
        Dictionary<string, string> wlanAccessNew = new Dictionary<string, string>();

        public PasswordItem[] ImportPasswords()
        {
            wlanAccess.Clear();
            wlanAccessNew.Clear();
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = false;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c netsh wlan show profile";
            p.OutputDataReceived += P_OutputDataReceived;
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();
            p.CancelOutputRead();
            p.Close();
            p.Dispose();

            foreach (KeyValuePair<string, string> pair in wlanAccess)
            {
                wlanAccessNew.Add(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<string, string> pair in wlanAccess)
            {
                TaggedProcess p2 = new TaggedProcess();
                p2.StartInfo.UseShellExecute = false;
                p2.StartInfo.RedirectStandardOutput = true;
                p2.StartInfo.RedirectStandardError = false;
                p2.StartInfo.RedirectStandardInput = false;
                p2.StartInfo.CreateNoWindow = true;
                p2.StartInfo.FileName = "cmd.exe";
                p2.StartInfo.Arguments = "/c netsh wlan show profile name=\"" + pair.Key + "\" key=clear";
                p2.Tag = pair.Key;
                p2.OutputDataReceived += P_OutputDataReceived1;
                p2.Start();
                p2.BeginOutputReadLine();
                p2.WaitForExit();
                p2.CancelOutputRead();
                p2.Close();
                p2.Dispose();
            }

            List<PasswordItem> items = new List<PasswordItem>();
            foreach (KeyValuePair<string, string> pair in wlanAccessNew)
            {
                if (!string.IsNullOrEmpty(pair.Value))
                {
                    PasswordItem item = new PasswordItem();
                    item.DisplayName = pair.Key;
                    item.Password = pair.Value;
                    items.Add(item);
                }
            }

            return items.ToArray();
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                try
                {
                    wlanAccess.Add(e.Data.Split(new string[] { " : " }, StringSplitOptions.None)[1], "");
                }
                catch { }
                /*if (e.Data.Contains("Profil f\u0081r alle Benutzer : ") || e.Data.Contains("User profiles"))
                {
                    
                }*/
            }
        }

        private void P_OutputDataReceived1(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.Contains("Schl\u0081sselinhalt") || e.Data.Contains("Key Content"))
                {
                    string password = e.Data.Split(':')[1].Substring(1);
                    Dictionary<string, string> newAccess = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> pair in wlanAccess)
                    {
                        if (((TaggedProcess)sender).Tag == pair.Key)
                        {
                            newAccess.Add(pair.Key, password);
                        }
                    }
                    foreach (KeyValuePair<string, string> pair in newAccess)
                    {
                        wlanAccessNew[pair.Key] = pair.Value;
                    }
                }
            }
        }

        public bool PasswordsAvailable()
        {
            return true;
        }
    }
}
