using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterestingCourseSelectionHelper.Helpers;
using HtmlAgilityPack;

namespace InterestingCourseSelectionHelper.Clients
{
    class QueryCourse
    {
        public static string Query(string CourseNumber)
        {
            var queryString = "";
            return queryString;
        }

        public static async Task<List<string>> GetCourseListAsync(InternetHelper ih)
        {
            var result = await UserAction.GetClass(ih);
            var text = await result.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            if (text.Contains("错误信息"))
            {
                throw new InvalidOperationException("Server closed!");
            }
            var list = new List<string>();

            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//tr//td"))
            {
                var item = node.InnerHtml.Trim().Replace("&nbsp;", "");
                if (item.Contains("<") || String.IsNullOrEmpty(item))
                    continue;
                list.Add(item);
            }
            return list;
        }
    }
}
