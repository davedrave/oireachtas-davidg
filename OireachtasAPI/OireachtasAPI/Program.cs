using System;
using System.Collections.Generic;
using System.Globalization;
using CommandLine;
using OireachtasAPI.Services;
using OireachtasAPI.Enums;
using OireachtasAPI.Factories;
using OireachtasAPI.Repositories;
using OireachtasAPI.DataLoaders;
using Newtonsoft.Json;
using System.ComponentModel;
using Serilog;

namespace OireachtasAPI
{
    public class Program
    {
        private static readonly CultureInfo cultureInfo = new CultureInfo("en-IE");

        public class Options
        {
            [Option('s', "source", Required = false, HelpText = "Set the source for the data, 1 for Local and 2 for API.")]
            public SourceType SourceType { get; set; }

            [Option('m', "memberId", Required = false, HelpText = "Member ID to filter the bills.")]
            public string MemberId { get; set; }

            [Option('f', "dateFrom", Required = false, HelpText = "The date from which to filter the bills")]
            public string DateFrom { get; set; }

            [Option('u', "dateUntil", Required = false, HelpText = "The up to which the bills will be filtered.")]
            public string DateUntil { get; set; }

            [Option('t', "filterType", Required = false, HelpText = "The Type of filter requested. 1 for FilterBillsSponsoredBy, 2 for FilterBillsLastUpdated")]
            public FilterType FilterType { get; set; }
        }

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()  // Set the minimum log level to Debug for all sinks
                .WriteTo.Console()     // Add console sink
                .WriteTo.Async(a => a.File("logs/OireachtasAPI.txt", rollingInterval: RollingInterval.Hour))
                .CreateLogger();

            try
            {
                Log.Information("Starting application");

                if (args.Length == 0)
                {
                    Console.WriteLine("Parameters not provided, choose your parameters");
                    Log.Information("Parameters not provided");
                    int value = 0;
                    SourceType source = SourceType.Local;
                    while (value != 1 && value != 2)
                    {
                        Console.WriteLine($"Choose the data souce:\n 1 for {SourceType.Local}, 2 for {SourceType.Api}:");
                        string result = Console.ReadKey().KeyChar.ToString();
                        if (!int.TryParse(result, out value) || value != 1 && value != 2)
                        {
                            Console.WriteLine("\nInvalid choice.");
                            
                        }
                        else
                        {
                            source = (SourceType)value;
                        }
                    }

                    //Have to assign something.
                    FilterType filterType = FilterType.FilterBillsSponsoredBy;
                    value = 0;
                    while (value != 1 && value != 2)
                    {
                        Console.WriteLine($"\nChoose the filter.\n 1 for {FilterType.FilterBillsSponsoredBy}, 2 for {FilterType.FilterBillsLastUpdated}:");
                        string result = Console.ReadKey().KeyChar.ToString();
                        if (!int.TryParse(result, out value) || value != 1 && value != 2)
                        {
                            Console.WriteLine("\nInvalid choice.");
                        }
                        else
                        {
                            filterType = (FilterType)value;
                        }
                    }

                    OireachtasService service = InitialiseService(source);

                    if (filterType == FilterType.FilterBillsSponsoredBy)
                    {
                        string memberId = string.Empty;

                        while (memberId == string.Empty)
                        {
                            Console.WriteLine("\nPlease supply a Member Name to filter by. This cannot be blank");
                            memberId = Console.ReadLine();
                        }

                        //TODO Exception 
                        var result = service.FilterBillsSponsoredBy(memberId);
                        PrintResult(result);
                    }
                    else if (filterType == FilterType.FilterBillsLastUpdated)
                    {
                        //use maxvalue as null to avoid a temp DateTime value with tryparse.
                        DateTime dateFrom = DateTime.MaxValue;
                        while (dateFrom == DateTime.MaxValue)
                        {
                            Console.WriteLine("\nPlease supply a Date to filter from. Leaving this blank can result in Bills from the beginning of time being included.");
                            string date = Console.ReadLine();

                            if (!string.IsNullOrEmpty(date))
                            {
                                if (!DateTime.TryParse(date, out dateFrom))
                                {
                                    Console.WriteLine($"Invalid Date format. Provide date in format {cultureInfo.DateTimeFormat}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("DateFrom bypassed.");
                                dateFrom = DateTime.MinValue;
                            }
                        }

                        DateTime dateTo = DateTime.MaxValue;
                        while (dateTo == DateTime.MaxValue)
                        {
                            Console.WriteLine("\nPlease supply a Date to filter until. Leaving this blank will include bills up to todays date.");
                            string date = Console.ReadLine();

                            if (!string.IsNullOrEmpty(date))
                            {
                                if (!DateTime.TryParse(date, out dateTo))
                                {
                                    Console.WriteLine($"Invalid Date format. Provide date in format {cultureInfo.DateTimeFormat}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("DateTo bypassed.");
                                dateTo = DateTime.Today;
                            }
                        }

                        //TODO Exception 
                        var result = service.FilterBillsByLastUpdated(dateFrom, dateTo);
                        PrintResult(result);
                        Log.Information("Result Printed.");
                    }

                    Console.WriteLine("Parameters not provided, choose your parameters");
                }
                else
                {
                    Log.Information("Parameters Provided and validated at a basic level by Parser.");
                    Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(parameters =>
                    {
                        OireachtasService service = InitialiseService(parameters.SourceType);

                        if (parameters.FilterType == FilterType.FilterBillsSponsoredBy)
                        {
                            if (parameters.MemberId == null)
                            {
                                Log.Fatal("User failed to provide correct parameter for {FilterType.FilterBillsSponsoredBy}", FilterType.FilterBillsSponsoredBy);

                                throw new ArgumentException("Parameter must not be null or empty for FilterBillsSponsoredBy");
                            }
                            else
                            {
                                Console.WriteLine($"Filtering Bills by Sponsor: {parameters.MemberId}");
                                //TODO Exception 
                                List<dynamic> result = service.FilterBillsSponsoredBy(parameters.MemberId);
                                PrintResult(result);
                            }
                        }
                        else if (parameters.FilterType == FilterType.FilterBillsLastUpdated)
                        {
                            /*Make dateparameters robust by defaulting to extremes if explicit value not provided
                             * E.G no dateUntil will result in including everything up to current date
                             * */
                            if (!DateTime.TryParse(parameters.DateFrom, cultureInfo, DateTimeStyles.None, out DateTime dateFrom))
                            {
                                dateFrom = DateTime.MinValue;
                                Log.Warning($"{nameof(dateFrom)} parameter has been set to MinValue.");
                            }

                            if (!DateTime.TryParse(parameters.DateUntil, cultureInfo, DateTimeStyles.None, out DateTime dateUntil))
                            {
                                dateUntil = DateTime.Today;
                                Log.Warning($"{nameof(dateUntil)} parameter has been set to Today.");
                            }

                            //TODO Exception 
                            var result = service.FilterBillsByLastUpdated(dateFrom, dateUntil);
                            PrintResult(result);
                        }
                        else
                        {
                            // should not get here unless an additional enum is added to FilterTypes without updating this area.
                            throw new InvalidEnumArgumentException();
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
                }

                Console.WriteLine("Press any button to quit.");
                Console.ReadKey();

                void PrintResult(List<dynamic> toPrint)
                {
                    if (toPrint == null || toPrint.Count == 0)
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

                //Initialise the OireachtasService with dependencies based on if the source is api or local.
                OireachtasService InitialiseService(SourceType sourceType)
                {
                    Log.Information("Initialising Oireachtas Service.");
                    string legislationPath = sourceType == SourceType.Local ? Settings.DataPaths.LocalLegislation : Settings.DataPaths.ApiLegislation;
                    string membersPath = sourceType == SourceType.Local ? Settings.DataPaths.LocalMembers : Settings.DataPaths.ApiMembers;

                    ILogger logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Async(a => a.File("logs/OireachtasAPI.txt", rollingInterval: RollingInterval.Hour))
                        .CreateLogger();

                    IDataLoader dataLoader = new DataLoaderFactory(logger).GetLoader(legislationPath);
                    IOireachtasRepository repository = new OireachtasRepository(dataLoader, logger, legislationPath, membersPath);

                    return new OireachtasService(repository, logger);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occurred.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
