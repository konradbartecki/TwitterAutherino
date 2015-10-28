using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAutherino.Model
{
    public static class Strings
    {
        public const string ExceptionUnauthorized =
            "Twitter service returned Unauthorized 401. Please double check if your date/time is current and check your consumer key and secret";

        public const string MissingAccessToken =
            "Missing access token (null). Please create TwitterAuth object with access token or get it through methods in following order: GetRequestTokenAsync, GetAccessTokenAsync";
    }
}
