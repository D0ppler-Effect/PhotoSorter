using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhotoSorter
{
	internal static class FileSystemHelper
	{
		public static IEnumerable<FileWithParsedDate> CollectFilesWithinDirectory(string path)
		{
			var files = Directory.GetFiles(path);
			Console.WriteLine($"Discovered {files.Length} files in directory '{path}'");

			return files.Select(f => new FileWithParsedDate(f));
		}

		public static bool CheckDirectoryExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Console.WriteLine($"Specified directory doesn't exist: '{path}'");
				return false;
			}

			return true;
		}

		public static void CreateDirectory(string path)
		{
			Directory.CreateDirectory(path);
		}
	}
}
