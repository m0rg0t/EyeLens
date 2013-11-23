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
        public static string GetColorName(byte R, byte G, byte B)
        {
            try
            {
                var data = JArray.Parse(AppResources.jsonColorData);
                foreach(var item in data)
                {
                    Debug.WriteLine(item.ToString());
                }
            }
            catch { };

            return "";
        }
    }
}
