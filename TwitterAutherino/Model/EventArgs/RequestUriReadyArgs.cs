using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAutherino.Model.EventArgs
{
    class RequestUriReadyArgs : System.EventArgs
    {
        public Uri RequestUri { get; set; }
    }
}
