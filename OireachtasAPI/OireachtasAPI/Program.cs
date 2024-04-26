using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;
using OireachtasAPI.DataLoaders;
using OireachtasAPI.Enums;
using OireachtasAPI.Factories;
using OireachtasAPI.Repositories;
using OireachtasAPI.Services;
using OireachtasAPI.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime;
using System.Security.Cryptography;

public static class Program
{
    private static readonly CultureInfo Culture = new CultureInfo("en-IE");

    /// <summary>
    /// Options class encapsulates the parameters available to the user when executing the EXE with command line parameters or manually via the CLI.
    /// </summary>
    public class Options
    {
        [Option('s', "source", Required = false, HelpText = "Set the source for the data, 1 for Local and 2 for API.")]
        public SourceType SourceType { get; set; }

        [Option('m', "memberId", Required = false, HelpText = "Member ID to filter the bills.")]
        public string MemberId { get; set; }

        [Option('f', "dateFrom", Required = false, HelpText = "The date from which to filter the bills")]
        public DateTime? DateFrom { get; set; }

        [Option('u', "dateUntil", Required = false, HelpText = "The up to which the bills will be filtered.")]
        public DateTime? DateUntil { get; set; }

        [Option('t', "filterType", Required = false, HelpText = "The Type of filter requested. 1 for FilterBillsSponsoredBy, 2 for FilterBillsLastUpdated")]
        public FilterType FilterType { get; set; }

    }

    // Main entry point
    static void Main(string[] args)
    {
        ConfigureLogger();

        try
        {
            Log.Information("Starting application");

            Options options = ParseArguments(args);

            OireachtasService service = InitializeService(options.SourceType);

            // Perform action based on filter type
            ExecuteAction(service, options);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Log.Information("Ending application");
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

    /// <summary>
    /// Configures the logger.
    /// </summary>
    static void ConfigureLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.Async(a => a.File("logs/OireachtasAPI.txt", rollingInterval: RollingInterval.Hour))
            .CreateLogger();
    }

    /// <summary>
    /// Parses the command line arguements, automatically or manually.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    static Options ParseArguments(string[] args)
    {
        Options options;
        if (args.Length == 0)
        {
            options = ParseOptionsFromUser();
        }
        else
        {
            options = AutomaticallyParse(args);
        }
        return options;
    }

    /// <summary>
    /// Parses the options from user via a dialog.
    /// </summary>
    private static Options ParseOptionsFromUser()
    {
        Options options = new Options();
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
                options.SourceType = (SourceType)value;
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
                options.FilterType = (FilterType)value;
            }
        }

        if (options.FilterType == FilterType.FilterBillsSponsoredBy)
        {
            options.MemberId = string.Empty;

            while (options.MemberId == string.Empty)
            {
                Console.WriteLine("\nPlease supply a Member Name to filter by. This cannot be blank");
                options.MemberId = Console.ReadLine();
            }
        }
        else if (options.FilterType == FilterType.FilterBillsLastUpdated)
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
                        Console.WriteLine($"Invalid Date format. Provide date in format {Culture.DateTimeFormat}");
                    }else
                    {
                        options.DateFrom = dateFrom;
                    }
                }
                else
                {
                    Console.WriteLine("DateFrom bypassed.");
                    options.DateFrom = DateTime.MinValue;
                    dateFrom = options.DateFrom.Value;
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
                        Console.WriteLine($"Invalid Date format. Provide date in format {Culture.DateTimeFormat}");
                    }
                    else
                    {
                        options.DateUntil = dateTo;
                    }
                }
                else
                {
                    Console.WriteLine("DateTo bypassed.");
                    options.DateUntil = DateTime.Today;
                    dateTo = options.DateUntil.Value;
                }
            }
        }

        return options;
    }

    /// <summary>
    /// Parses from commandline parser with additional logic to manage mandatory parameters for particular scenarios.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <returns></returns>
    private static Options AutomaticallyParse(string[] args)
    {

        Options options;
        Log.Information("Parameters Provided and validated at a basic level by Parser.");
        options = new Parser(config =>
        {
            config.ParsingCulture = Culture;
        }).ParseArguments<Options>(args)
        .WithParsed(ops =>
        {
    
            if (ops.FilterType == FilterType.FilterBillsSponsoredBy)
            {
                if (ops.MemberId == null)
                {
                    Log.Fatal("User failed to provide correct parameter for {FilterType.FilterBillsSponsoredBy}", FilterType.FilterBillsSponsoredBy);

                    throw new ArgumentException("Parameter must not be null or empty for FilterBillsSponsoredBy");
                }
            }
            else if (ops.FilterType == FilterType.FilterBillsLastUpdated)
            {
                /*Make dateparameters robust by defaulting to extremes if explicit value not provided
                 * E.G no dateUntil will result in including everything up to current date
                 * */
                if (ops.DateFrom == null)
                {
                    ops.DateFrom = DateTime.MinValue;
                    Log.Warning("DateFrom parameter has been set to MinValue.");
                }

                if (ops.DateUntil == null)
                {
                    ops.DateUntil = DateTime.Today;
                    Log.Warning("DateFrom parameter has been set to Today.");
                }
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
        }).Value;
        return options;
    }

    /// <summary>
    /// Initialize OireachtasService based on source type.
    /// </summary>
    /// <param name="sourceType">Type of the source.</param>
    static OireachtasService InitializeService(SourceType sourceType)
    {
        Log.Information("Initializing Oireachtas Service.");

        string legislationPath = sourceType == SourceType.Local ? DataPaths.LocalLegislation : DataPaths.ApiLegislation;
        string membersPath = sourceType == SourceType.Local ? DataPaths.LocalMembers : DataPaths.ApiMembers;

        ILogger logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Async(a => a.File("logs/OireachtasAPI.txt", rollingInterval: RollingInterval.Hour))
            .CreateLogger();

        IDataLoader dataLoader = new DataLoaderFactory(logger).GetLoader(legislationPath);
        IOireachtasRepository repository = new OireachtasRepository(dataLoader, logger, legislationPath, membersPath);

        return new OireachtasService(repository, logger);
    }

    /// <summary>
    /// Execute action based on filter type
    /// </summary>
    /// <param name="service">The service.</param>
    /// <param name="options">The options.</param>
    static void ExecuteAction(OireachtasService service, Options options)
    {
        switch (options.FilterType)
        {
            case FilterType.FilterBillsSponsoredBy:
                if (string.IsNullOrWhiteSpace(options.MemberId))
                {
                    Log.Fatal("Member ID must not be null or empty for filtering bills sponsored by a member.");
                    Environment.Exit(1);
                }

                Log.Information("Filtering bills by sponsor: {MemberId}", options.MemberId);
                PrintResult(service.FilterBillsSponsoredBy(options.MemberId));
                break;

            case FilterType.FilterBillsLastUpdated:
                Log.Information("Filtering bills by last updated: {DateFrom} to {DateUntil}", options.DateFrom.Value, options.DateUntil.Value);
                PrintResult(service.FilterBillsByLastUpdated(options.DateFrom.Value, options.DateUntil.Value));
                break;

            default:
                Log.Fatal("Invalid filter type: {FilterType}", options.FilterType);
                Environment.Exit(1);
                break;
        }
    }

    /// <summary>
    ///  Print result to console
    /// </summary>
    /// <param name="result">The result.</param>
    static void PrintResult(List<dynamic> result)
    {
        if (result == null || result.Count == 0)
        {
            Log.Information("No results found.");
            return;
        }

        foreach (dynamic p in result)
        {
            string prettyJson = JsonConvert.SerializeObject(p, Formatting.Indented);
            Console.WriteLine(prettyJson);
        }
    }
}
