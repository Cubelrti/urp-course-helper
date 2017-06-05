using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using InterestingCourseSelectionHelper.Helpers;
using InterestingCourseSelectionHelper.Interfaces;
using Tesseract;
using System.IO;
using System.Net.Http;

namespace InterestingCourseSelectionHelper.Clients
{
    class ValidateCode
    {
        public async static Task<Bitmap> GetValidateCodeImage(InternetHelper ih)
        {
            Stream stream = await ih.GetAsyncStream(Address.GetValidateImageUri());
            Bitmap map = new Bitmap(Image.FromStream(stream));
            ImageProcessor.ToGrey(map);
            ImageProcessor.Thresholding(map);
            return map;
        }
        public async static Task<string> GetValidateCode(Bitmap map)
        {
            const string dataUri = @"C:\Program Files (x86)\Tesseract-OCR\tessdata\";
            const string lang = "eng";
            const string defaultList = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            TesseractEngine ocr = new TesseractEngine(dataUri, lang);
            ocr.SetVariable("tessedit_char_whitelist", defaultList);
            return await Task.Run(() => {
                Page pg = ocr.Process(map, pageSegMode: ocr.DefaultPageSegMode);
                return pg.GetText().Trim().Replace(" ", "").ToLower();
            });
        }

        public async static Task GetAndCheckCodeAsync(InternetHelper ih)
        {
            bool isValidateCodeIncorrect = true;
            const int MAXIMUM_RETRY_TIMES = 8;
            int tryTime = 0;
            while (isValidateCodeIncorrect || tryTime > MAXIMUM_RETRY_TIMES) 
            {
                var map = await GetValidateCodeImage(ih);
                var code = await GetValidateCode(map);
                isValidateCodeIncorrect = !await CheckValidateCode(code, ih);
            }

            return;
        }

        public async static Task<bool> CheckValidateCode(string validateCode, InternetHelper ih)
        {
            var result = await ih.GetAsync(Address.GetValidateCheckUri(validateCode));
            var text = await result.ReadAsStringAsync();
            return text.Contains("TRUE");
        }
    }
}
