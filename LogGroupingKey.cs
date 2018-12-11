using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogScraper
{
    class LogGroupingKey
    {
        public LogGroupingKey(string csReferer,string csUriStem)
        {
            this.csReferer = csReferer;
            this.csUriStem = csUriStem;
        }

        public string csReferer { get;  }

        public string csUriStem { get;  }
    }
}
