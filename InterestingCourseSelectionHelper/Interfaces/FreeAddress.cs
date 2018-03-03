using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrpSelectionHelper.Interfaces
{
    public static class FreeAddress
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
            //return new Uri("http://121.194.57.131/zyxkAction.do?oper=yxkc&type=yx");
            //freeSelect
            return new Uri("http://121.194.57.131/rxkYxkcAction.do?actiontype=1&type=btx");
        }
        public static Uri GetSelectableCrossCourseUri()
        {
            return new Uri("http://121.194.57.131/bxXxBtxAction.do?actionType=5");
        }

        public static Uri GetSelectableCourseUri()
        {
            return new Uri("http://121.194.57.131/bxXxBtxAction.do?actionType=2");
        }
        public static Uri GetFreeSelectableCourseUri()
        {
            return new Uri("http://121.194.57.131/rxkAction.do?actionType=2");
        }

        public static Uri GetValidateCheckUri(string validateCode)
        {
            //mustSelect
            //return new Uri($"http://121.194.57.131/bxXxBtxAction.do?actionType=10&v_yzm={validateCode}");
            //freeSelect
            return new Uri($"http://121.194.57.131/rxkAction.do?actionType=13&v_yzm={validateCode}");
        }
        public static Uri GetSelectCourseUri()
        {
            //mustSelect
            //return new Uri("http://121.194.57.131/bxXxBtxAction.do?actionType=3");
            //freeSelect
            return new Uri("http://121.194.57.131/rxkBtxAction.do?actionType=3");
        }
        public static Uri GetStartValidateUri()
        {
            //mustSelect
            //return new Uri("http://121.194.57.131/bxXxBtxAction.do?actionType=9");
            //freeSelect
            return new Uri("http://121.194.57.131/rxkAction.do?actionType=12");
        }
    }
}
