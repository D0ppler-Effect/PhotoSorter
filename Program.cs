using System;
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

            // analyze files in source
            var collectedFiles = FileSystemHelper.CollectFilesWithinDirectory(options.Source).ToList();

            // view file statistics: files count for each concrete target directory

            // require user confirmation

            // actually MOVE files
        }
    }
}