using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrpSelectionHelper.Interfaces;
using System.Net.Http;
using UrpSelectionHelper.Helpers;
using InterestingCourseSelectionHelper.Interfaces;

namespace UrpSelectionHelper.Clients
{
    class FormAction
    {
        public static async Task<HttpContent> SignIn(string username, string password, string validateCode,InternetHelper ih)
        {
            var formData = new Dictionary<string, string> {
                {"zjh1","" },
                {"tips","" },
                {"lx","" },
                {"evalue","" },
                {"eflag","" },
                {"fs","" },
                {"dzslh","" },
                {"zjh",username },
                {"mm",password },
                {"v_yzm",validateCode }
            };
            return await ih.PostAsyncWithValidation(FreeAddress.GetLoginActionUri(), formData);
        }

        public static async Task<HttpContent> CheckFreeClass(string Id,string serial,InternetHelper ih)
        {
            var formData = new Dictionary<string, string> {
                {"cxType","kch" },
                {"pageNumber","-1" },
                {"kch",Id },
                {"kxh",serial },
                {"rxklb","" },
                {"xsyyl","" },
                {"krlPxfs","desc" },
                {"kchPxfs","asc" }
            };
            return await ih.PostAsync(FreeAddress.GetFreeSelectableCourseUri(), formData);
        }


        public static async Task<HttpContent> SelectClass(string validateCode, string courseId,InternetHelper ih)
        {
            var formData = new Dictionary<string, string>
            {
                //ifraType=wct&bclx=rxk&pageNumber=1&krlPxfs=asc&kchPxfs=desc&v_yzm={0}&kcId={1}&pageNo=
                {"ifraType","wct" },
                {"bclx","rxk" }, // Free-Select Time **not specified course**
                {"pageNumber","1" }, //will it validate?
                {"krlPxfs","asc" },
                {"kchPxfs","desc" },
                {"v_yzm",validateCode },
                {"kcId",courseId },
                {"pageNo","" }
            };
            return await ih.PostAsync(FreeAddress.GetSelectCourseUri(), formData);
        }
        public static async Task<HttpContent> SelectLimitedClass(string validateCode, string courseId, InternetHelper ih)
        {
            var formData = new Dictionary<string, string>
            {
                {"ifraType","bxq" },
                {"v_yzm",validateCode },
                {"kcId",courseId }
            };
            return await ih.PostAsync(LimitedAddress.GetSelectCourseUri(), formData);
        }
        public static async Task<HttpContent> SelectCrossClass(string validateCode, string courseId, InternetHelper ih)
        {
            var formData = new Dictionary<string, string>
            {
                {"ifraType","knj" },
                {"v_yzm",validateCode },
                {"kcId",courseId },
                {"jhxn","" },
                {"jhxq","" }
            };
            return await ih.PostAsync(FreeAddress.GetSelectCourseUri(), formData);
        }

        public static async Task<HttpContent> GetSelectableCourse(InternetHelper ih,bool isCross = false)
        {
            if (isCross) return await ih.GetAsync(FreeAddress.GetSelectableCrossCourseUri());
            return await ih.GetAsync(FreeAddress.GetSelectableCourseUri());
        }

        public static async Task<HttpContent> GetLimitedSelectableCourse(InternetHelper ih)
        {
            return await ih.GetAsync(LimitedAddress.GetSelectableCourseUri());
        }

        public static async Task<HttpContent> GetFreeClass(InternetHelper ih)
        {
            return await ih.GetAsync(FreeAddress.GetSelectedCourseUri());
        }

        public static async Task<HttpContent> GetLimitedClass(InternetHelper ih)
        {
            return await ih.GetAsync(LimitedAddress.GetSelectedCourseUri());
        }

    }
}
