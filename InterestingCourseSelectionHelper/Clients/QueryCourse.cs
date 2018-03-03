using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrpSelectionHelper.Helpers;
using HtmlAgilityPack;

namespace UrpSelectionHelper.Clients
{
    class QueryCourse
    {
        public static async Task<bool> QueryIfSelectable(string id,string serial,InternetHelper ih)
        {
            var result = await FormAction.CheckFreeClass(id,serial, ih);
            var text = await result.ReadAsStringAsync();
            if (text.Contains("无余量"))
            {
                return false;
            }
            return true;
            //var htmlDoc = new HtmlDocument();
            //var list = new List<string>();
            //htmlDoc.LoadHtml(text);
            //foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//tr//td"))
            //{
            //    var item = node.InnerHtml.Trim().Replace("&nbsp;", "");
            //    if ((item.Contains("<!-- 任选课补退选 -->") && !item.Contains("checkbox")) || item.Contains("无余量"))
            //    {
            //        list.Add("no "
            //            + node.InnerText
            //            .Trim()
            //            .Split('\n')
            //            .Aggregate(
            //                (str1, str2) => str1.Trim() + " " + str2.Trim()
            //            ));
            //    }
            //    if (item.StartsWith("<table") && !item.Contains("选择"))
            //    {
            //        node.Remove();
            //        continue;
            //    }
            //    if (item.Contains("kcxx"))
            //    {
            //        list.Add(node.InnerText.Trim());
            //    }
            //    if (item.Contains("jsxx"))
            //    {
            //        list.Add(node.InnerText.Trim());
            //    }
            //    if (item.Contains("checkbox"))
            //    {
            //        list.Add("yes");
            //    }
            //    if (item.Contains("<") || String.IsNullOrEmpty(item))
            //        continue;
            //    list.Add(item);
            //}

            //var resultList = new List<Dto.SelectedCourse>();

            //for (int i = 0; i < list.Count; i += 12)
            //{
            //    var course = new Dto.SelectedCourse
            //    {
            //        Selectable = list[i + 0],
            //        Id = list[i + 3],
            //        Serial = list[i + 5],
            //        Name = list[i + 4],
            //        Teacher = list[i + 8]
            //    };
            //    resultList.Add(course);
            //}
            //var isSelectable = resultList.Find((item) => item.Id == id && item.Serial == serial && item.Selectable == "yes");
            //if (isSelectable == null)
            //{
            //    return false;
            //}
            //else return true;
        }

        public static async Task<List<string>> GetCourseListAsync(InternetHelper ih)
        {
            var result = await FormAction.GetSelectableCourse(ih);
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

        public static async Task<List<Dto.SelectedCourse>> GetSelectableCourseAsync(InternetHelper ih ,bool isCross = false)
        {
            var result = await FormAction.GetSelectableCourse(ih,isCross);
            var text = await result.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            if (text.Contains("错误信息"))
            {
                return null;
            }
            var list = new List<string>();

            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//tr"))
            {
                var item = node.InnerHtml;
                if (item.Contains("选中"))
                {
                    var partialDoc = new HtmlDocument();
                    partialDoc.LoadHtml(item);
                    foreach (var box in partialDoc.DocumentNode.SelectNodes("//td//table//tr"))
                    {
                        var innerTable = box.SelectNodes(".//td//tr");
                        if (innerTable == null) continue;
                        foreach (var useless in innerTable)
                        {
                            useless.Remove();
                        }
                        list.Add(box.InnerHtml.Trim());
                        break;
                    }
                    break;
                }

            }
            list.Skip(1);
            var trimmedDoc = new HtmlDocument();
            trimmedDoc.LoadHtml(list[0]);

            var bigList = new List<string>();
            foreach (HtmlNode node in trimmedDoc.DocumentNode.SelectNodes("//td"))
            {
                var item = node.InnerHtml.Trim().Replace("&nbsp;", "");
                if (item.Contains("img"))
                {
                    bigList.Add(node.InnerText.Trim());
                }
                if (item.Contains("选中") || item.Contains("无余量"))
                {
                    bigList.Add("no " 
                        + node.InnerText
                        .Trim()
                        .Split('\n')
                        .Aggregate(
                            (str1, str2) => str1.Trim() + " " + str2.Trim()
                        ));
                }
                if (item.Contains("checkbox"))
                {
                    bigList.Add("yes");
                }
                if (item.Contains("<") || String.IsNullOrEmpty(item))
                    continue;
                bigList.Add(item);
            }

            var resultList = new List<Dto.SelectedCourse>();

            if (isCross)
            {
                for (int i = 0; i < bigList.Count; i += 12)
                {
                    var course = new Dto.SelectedCourse
                    {
                        Selectable = bigList[i + 0],
                        Id = bigList[i + 3],
                        Serial = bigList[i + 5],
                        Name = bigList[i + 4],
                        Teacher = bigList[i + 8]
                    };
                    resultList.Add(course);
                }
                 return resultList;
            }

            for (int i = 0; i < bigList.Count; i += 10)
            {
                var course = new Dto.SelectedCourse
                {
                    Selectable = bigList[i + 0],
                    Id = bigList[i + 1],
                    Serial = bigList[i + 3],
                    Name = bigList[i + 2],
                    Teacher = bigList[i + 6]
                };
                resultList.Add(course);
            }

            return resultList;
        }
    }
}
