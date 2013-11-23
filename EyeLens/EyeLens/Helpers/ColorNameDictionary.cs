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

        /// <summary>
        /// Get nearest name for color
        /// </summary>
        /// <param name="R">red component</param>
        /// <param name="G">green component</param>
        /// <param name="B">blue component</param>
        /// <returns></returns>
        public static string GetColorName(byte R, byte G, byte B)
        {
            string _outValue = "";
            try
            {
                string jsonStr = AppResources.jsonColorData.Replace("\\", "");
                var data = JObject.Parse(jsonStr);
                foreach(var item in data)
                {
                    string key = item.Key.ToString();
                    byte[] colorCode = ColorNameDictionary.StringToByteArray(key);
                    byte Rcode = colorCode[0];
                    byte Gcode = colorCode[1];
                    byte Bcode = colorCode[2];

                    string value = item.Value.ToString();
                }
            }
            catch { };

            return _outValue;
        }
    }
}
