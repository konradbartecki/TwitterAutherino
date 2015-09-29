using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAutherino.Model
{
    public class User
    {
        public string ScreenName { get; set; }
        public Keypair AccessKeypair { get; set; }
    }
}
