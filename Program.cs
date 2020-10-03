using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using PhotoSorter.CommandLine;

namespace PhotoSorter
{
    class Program
    {
        static void Main(string[] args)
        {
			if (!ConfigurationProvider.Configuration.Validate())
			{
				Console.WriteLine("Application configuration is incorrect. Please fix it and re-run program.");
                return;
			}

            Parser.Default.ParseArguments<Options>(args).WithParsed(Process);
        }

        static void Process(Options options)
		{
			// check source folder
			if (!FileSystemHelper.CheckDirectoryExists(options.Source))
			{
				Console.WriteLine("Source directory not found, exiting");
				return;
			}

			// check target root folder
			if (!FileSystemHelper.CheckDirectoryExists(options.Target))
			{
				Console.WriteLine("Target root directory not found, creating");
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

		private static bool RequestConfirmation(Options options)
		{
			Console.WriteLine($"Type 'Y' to proceed with copying files into '{options.Target}\\YYYY\\MM' folders.");
			var decision = Console.ReadLine();

			if (decision.ToUpper() != "Y")
			{
				Console.WriteLine("Ok. See you later, bye!");
				return false;
			}

			return true;
		}

		private static void ViewFileStatistics(List<FileWithDate> parsedFiles, List<FileWithDate> unparsedFiles, List<FilesGroup> discoveredGroups)
		{
			Console.WriteLine($"Successfully parsed {parsedFiles.Count} of them, failed to parse {unparsedFiles.Count}.");
			if (unparsedFiles.Count > 0)
			{
				Console.WriteLine("The following files could not be parsed: ");
				foreach (var file in unparsedFiles)
				{
					Console.WriteLine(file.FileName);
				}
			}

			Console.WriteLine("Discovered the following file groups:");
			foreach (var group in discoveredGroups)
			{
				Console.WriteLine($"Year: {group.Year}, month: {group.Month}, files: {group.Files.Count}");
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
    }
}