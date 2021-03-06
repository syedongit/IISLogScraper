﻿using IISLogParser;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LogScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine("Welcome to Log Parser !");

            string pathtowritecsv;
            string logFolderPath;
            string LiftMasterBrand = ConfigurationManager.AppSettings["LiftMaster"];
            var filters = new List<string>();

            if (args.Length > 0)
            {
                filters = File.ReadAllText($@"..\..\{args[0]}").Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            if (LiftMasterBrand == "y")
            {
                logFolderPath = new FileInfo("..\\..\\Logs").FullName;
                pathtowritecsv = new FileInfo("..\\..\\Output").FullName + "\\LiftMaster_logs.csv";
            }
            else
            {
                logFolderPath = new FileInfo("..\\..\\ChamberlainLogs").FullName;
                pathtowritecsv = new FileInfo("..\\..\\Output").FullName + "\\Chamberlain_logs.csv";
            }

            //Read all files from Logs Folder
            var files = Directory.GetFiles(logFolderPath);
            var FinalLogInfoList = new List<LogInfo>();
            foreach (var file in files)
            {

                using (var parserEngine = new ParserEngine(file))
                {
                    while (parserEngine.MissingRecords)
                    {
                        FinalLogInfoList.AddRange(FilterList(parserEngine.ParseLog().Select(LogInfo.MapLogInfo), filters));
                        Console.WriteLine(FinalLogInfoList.Count);
                    }
                }
            }

            WriteToCSV(FinalLogInfoList, pathtowritecsv);

            Console.WriteLine($"Executed in {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }

        private static IEnumerable<LogInfo> FilterList(IEnumerable<LogInfo> logsInfoList, IReadOnlyCollection<string> filters)
        {
            return logsInfoList
                //TODO - include a report for 404's
                .Where(_ => _.Status == 200)
                .Where(_ => !filters.Any(filter => _.TargetUri.StartsWith(filter)))
                .GroupBy(p => new { csReferer = p.Referer, csUriStem = p.TargetUri })
                .Select(_ => new LogInfo { Referer = _.Key.csReferer, TargetUri = _.Key.csUriStem, Count = _.Count() })
                .OrderByDescending(_ => _.Count);
        }

        private static void WriteToCSV(IEnumerable<LogInfo> logsInfo, string pathtowritecsv)
        {
            var itemType = typeof(LogInfo);
            var properties = itemType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).OrderBy(p => p.Name).ToList();
            using (var writer = new StreamWriter(pathtowritecsv, true))
            {
                //write the header line 
                writer.WriteLine(string.Join(", ", properties.Select(p => p.Name)));
                foreach (var log in logsInfo)
                {
                    writer.WriteLine(string.Join(", ", properties.Select(p => p.GetValue(log))));
                }
                writer.Close();
            }
        }
    }
}
