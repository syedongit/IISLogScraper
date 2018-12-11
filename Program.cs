using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IISLogParser;
namespace LogScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Log Parser !");
            string path = "D:\\CGI\\test\\u_ex181124.log";
            string pathtowritecsv = "D:\\CGI\\test\\test.csv";

            List<IISLogEvent> logs = new List<IISLogEvent>();
            using(ParserEngine parser = new ParserEngine(path))
            {
                while (parser.MissingRecords)
                {
                    logs = parser.ParseLog().ToList();
                    logs = logs.Where(p => p.scStatus == 200).Select(p => p).ToList();
                    var logsInfoList = logs.Select(p => LogInfo.MapLogInfo(p)).ToList();
                    logsInfoList = FilterList(logsInfoList);
                
                    WriteToCSV(logsInfoList, pathtowritecsv);
                    //logs.ForEach(x=>Console.WriteLine(parser.CurrentFileRecord));
                    
                }
            }

            Console.Read();
        }

        private static List<LogInfo> FilterList(List<LogInfo> logsInfoList)
        {
            var returnlist = new List<LogInfo>();
            var groupedreturnlist = logsInfoList.GroupBy(p => new LogGroupingKey(p.csReferer,p.csUriStem)).Select(p=>p).ToList();

            foreach( var group in groupedreturnlist)
            {
                Console.WriteLine($"URL: {group.Key.csUriStem}, Referer: {group.Key.csUriStem}, Count: {group.Count()}");
                //foreach(var value in group)
                   // returnlist.Add(value);
            }

            return returnlist;
        }

        private static void WriteToCSV(List<LogInfo> logsInfo, string pathtowritecsv)
        {
            Type itemType = typeof(LogInfo);
            var properties = itemType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).OrderBy(p => p.Name);
            using (var writer = new StreamWriter(pathtowritecsv, true))
            {
                //write the header line 
                writer.WriteLine(string.Join(", ", properties.Select(p=>p.Name) ));
                foreach(var log in logsInfo)
                {
                    writer.WriteLine(string.Join(", ", properties.Select(p => p.GetValue(log))));
                }
                writer.Close();
            }

        }
    }
}
