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
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed(Process);
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
                Console.WriteLine("Target root directory not found, exiting");
                return;
            }

            // get files from source
            var collectedFiles = FileSystemHelper.CollectFilesWithinDirectory(options.Source).ToList();
            var parsedFiles = collectedFiles.Where(f => f.IsParsed).ToList();
            var unparsedFiles = collectedFiles.Where(f => !f.IsParsed).ToList();

			Console.WriteLine($"Successfully parsed {parsedFiles.Count} of them, failed to parse {unparsedFiles.Count}.");
            if(unparsedFiles.Count > 0)
			{
				Console.WriteLine("The following files could not be parsed: ");
                foreach(var file in unparsedFiles)
				{
					Console.WriteLine(file.FileName);
				}
			}

            // view file statistics: files count for each concrete target directory
            var discoveredGroups = DiscoverFileGroups(parsedFiles);

			Console.WriteLine("Discovered the following file groups:");
            foreach(var group in discoveredGroups)
			{
                Console.WriteLine($"Year: {group.Year}, month: {group.Month}, files: {group.Files.Count}");
			}

			// require user confirmation
			Console.WriteLine($"Type 'Y' to proceed with copying files into '{options.Target}\\YYYY\\MM' folders.");
            var decision = Console.ReadLine();

            if(decision.ToUpper() != "Y")
			{
				Console.WriteLine("Ok. See you later, bye!");
                return;
			}

            // actually MOVE files
            foreach(var group in discoveredGroups)
			{
                group.MoveFiles(options.Target);
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