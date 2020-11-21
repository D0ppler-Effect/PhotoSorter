using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using PhotoSorter.CommandLine;
using Serilog;

namespace PhotoSorter
{
	class Program
	{
		static void Main(string[] args)
		{
			ConfigureLogging();

			try
			{
				if (!ConfigurationProvider.Configuration.Validate())
				{
					Logger.Fatal("Application configuration is incorrect. Please fix it and re-run program.");
					return;
				}

				Parser.Default.ParseArguments<Options>(args).WithParsed(Process);
			}
			catch (Exception e)
			{
				Logger.Fatal(e, "Something went wrong!");
			}
		}

		static void Process(Options options)
		{
			// check source folder
			if (!FileSystemHelper.CheckDirectoryExists(options.Source))
			{
				Logger.Error("Source directory not found, exiting");
				return;
			}

			// check target root folder
			if (!FileSystemHelper.CheckDirectoryExists(options.Target))
			{
				Logger.Warning("Target root directory not found, creating");
				FileSystemHelper.CreateDirectory(options.Target);
			}

			// get files from source
			var collectedFiles = FileSystemHelper.CollectFilesWithinDirectory(options.Source).ToList();
			var parsedFiles = collectedFiles.Where(f => f.IsParsed).ToList();
			var unparsedFiles = collectedFiles.Where(f => !f.IsParsed).ToList();

			var discoveredGroups = DiscoverFileGroups(parsedFiles);

			// view file statistics: files count for each concrete target directory
			ViewFileStatistics(parsedFiles, unparsedFiles, discoveredGroups);

			// require user confirmation
			if (!RequestConfirmation(options))
			{
				return;
			}

			// actually MOVE files
			foreach (var group in discoveredGroups)
			{
				group.MoveFiles(options.Target);
			}
		}

		private static void ConfigureLogging()
		{
			const string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console(Serilog.Events.LogEventLevel.Information, outputTemplate: outputTemplate)
				.WriteTo.File("PhotoSorter.log", Serilog.Events.LogEventLevel.Verbose, outputTemplate: outputTemplate, rollingInterval: RollingInterval.Month)
				.CreateLogger();
		}

		private static bool RequestConfirmation(Options options)
		{
			Console.WriteLine($"Type 'Y' to proceed with copying files into '{options.Target}\\YYYY\\MM' folders.");
			var decision = Console.ReadLine();

			if (decision.ToUpper() != "Y")
			{
				Console.WriteLine("Ok. See you later, bye!");

				Logger.Debug("Further processing was cancelled by user");
				return false;
			}

			Logger.Debug("Got user confirmation on further file processing");
			return true;
		}

		private static void ViewFileStatistics(List<FileWithDate> parsedFiles, List<FileWithDate> unparsedFiles, List<FilesGroup> discoveredGroups)
		{
			Logger.Information("Successfully parsed {parsedFilesCount} files, failed to parse {unparsedFilesCount} files.", parsedFiles.Count, unparsedFiles.Count);

			Logger.Information("Discovered the following file groups:");
			foreach (var group in discoveredGroups)
			{
				Logger.Information("{groupYear}.{groupMonth:D2}, files: {groupFilesCount}", group.Year, group.Month, group.Files.Count);
			}
		}

		static List<FilesGroup> DiscoverFileGroups(IEnumerable<FileWithDate> files)
		{
			var result = new List<FilesGroup>();
			var years = files.Select(f => f.Year).Distinct();

			foreach (var year in years)
			{
				var filesWithinYear = files.Where(f => f.Year == year);
				var months = filesWithinYear.Select(f => f.Month).Distinct();

				result.AddRange(months
					.Select(m => new FilesGroup(year, m, filesWithinYear.Where(f => f.Month == m)))
					.OrderBy(g => g.Month));
			}

			return result;
		}

		static readonly ILogger Logger = Log.ForContext<Program>();
	}
}