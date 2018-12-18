using IISLogParser;

namespace LogScraper
{
    public class LogInfo
    {

        public static LogInfo MapLogInfo(IISLogEvent iiSLogEvent)
        {
            return new LogInfo
            {
                Status = iiSLogEvent.scStatus,
                Referer = iiSLogEvent.csReferer,
                UserAgent = iiSLogEvent.csUserAgent,
                TargetUri  = iiSLogEvent.csUriStem
            };
        }

        public int? Status { get; set; }
        public string Referer { get; set; }
        public string UserAgent { get; set; }
        public string TargetUri { get; set; }
        public int Count { get; set; }
    }
}
