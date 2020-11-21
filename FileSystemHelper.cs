using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhotoSorter
{
	internal static class FileSystemHelper
	{
		public static IEnumerable<FileWithDate> CollectFilesWithinDirectory(string path)
		{
			var files = Directory.GetFiles(path);
			Logger.Information("Discovered {filesCount} files in directory '{path}'", files.Length, path);

			return files.Select(f => new FileWithDate(f));
		}

		public static bool CheckDirectoryExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Logger.Warning("Specified directory doesn't exist: '{path}'", path);
				return false;
			}

			return true;
		}

		public static void CreateDirectory(string path)
		{
			Logger.Information("Creating directory '{directoryPath}'", path);
			Directory.CreateDirectory(path);
		}

		public static bool TryGetFileName(string path, out string fileName)
		{
			try
			{
				fileName = Path.GetFileName(path);
				return true;
			}
			catch
			{
				fileName = string.Empty;
				return false;
			}
		}

		static readonly ILogger Logger = Log.ForContext<Program>();
	}
}
