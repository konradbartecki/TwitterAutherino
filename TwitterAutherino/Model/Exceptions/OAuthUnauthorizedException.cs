using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAutherino.Model.Exceptions
{
    public class OAuthUnauthorizedException : Exception
    {
        public OAuthUnauthorizedException()
        {
        }

        public OAuthUnauthorizedException(string message) : base(message)
        {
        }
    }
}
