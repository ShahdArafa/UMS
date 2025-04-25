using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;

namespace UMS.Service
{
    public class OCRHelper
    {
      public  static string ExtractArabicText(string imagePath, string tessDataPath, out string errMsg)
        {
            
            errMsg = string.Empty;
            if (!File.Exists(imagePath))
            {
                errMsg = new FileNotFoundException("Image file not found.", imagePath).ToString();
                return string.Empty;
            }

            // Initialize Tesseract engine with Arabic language
            // (Consider "ara+eng" if you have mixed Arabic/English).
            string fullTessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "tessdata");
            using (var engine = new TesseractEngine(tessDataPath, "ara", EngineMode.Default))
            {
                // Add punctuation: dot, Arabic decimal separator, Arabic comma, etc.
                engine.SetVariable("tessedit_char_whitelist",
                   "ابتثجحخدذرزسشصضطظعغفقكلمنهوي٠١٢٣٤٥٦٧٨٩0123456789.,٫،: ");

                // Optionally specify user-defined DPI if your images do not have DPI metadata
                engine.SetVariable("user_defined_dpi", "300");

                using (var img = Pix.LoadFromFile(imagePath))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
            }
        }

      public  static double GetStummation(string fullText, out string errMsg)
        {
            try
            {
                errMsg = string.Empty;


                // Convert Arabic punctuation/digits to western equivalents
                fullText = ConvertArabicSignsAndDigitsToWestern(fullText);


                // Pattern that matches "المجموع 329.0" or "المجموع : 329.0"
                string patternSum = @"المجموع\s*:?\s*(\d+(\.\d+)?)";
                Regex regexSum = new Regex(patternSum, RegexOptions.Multiline);

                Match matchSum = regexSum.Match(fullText);
                if (matchSum.Success)
                {
                    string extractedValue = matchSum.Groups[1].Value;

                    // Try parse
                    if (double.TryParse(extractedValue,
                                        NumberStyles.Any,
                                        CultureInfo.InvariantCulture,
                                        out double sum))
                    {
                        return sum;
                    }
                    else
                    {
                        errMsg = "تعذر تحويل المجموع إلى رقم.";
                        return -1;
                    }
                }
                else
                {
                    errMsg = "لم يتم العثور على المجموع في النص.";
                    return -1;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
                return -1;
            }
        }

      public  static string ConvertArabicSignsAndDigitsToWestern(string input)
        {
            // 1. Convert Arabic colon (if present) to ASCII colon
            input = input.Replace('：', ':');

            // 2. Convert Arabic decimal separators to '.'
            // These are common in Arabic text for decimal separation
            input = input.Replace('٫', '.');  // ARABIC DECIMAL SEPARATOR (U+066B)
            input = input.Replace('،', '.');  // ARABIC COMMA (U+060C) used sometimes

            // 3. Convert Arabic-Indic digits (٠١٢٣٤٥٦٧٨٩) to Western digits (0-9)
            char[] arabicDigits = { '٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩' };
            for (int i = 0; i < arabicDigits.Length; i++)
            {
                input = input.Replace(arabicDigits[i], (char)('0' + i));
            }

            return input;
        }

       public static string GetDepartment(string fullText, out string errMsg)
        {
            try
            {
                errMsg = string.Empty;

                // This will match:
                // "شعب" + (اختياري: "ة" أو "ه") + مسافات + النص التالي حتى نهاية السطر
                string patternShuaba = @"شعب(?:ة|ه)?\s+([^\r\n]+)";

                // يطابق:
                // "شعبة" + فراغات + أي نص حتى نهاية السطر (دون تضمين التبديل للسطر)

                Regex regexShuaba = new Regex(patternShuaba, RegexOptions.Multiline);

                Match matchShuaba = regexShuaba.Match(fullText);
                if (matchShuaba.Success)
                {
                    // المجموعة الأولى (Group 1) فيها اسم الشعبة، مثل "علوم الحاسب"
                    string shuabaValue = matchShuaba.Groups[1].Value.Trim();

                    return shuabaValue;
                }
                else
                {
                    errMsg = "لم يتم العثور على الشعبة في النص.";
                    return string.Empty;
                }

            }
            catch (Exception ex)
            {

                errMsg = ex.ToString();
                return string.Empty;
            }
        }
    }
}
