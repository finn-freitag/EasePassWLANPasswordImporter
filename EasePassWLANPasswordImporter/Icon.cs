using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePassWLANPasswordImporter
{
    internal static class Icon
    {
        public static Uri GetIconUri()
        {
            string path = Path.GetTempFileName();
            File.WriteAllBytes(path, Properties.Resources.icon);
            return new Uri(path);
        }
    }
}
