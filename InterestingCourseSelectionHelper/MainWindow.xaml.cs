using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UrpSelectionHelper.Clients;
using UrpSelectionHelper.Helpers;
using System.Drawing;
using UrpSelectionHelper.Interfaces;
using System.Net;
using HtmlAgilityPack;
using UrpSelectionHelper.Dto;
using System.Threading;
using System.Net.Http;
using InterestingCourseSelectionHelper.Interfaces;
using System.Media;

namespace UrpSelectionHelper
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
            Username.Focus();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ih = new InternetHelper();
            Bitmap bitmap = await ValidateCode.GetValidateCodeImage(ih,"0.1");
            ValidateCodeImage.Source = ImageProcessor.ToBitmapImage(bitmap);
            string validateResult = await ValidateCode.GetValidateCode(bitmap);
            ValidateResult.Text = "ValidateResult: " + validateResult;
            var result = await FormAction.SignIn(Username.Text, Password.Password, validateResult, ih);
            if ((await result.ReadAsStringAsync()).Contains("学分制综合教务"))
            {
                // 播放提示音
                SoundPlayer simpleSound = new SoundPlayer(@"C:\Users\ti\Documents\GitHub\urp-course-helper\InterestingCourseSelectionHelper\surprise.wav");
                simpleSound.Play();
                //Login.IsEnabled = false;
                var cookies = ih.Cookies;
                IEnumerable<Cookie> responseCookies = cookies.GetCookies(FreeAddress.GetIPUri()).Cast<Cookie>();
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
            HttpContent result;
            if (isLimited.IsChecked.Value)
            {
                result = await FormAction.GetLimitedClass(ih);
            }
            else {
                result = await FormAction.GetFreeClass(ih);
            }
            var text = await result.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            if (text.Contains("错误信息"))
            {
                Status.Text = "Error.";
                NotificationPanel.Items.Add(new Notification { Action = "getclass", Time = DateTime.Now.ToString(), Result = "error" });
                return;
            }
            var list = new List<string>();
            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//tr//td"))
            {
                var item = node.InnerHtml.Trim().Replace("&nbsp;", "");
                if (item.Contains("kcxx") && !item.Contains("选择"))
                {
                    list.Add(node.InnerText.Trim());
                    continue;
                }
                if (item.Contains("<") || String.IsNullOrEmpty(item))
                    continue;
                list.Add(item);
            }


            
            while (true)
            {
                var teacher = list.Find((item) => item.Contains("*"));
                if(teacher == null)
                {
                    break;
                }
                var course = new SelectedCourse
                {
                    Id = list[0],
                    Serial = list[1],
                    Name = list[2],
                    Teacher = teacher
                };
                var ind = list.FindIndex((item) => item.Equals(teacher));
                list = list.Skip(ind+1).ToList();
                CourseList.Items.Add(course);
            }
            


        }

        CancellationTokenSource source;
        string validateBuffer;

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            source = new CancellationTokenSource();
            Status.Text = "Monitoring...";
            //TODO:implement this
            await FetchNewBuffer();
            PeriodicCheckFreeCourseAsync(TimeSpan.FromMilliseconds(Int32.Parse(MonitorInterval.Text)), source.Token);
        }

        private async Task FetchNewBuffer()
        {
            validateBuffer = await ValidateCode.GetAndCheckCodeAsync(ih, isLimited.IsChecked.Value);
            if (validateBuffer != null)
            {
                ValidateResult.Text = "valid at." + DateTime.Now.ToShortTimeString();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Status.Text = "Stopped.";
            source.Cancel();
        }

        public async void PeriodicCheckFreeCourseAsync(TimeSpan interval, CancellationToken cancellationToken,bool isCross = false)
        {
            if(isLimited.IsChecked.Value)
            {
                PeriodicCheckLimitedCourseAsync(interval, cancellationToken);
                return;
            }
            while (true)
            {
                try
                {
                    //var list = await QueryCourse.GetSelectableCourseAsync(ih,isCross);

                    foreach (MonitoringCourse item in MonitorCourse.Items)
                    {
                        /**
                            var avalible = list.Find((i) =>
                            i.Id == item.Id &&
                            i.Serial == item.Serial &&
                            i.Selectable == "yes");
                        **/
                        
                        if (!await QueryCourse.QueryIfSelectable(item.Id,item.Serial,ih))
                        {
                            var text = "Last check:nope!";
                            item.Status = "nope at "+ DateTime.Now.ToShortTimeString();
                            //NotificationPanel.Items.Add(new Notification { Action = "checkclass",Time=DateTime.Now.ToString(),Result=text });
                            Status.Text = text;
                            MonitorCourse.Items.Refresh();
                        }

                        else
                        {
                            // 开始抢课逻辑
                            // 播放提示音
                            SoundPlayer simpleSound = new SoundPlayer(@"C:\Users\ti\Documents\GitHub\urp-course-helper\InterestingCourseSelectionHelper\surprise.wav");
                            simpleSound.Play();
                            await ih.GetAsync(FreeAddress.GetStartValidateUri());
                            string code;
                            if (String.IsNullOrEmpty(validateBuffer))
                            {
                                code = validateBuffer;
                                validateBuffer = null;
                                //using buffer here
                            }
                            //buffer not avalible
                            else code = await ValidateCode.GetAndCheckCodeAsync(ih, isLimited.IsChecked.Value);
                            var result = await SelectCourse.TrySelect(item.Id + "_" + item.Serial, code, ih,isCross);
                            if (result.Contains("成功"))
                            {
                                var text = Status.Text = $"Selected{item.Id}! gratz";
                                NotificationPanel.Items.Add(new Notification { Action = "selectclass", Time = DateTime.Now.ToString(), Result = text });
                                item.Status = text;
                                MonitorCourse.Items.Remove(item);
                                continue;
                            }
                            if (result.Contains("最高门数"))
                            {
                                var text = Status.Text = "Exceed maximum.";
                                NotificationPanel.Items.Add(new Notification { Action = "selectclass", Time = DateTime.Now.ToString(), Result = text });
                                item.Status = text;
                                MonitorCourse.Items.Remove(item);
                                continue;
                            }
                            Status.Text = "Failed. " + DateTime.Now.ToString();
                            NotificationPanel.Items.Add(new Notification { Action = "selectclass", Time = DateTime.Now.ToString(), Result = $"{item.Id} failed ${result.Trim()}" });
                        }
                    }

                    
                }
                catch (Exception)
                {
                    await Task.Delay(interval, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    continue;
                }
                await Task.Delay(interval, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }
        public async void PeriodicCheckLimitedCourseAsync(TimeSpan interval, CancellationToken cancellationToken, bool isCross = false)
        {
            while (true)
            {
                try
                {
                    var list = await QueryCourse.GetSelectableCourseAsync(ih,isCross);
                    foreach (MonitoringCourse item in MonitorCourse.Items)
                    {
                        var avalible = list.Find((i) =>
                        i.Id == item.Id &&
                        i.Serial == item.Serial &&
                        i.Selectable == "yes");
                        if(avalible == null)
                        {
                            var text = "Last check:nope!";
                            item.Status = "nope at " + DateTime.Now.ToShortTimeString();
                            //NotificationPanel.Items.Add(new Notification { Action = "checkclass",Time=DateTime.Now.ToString(),Result=text });
                            Status.Text = text;
                            MonitorCourse.Items.Refresh();
                            continue;
                        }
                        // 开始抢课逻辑
                        // 播放提示音
                        SoundPlayer simpleSound = new SoundPlayer(@"C:\Users\ti\Documents\GitHub\urp-course-helper\InterestingCourseSelectionHelper\surprise.wav");
                        simpleSound.Play();

                        await ih.GetAsync(LimitedAddress.GetStartValidateUri());
                        string code;
                        if (String.IsNullOrEmpty(validateBuffer))
                        {
                            code = validateBuffer;
                            validateBuffer = null;
                            //using buffer here
                        }
                        //buffer not avalible
                        else code = await ValidateCode.GetAndCheckCodeAsync(ih, isLimited.IsChecked.Value);
                        var result = await SelectCourse.TrySelectLimited(item.Id + "_" + item.Serial, code, ih);
                        if (result.Contains("成功"))
                        {
                            var text = Status.Text = $"Selected{item.Id}! gratz";
                            NotificationPanel.Items.Add(new Notification { Action = "selectclass", Time = DateTime.Now.ToString(), Result = text });
                            item.Status = text;
                            MonitorCourse.Items.Remove(item);
                            continue;
                        }
                        if (result.Contains("最高门数"))
                        {
                            var text = Status.Text = "Exceed maximum.";
                            NotificationPanel.Items.Add(new Notification { Action = "selectclass", Time = DateTime.Now.ToString(), Result = text });
                            item.Status = text;
                            MonitorCourse.Items.Remove(item);
                            continue;
                        }
                        Status.Text = "Failed. " + DateTime.Now.ToString();
                        NotificationPanel.Items.Add(new Notification { Action = "selectclass", Time = DateTime.Now.ToString(), Result = $"{item.Id} failed ${result.Trim()}" });

                    }


                }
                catch (Exception)
                {
                    await Task.Delay(interval, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    continue;
                }
                try { await Task.Delay(interval, cancellationToken); } catch { break; }
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }
        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            SelectableList.Items.Clear();
            var result = await FormAction.GetSelectableCourse(ih);
            var text = await result.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(text);
            if (text.Contains("错误信息"))
            {
                Status.Text = "Error.";
                return;
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
                    bigList.Add("no " + node.InnerText
                        .Trim()
                        .Split('\n')
                        .Aggregate((str1,str2)=>str1.Trim()+" "+str2.Trim()));
                }
                if (item.Contains("checkbox"))
                {
                    bigList.Add("yes");
                }
                if (item.Contains("<") || String.IsNullOrEmpty(item))
                    continue;
                bigList.Add(item);
            }




            for (int i = 0; i < bigList.Count; i += 10)
            {
                var course = new SelectedCourse
                {
                    Selectable = bigList[i + 0],
                    Id = bigList[i + 1],
                    Serial = bigList[i + 3],
                    Name = bigList[i + 2],
                    Teacher = bigList[i + 6]
                };
                SelectableList.Items.Add(course);
            }

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var course = new MonitoringCourse
            {
                Id = MonitorID.Text,
                Serial = MonitorSerial.Text,
                Status = "Unselected."
            };
            MonitorCourse.Items.Add(course);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            MonitorCourse.Items.Remove(MonitorCourse.SelectedItem);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            source = new CancellationTokenSource();
            Status.Text = "Monitoring...";
            PeriodicCheckFreeCourseAsync(TimeSpan.FromMilliseconds(5000), source.Token,true);
        }

        private async void ValidateCodeImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Status.Text = "Refreshing...";
            await FetchNewBuffer();
        }
    }
}
