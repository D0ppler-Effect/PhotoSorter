using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoSorter
{
	internal class FilesGroup
	{
		public FilesGroup(int year, int month, IEnumerable<FileWithDate> files)
		{
			Year = year;

			Month = month;

			Files = files.ToList();
		}

		public void MoveFiles(string targetRootFolder)
		{
			var targetFolderPath = Path.Combine(targetRootFolder, FormattedYear, FormattedMonth);
			if (!FileSystemHelper.CheckDirectoryExists(targetFolderPath))
			{
				FileSystemHelper.CreateDirectory(targetFolderPath);
			}

			var counter = 1;
			foreach(var file in Files)
			{
				Logger.Information("Moving file {current} of {total}: '{filename}'", counter++, Files.Count, file.FileName);

				var targetFilePath = Path.Combine(targetFolderPath, file.FileName);
				MoveFileWithCheck(file.FilePath, targetFilePath);
			}
		}

		private void MoveFileWithCheck(string sourcePath, string destinationPath)
		{
			if (File.Exists(destinationPath))
			{
				Logger.Error("Destination file '{destinationPath}' already exists! Source file '{sourcePath}' will not be moved! Please check both files and resolve conflict manually!",
					destinationPath,
					sourcePath);

				return;
			}

			try
			{
				File.Move(sourcePath, destinationPath, false);
			}
			catch (Exception e)
			{
				Logger.Fatal(e, "Error moving file '{sourcePath}' to '{destinationPath}'", sourcePath, destinationPath);				
			}
		}

		public List<FileWithDate> Files { get; }

		public int Year { get; }

		public int Month { get; }

		public string FormattedMonth => Month.ToString("D2");

		public string FormattedYear => Year.ToString("D4");

		ILogger Logger = Log.ForContext<FilesGroup>();
	}
}
