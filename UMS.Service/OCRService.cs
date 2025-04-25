using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMS.Core.Entities;

namespace UMS.Service
{
    public class OCRService : IOCRService
    {
        private readonly string _tessDataPath;

        public OCRService(IWebHostEnvironment env)
        {
            _tessDataPath = Path.Combine(env.ContentRootPath, "tessdata");
        }
        public Task<string> AnalyzeStudentImageAsync(string imagePath)
        {
          string _tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tessdata");

        string extractedText = OCRHelper.ExtractArabicText(imagePath, _tessDataPath, out string _errMsg);
            if (!string.IsNullOrEmpty(_errMsg))
                return Task.FromResult($"خطأ في قراءة النص من الصورة: {_errMsg}") ;

            string department = OCRHelper.GetDepartment(extractedText, out string errMsg);
            if (!string.IsNullOrEmpty(errMsg))
                return Task.FromResult ($"خطأ في استخراج الشعبة: {errMsg}");

            double summation = OCRHelper.GetStummation(extractedText, out string errMsg1);
            if (summation == -1)
                return Task.FromResult ($"خطأ في استخراج المجموع: {errMsg1}");

            // الحدود حسب التخصص
            double computerScienceSum = 300;
            double engineeringSum = 400;
            double businessAdministrationSum = 250;

            string resultMessage = string.Empty;

            switch (department)
            {
                case "علوم حاسب":
                case "علوم الحاسب":
                    resultMessage = (summation >= computerScienceSum)
                        ? "مقبول"
                        : "عذرا لم يتم قبول الطالب";
                    break;

                case "هندسة":
                case "هندسه":
                    resultMessage = (summation >= engineeringSum)
                        ? "مقبول"
                        : "عذرا لم يتم قبول الطالب";
                    break;

                case "ادارة اعمال":
                case "إدارة اعمال":
                case "إدارة أعمال":
                case "ادارة أعمال":
                    resultMessage = (summation >= businessAdministrationSum)
                        ? "مقبول"
                        : "عذرا لم يتم قبول الطالب";
                    break;

                default:
                    resultMessage = "لم يتم التعرف على الشعبة.";
                    break;
            }

            return Task.FromResult($"الشعبة: {department}\nالمجموع: {summation}\nالنتيجة: {resultMessage}");
        }

       
    }
    }
