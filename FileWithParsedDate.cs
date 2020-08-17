using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PhotoSorter
{
	internal class FileWithParsedDate
	{
		public FileWithParsedDate(string path)
		{
			FilePath = path;
			FileName = Path.GetFileName(FilePath);

			var rex = new Regex(Regexp);
			var foo = rex.Match(FileName);
		}

		public int Year { get; }

		public int Month { get; }

		public string FileName { get; }

		public string FilePath { get; }

		public const string Regexp = @"(?<year>\d{4})-(?<month>\d{2})";
	}
}
