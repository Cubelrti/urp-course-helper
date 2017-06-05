using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterestingCourseSelectionHelper.Interfaces;
using System.Net.Http;
using InterestingCourseSelectionHelper.Helpers;

namespace InterestingCourseSelectionHelper.Clients
{
    class UserAction
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
            return await ih.PostAsyncWithValidation(Address.GetLoginActionUri(), formData);
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
            return await ih.PostAsyncWithValidation(Address.GetFreeSelectCourseUri(), formData);
        }

        public static async Task<HttpContent> GetSelectableCourse(InternetHelper ih)
        {
            return await ih.GetAsync(Address.GetSelectableCourseUri());
        }


        public static async Task<HttpContent> GetClass(InternetHelper ih)
        {
            return await ih.GetAsync(Address.GetSelectedCourseUri());
        }

    }
}
