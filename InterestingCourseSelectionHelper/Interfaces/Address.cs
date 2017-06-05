using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterestingCourseSelectionHelper.Interfaces
{
    public static class Address
    {
        public static Uri GetIPUri()
        {
            return new Uri("http://121.194.57.131/");
        }
        public static Uri GetValidateImageUri()
        {
            return new Uri("http://121.194.57.131/validateCodeAction.do?random=0.1");
        }
        public static Uri GetLoginActionUri()
        {
            return new Uri("http://121.194.57.131/loginAction.do");
        }
        public static Uri GetSelectedCourseUri()
        {
            return new Uri("http://121.194.57.131/zyxkAction.do?oper=yxkc&type=yx");
        }
        public static Uri GetSelectableCourseUri()
        {
            return new Uri("http://121.194.57.131/zytzAction.do?oper=bxqkc");
        }
        public static Uri GetValidateCheckUri(string validateCode)
        {
            return new Uri($"http://121.194.57.131/rxkAction.do?actionType=13&v_yzm={validateCode}");
        }
        public static Uri GetFreeSelectCourseUri()
        {
            return new Uri("http://121.194.57.131/rxkBtxAction.do?actionType=3");
        }
    }
}
