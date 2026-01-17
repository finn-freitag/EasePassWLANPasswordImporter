using EasePassExtensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePassWLANPasswordImporter
{
    public class AboutPage : IAboutPlugin
    {
        public string PluginName => "WLAN Password Importer";

        public string PluginDescription => "Reads your registered WLAN passwords and saves them in Ease Pass. This plugin does only work on English and German computers (becaus of 'netsh' command).";

        public string PluginAuthor => "Finn Freitag";

        public string PluginAuthorURL => "https://finnfreitag.com?ref=ep_plgn_wlanimp";

        public Uri PluginIcon => Icon.GetIconUri();
    }
}
