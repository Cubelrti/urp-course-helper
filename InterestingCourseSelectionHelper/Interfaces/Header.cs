using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterestingCourseSelectionHelper.Interfaces
{
    public static class Header
    {
        public static Dictionary<string, string> getUserAgent()
        {
            return new Dictionary<string, string>
            {
                {"user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko"},
                {"Accept-Language", "zh-Hans-CN,zh-Hans;q=0.93,pt-PT;q=0.87,de-DE;q=0.80,de;q=0.73,en-US;q=0.67,en;q=0.60,it-IT;q=0.53,it;q=0.47,nl-NL;q=0.40,nl;q=0.33,pl;q=0.27,pt-BR;q=0.20,pt;q=0.13,id;q=0.067"},
                {"Accept", "text/html, application/xhtml+xml, image/jxr, */*" },
                {"Connection", "Keep-Alive" },
                {"Accept-Encoding", "gzip, deflate" },
                {"Pragma", "no-cache" }
            };
        }
    }
}
