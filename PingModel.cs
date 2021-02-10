using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeAsistant
{
    public class PingModel
    {
        public string address { get; set; }
        public long roundtripTime { get; set; }
        public int ttl { get; set; }
        public bool dontFragment { get; set; }
        public int bufferLength { get; set; }
    }
}
