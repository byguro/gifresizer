using System;
using System.Configuration;

namespace gifresizer.test.web
{
    public static class Helper
    {
        public static string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string BaseGifFileDirectory
        {
            get { return GetSetting("BaseGifFileDirectory"); }
        }

        public static int ToInt(this object item)
        {
            var result = 0;
            try
            {
                result = Convert.ToInt32(item);
            }
            catch {}
            return result;
        }
    }
}