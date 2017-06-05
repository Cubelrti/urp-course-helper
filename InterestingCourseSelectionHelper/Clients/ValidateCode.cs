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

namespace InterestingCourseSelectionHelper.Clients
{
    class ValidateCode
    {
        public async static Task<Bitmap> getValidateCodeImage(InternetHelper ih)
        {
            Stream stream = await ih.GetAsyncStream(Address.GetValidateImageUri());
            Bitmap map = new Bitmap(Image.FromStream(stream));
            ImageProcessor.ToGrey(map);
            ImageProcessor.Thresholding(map);
            return map;
        }
        public async static Task<string> getValidateCode(Bitmap map)
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
    }
}
