using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IISLogParser;
namespace LogScraper
{
    public class LogInfo
    {

        public static LogInfo MapLogInfo(IISLogEvent iiSLogEvent) {


            return new LogInfo
            {
                scStatus = iiSLogEvent.scStatus,
                scSubstatus = iiSLogEvent.scSubstatus,
                csReferer = iiSLogEvent.csReferer,
                csUserAgent = iiSLogEvent.csUserAgent,
                csUriStem  = iiSLogEvent.csUriStem

            };
        }

        public int? scSubstatus { get; set; }
        public int? scStatus { get; set; }

        public string csReferer { get; set; }
        public string csUserAgent { get; set; }
        public string csUriStem { get; set; }

    }
}
