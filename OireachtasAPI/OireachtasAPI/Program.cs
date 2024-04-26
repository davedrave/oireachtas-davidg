using CommandLine;
using OireachtasAPI.Services;
using OireachtasAPI.Enums;
using OireachtasAPI.Factories;
using System;
using OireachtasAPI.DataLoaders;
using System.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using Microsoft.SqlServer.Server;

namespace OireachtasAPI
{
    public class Program
    {
        public static string LEGISLATION_DATASET = "legislation.json";
        public static string MEMBERS_DATASET = "members.json";
        
        // Keep Irish culture info for now incase a machines set to US when parsing dates
        private static CultureInfo cultureInfo = new CultureInfo("en-IE");

        public class Options
        {
            [Option('p', "path", Required = false, HelpText = "Set the source path for the data, be it a file path or a url.")]
            public string Path { get; set; }

            [Option('m', "memberName", Required = false, HelpText = "Member ID to filter the bills.")]
            public string MemberName { get; set; }

            [Option('s', "dateSince", Required = false, HelpText = "The date from which to filter the bills")]
            public string DateSince { get; set; }

            [Option('u', "dateUntil", Required = false, HelpText = "The up to which the bills will be filtered.")]
            public string DateUntil { get; set; }

            [Option('f', "filterType", Required = false, HelpText = "The Type of filter requested.")]
            public string FilterType { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed(parameters =>
            {
                if(Enum.TryParse(parameters.FilterType, false, out FilterType filterType))
                {
                    IDataLoader dataLoader = DataLoaderFactory.GetLoader(parameters.Path);
                    // Call OireactasService with the provided parameters
                    OireachtasService service = new OireachtasService(dataLoader);

                    if (filterType == FilterType.FilterBillsSponsoredBy)
                    {
                        if (parameters.MemberName == null)
                        {
                            throw new ArgumentException("Parameter must not be null or empty for FilterBillsSponsoredBy");
                        }
                        else
                        {
                            Console.WriteLine($"Filtering Bills by Sponsor: {parameters.MemberName}");
                            //TODO Exception 
                            List<dynamic> result = service.FilterBillsSponsoredBy(parameters.MemberName);
                            PrintResult(result);
                        }
                    }
                    else if (filterType == FilterType.FilterBillsLastUpdated)
                    {
                        /*Make dateparameters robust by defaulting to extremes if explicit value not provided
                         * E.G no dateUntil will result in including everything up to current date
                         * */
                        if (!DateTime.TryParse(parameters.DateSince, cultureInfo, DateTimeStyles.None, out DateTime dateSince))
                        {
                            dateSince = DateTime.MinValue;
                        }

                        if (!DateTime.TryParse(parameters.DateUntil, cultureInfo, DateTimeStyles.None, out DateTime dateUntil))
                        {
                            dateUntil = DateTime.Now;
                        }

                        //TODO Exception 
                        var result = service.FilterBillsByLastUpdated(dateSince, dateUntil);
                        PrintResult(result);
                    }else
                    {
                        // should not get here unless an additional enum is added to FilterTypes without updating this area.
                        throw new InvalidEnumArgumentException();
                    }
                }                
                else
                {
                    Console.WriteLine("Parameters not provided, choose your parameters");

                    int value = 0;

                    while(value != 1 && value != 2 )
                    {
                        Console.WriteLine($"Choose 1 for {FilterType.FilterBillsSponsoredBy}, 2 for {FilterType.FilterBillsLastUpdated}:");
                        string result  = Console.ReadKey().KeyChar.ToString();
                        if(!int.TryParse(result, out value) || value != 1 && value != 2)
                        {
                            Console.WriteLine("\nInvalid choice.");
                        }
                    }

                    string path = string.Empty;
                    while(path == string.Empty)
                    {
                        Console.WriteLine("\nPlease supply a path to your datasource. This can be a local path or url.");
                        //TODO Path validation could be used here and in DataLoaders or Factory.
                        path = Console.ReadLine();
                        if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                        {
                        }else
                        {
                            Console.WriteLine("The path must start with 'http' or end in .json to denote a url or a local path. Please try again.");
                            path = string.Empty;
                        }
                    }

                    if(filterType == FilterType.FilterBillsSponsoredBy)
                    {
                        string memberName = string.Empty;

                        while (memberName == string.Empty)
                        {
                            Console.WriteLine("Please supply a Member Name to filter by. This cannot be blank");
                            memberName = Console.ReadLine();
                        }


                        IDataLoader dataLoader = DataLoaderFactory.GetLoader(path);
                        // Call OireactasService with the provided parameters
                        OireachtasService service = new OireachtasService(dataLoader);

                        //TODO Exception 
                        var result = service.FilterBillsSponsoredBy(memberName);
                        PrintResult(result);
                    }
                    else if (filterType == FilterType.FilterBillsSponsoredBy)
                    {
                        //use maxvalue as null to avoid a temp DateTime value with tryparse.
                        DateTime dateSince = DateTime.MaxValue;
                        while(dateSince == DateTime.MaxValue)
                        {
                            Console.WriteLine("Please supply a Date to filter from. Leaving this blank can result in Bills from the beginning of time being included.");
                            string date = Console.ReadLine();

                            if (!string.IsNullOrEmpty(date)) 
                            {
                                if(!DateTime.TryParse(date, out dateSince))
                                {
                                    Console.WriteLine($"Invalid Date format. Provide date in format {cultureInfo.DateTimeFormat}");
                                }
                            }else
                            {
                                Console.WriteLine("DateSince bypassed.");
                                dateSince = DateTime.MinValue;
                            }
                        }

                        IDataLoader dataLoader = DataLoaderFactory.GetLoader(parameters.Path);
                        // Call OireactasService with the provided parameters
                        OireachtasService service = new OireachtasService(dataLoader);

                        //TODO Exception 
                        var result = service.FilterBillsByLastUpdated(dateSince, dateSince);
                        PrintResult(result);

                    }

                    Console.WriteLine("Parameters not provided, choose your parameters");
                }
            })
            .WithNotParsed(errors =>
            {
                // Handle parsing errors
                foreach (Error error in errors)
                {
                    Console.WriteLine(error);
                }
            });

            Console.WriteLine("Press any button to quit.");
            Console.ReadKey();

            void PrintResult(List<dynamic> toPrint)
            {
                if(toPrint == null || toPrint.Count == 0)
                {
                    Console.WriteLine("Results null or empty!");
                }
                else
                {


                    foreach (dynamic p in toPrint)
                    {
                        string prettyJson = JsonConvert.SerializeObject(p, Formatting.Indented);
                        Console.WriteLine(prettyJson);
                    }
                }
            }
        }       
    }
}
