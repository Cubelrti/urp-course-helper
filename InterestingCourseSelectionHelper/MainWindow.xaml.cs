using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InterestingCourseSelectionHelper.Clients;
using InterestingCourseSelectionHelper.Helpers;
using System.Drawing;
using InterestingCourseSelectionHelper.Interfaces;
using System.Net;
using HtmlAgilityPack;
using InterestingCourseSelectionHelper.Dto;
using System.Threading;

namespace InterestingCourseSelectionHelper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        InternetHelper ih;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ih = new InternetHelper();
            Bitmap bitmap = await ValidateCode.GetValidateCodeImage(ih);
            ValidateCodeImage.Source = ImageProcessor.ToBitmapImage(bitmap);
            string validateResult = await ValidateCode.GetValidateCode(bitmap);
            ValidateResult.Text = "ValidateResult: " + validateResult;
            var result = await UserAction.SignIn(Username.Text, Password.Text, validateResult, ih);
            HttpResultContent.Text = await result.ReadAsStringAsync();
            if (HttpResultContent.Text.Contains("学分制综合教务"))
            {
                //Login.IsEnabled = false;
                var cookies = ih.Cookies;
                IEnumerable<Cookie> responseCookies = cookies.GetCookies(Address.GetIPUri()).Cast<Cookie>();
                foreach (var item in responseCookies)
                {
                    Cookies.Text = item.Value;
                }
                Controls.IsEnabled = true;
            }
            else if (AutoRetry.IsChecked.Value)
            {
                Button_Click(sender, e);
            }

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CourseList.Items.Clear();
            var result = await UserAction.GetClass(ih);
            var text = await result.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            if (text.Contains("错误信息"))
            {
                Status.Text = "Error.";
                return;
            }
            var list = new List<string>();
        
            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//tr//td"))
            {
                var item = node.InnerHtml.Trim().Replace("&nbsp;", "");
                if (item.Contains("<") || String.IsNullOrEmpty(item))
                    continue;
                list.Add(item);
            }

            

            for (int i = 0; i < list.Count; i+=9)
            {
                var course = new SelectedCourse
                {
                    Id = list[i+0],
                    Serial = list[i+1],
                    Name = list[i+2],
                    Teacher = list[i+8]
                };
                CourseList.Items.Add(course);
            }

        }

        CancellationTokenSource source;

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            source = new CancellationTokenSource();
            Status.Text = "Monitoring...";
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            PeriodicCheckCourseAsync(TimeSpan.FromMilliseconds(1000), source.Token);
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            await Clients.ValidateCode.GetAndCheckCodeAsync(ih);
            Status.Text = "Stopped.";
            source.Cancel();
        }

        public async Task PeriodicCheckCourseAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                //await FooAsync();
                //if FooAsync get validateCode
                //if (true)
                //{
                //    await Clients.ValidateCode.GetAndCheckCodeAsync(ih);
                //}
                Status.Text = "Monitoring Dynamically...";
                await Task.Delay(interval, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }
    }
}
