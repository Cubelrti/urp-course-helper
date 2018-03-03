using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterestingCourseSelectionHelper.Interfaces
{
    public static class LimitedAddress
    {
        public static Uri GetIPUri()
        {
            return new Uri("http://121.194.57.131/");
        }
        public static Uri GetValidateImageUri(string random)
        {
            return new Uri($"http://121.194.57.131/validateCodeAction.do?random={random}");
        }
        public static Uri GetLoginActionUri()
        {
            return new Uri("http://121.194.57.131/loginAction.do");
        }
        public static Uri GetSelectedCourseUri()
        {
            //mustSelect
            return new Uri("http://121.194.57.131/zyxkAction.do?oper=yxkc&type=btx");
        }
        public static Uri GetStartValidateUri()
        {
            //mustSelect
            return new Uri("http://121.194.57.131/bxXxBtxAction.do?actionType=9");
        }
        public static Uri GetValidateCheckUri(string validateCode)
        {
            //mustSelect
            return new Uri($"http://121.194.57.131/bxXxBtxAction.do?actionType=10&v_yzm={validateCode}");
        }
        public static Uri GetSelectCourseUri()
        {
            //mustSelect
            return new Uri("http://121.194.57.131/bxXxBtxAction.do?actionType=3");
        }

        internal static Uri GetSelectableCourseUri()
        {
            throw new NotImplementedException();
        }
    }
}
