using System.Globalization;
using Microsoft.Extensions.Localization;
using System.Reflection;
using System.Resources;

namespace UseTheOps.PolyglotInitiative
{
    public static class LocalizationHelper
    {
        private static readonly ResourceManager ResourceManager = new ResourceManager("UseTheOps.PolyglotInitiative.Resources.Strings", Assembly.GetExecutingAssembly());

        public static string GetString(string key, params object[] args)
        {
            var value = ResourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
            return args.Length > 0 ? string.Format(value, args) : value;
        }
    }
}
