using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAutherino.Model.Exceptions
{
    public class OAuthMissingAccessTokenException : Exception
    {
        public OAuthMissingAccessTokenException()
        {
            
        }

        public OAuthMissingAccessTokenException(string message) : base(message)
        {
            
        }
    }
}
