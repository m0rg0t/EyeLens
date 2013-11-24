using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeLens.Resources;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace EyeLens.Helpers
{
    public struct RGBColor
    {
        public byte R;
        public byte G;
        public byte B;
    };

    public static class ColorNameDictionary
    {
        public static void InitDictionary()
        {
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private static int GetValueDiff(RGBColor c1, RGBColor c2)
        {
            return Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B);
        }
            
        /// <summary>
        /// Get nearest name for color
        /// </summary>
        /// <param name="R">red component</param>
        /// <param name="G">green component</param>
        /// <param name="B">blue component</param>
        /// <returns></returns>
        public static string GetColorName(RGBColor grabbedColor)
        {
            string _outValue = "";
            try
            {
                string jsonStr = AppResources.jsonColorData.Replace("\\", "");
                var minDiff = Int32.MaxValue;
                var data = JObject.Parse(jsonStr);
                foreach(var item in data)
                {
                    try
                    {
                        string key = item.Key.ToString();
                        byte[] colorCode = ColorNameDictionary.StringToByteArray(key);
                        RGBColor c = new RGBColor();
                        c.R = colorCode[0];
                        c.G = colorCode[1];
                        c.B = colorCode[2];

                        int curDiff = GetValueDiff(grabbedColor, c);
                        if (minDiff > curDiff)
                        {
                            minDiff = curDiff;
                            _outValue = item.Value.ToString();
                        }
                    }
                    catch { };
                }
            }
            catch { };

            return _outValue;
        }
    }
}
