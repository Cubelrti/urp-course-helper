using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InterestingCourseSelectionHelper.Interfaces;
using System.IO;
using System.Net;

namespace InterestingCourseSelectionHelper.Helpers
{
    public class InternetHelper
    {
        private HttpClient http;
        public InternetHelper()
        {
            Cookies = new CookieContainer();
            var handler = new HttpClientHandler() { UseCookies = true, CookieContainer = Cookies };
            http = new HttpClient(handler)
            {
                MaxResponseContentBufferSize = 10240000
            };
            http.DefaultRequestHeaders.ExpectContinue = false;
            foreach (var item in Header.getUserAgent())
                http.DefaultRequestHeaders.Add(item.Key, item.Value);
        }
        public CookieContainer Cookies { get; private set; }

        public async Task<HttpContent> PostAsyncWithValidation(Uri link, Dictionary<string, string> form)
        {
            var content = new FormUrlEncodedContent(form);
            var responseFromValidation = await http.GetAsync(Address.GetLoginActionUri());
            var response = await http.PostAsync(link, content);
            return response.Content;
        }
        public async Task<Stream> GetAsyncStream(Uri link)
        {
            var response = await http.GetAsync(link);
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<HttpContent> GetAsync(Uri link)
        {
            var response = await http.GetAsync(link);
            return response.Content;
        }
    }
}


