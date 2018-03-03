using UrpSelectionHelper.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UrpSelectionHelper.Clients
{
    class SelectCourse
    {
        public static async Task<string> TrySelect(string course, string validateCode, InternetHelper ih, bool isCross = false)
        {
            HttpContent result;
            if (isCross)
            {
                result = await FormAction.SelectCrossClass(validateCode, course, ih);
            }
            else result = await FormAction.SelectClass(validateCode, course, ih);
            var text = await result.ReadAsStringAsync();
            return text;
        }
        public static async Task<string> TrySelectLimited(string course, string validateCode, InternetHelper ih)
        {
            var result = await FormAction.SelectLimitedClass(validateCode, course, ih);
            var text = await result.ReadAsStringAsync();
            return text;
        }

    }
}
