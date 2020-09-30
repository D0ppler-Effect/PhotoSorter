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
			var targetFolderPath = Path.Combine(targetRootFolder, Year.ToString(), Month.ToString());
			if (!FileSystemHelper.CheckDirectoryExists(targetFolderPath))
			{
				FileSystemHelper.CreateDirectory(targetFolderPath);
			}

			var counter = 1;
			foreach(var file in Files)
			{
				Console.WriteLine($"Copying file {counter++} of {Files.Count}");

				var targetFilePath = Path.Combine(targetFolderPath, file.FileName);
				File.Copy(file.FilePath, targetFilePath);
			}
		}

		public List<FileWithDate> Files { get; }

		public int Year { get; }

		public int Month { get; }
	}
}
