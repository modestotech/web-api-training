using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiTestProject1.Helpers
{
    public static class Validation
    {
        /// <summary>
        /// Validate the api key according to the rules
        /// </summary>
        public static bool IsValidApiKey(string sKey)
        {
            return (!string.IsNullOrEmpty(sKey) &&
                (sKey.StartsWith("R") || sKey.StartsWith("W")) &&
                sKey.Length == 6);
        }
    }
}