using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeReaction.Web.Tools
{
    public static class Parser
    {
        public static T? ParseNullable<T>(string str) where T : struct
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return (T)Convert.ChangeType(str, typeof(T));
        }
    }
}