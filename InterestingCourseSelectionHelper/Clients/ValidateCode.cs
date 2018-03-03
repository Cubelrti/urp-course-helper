using System.Threading.Tasks;
using System.Drawing;
using UrpSelectionHelper.Helpers;
using UrpSelectionHelper.Interfaces;
using Tesseract;
using System.IO;
using System.Net.Http;
using InterestingCourseSelectionHelper.Interfaces;

namespace UrpSelectionHelper.Clients
{
    class ValidateCode
    {
        public async static Task<Bitmap> GetValidateCodeImage(InternetHelper ih,string random)
        {
            Stream stream = await ih.GetAsyncStream(FreeAddress.GetValidateImageUri(random));
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

        public async static Task<string> GetAndCheckCodeAsync(InternetHelper ih, bool isLimited)
        {

            string code = "";
            var map = await GetValidateCodeImage(ih,"100");
            code = await GetValidateCode(map);
            string result;
            if(isLimited)
                result = await (await ih.GetAsync(LimitedAddress.GetValidateCheckUri(code))).ReadAsStringAsync();
            else result = await (await ih.GetAsync(FreeAddress.GetValidateCheckUri(code))).ReadAsStringAsync();
            if (result.Contains("true"))
            {
                return code;
            }

            else return await GetAndCheckCodeAsync(ih, isLimited);
        }

        public async static Task<bool> CheckValidateCode(string validateCode, InternetHelper ih)
        {
            var result = await ih.GetAsync(FreeAddress.GetValidateCheckUri(validateCode));
            var text = await result.ReadAsStringAsync();
            return text.Contains("true");
        }
    }
}
