using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace PhotoSorter
{
	internal class FileWithDate
	{
		public FileWithDate(string path)
		{
			FilePath = path;

			IsParsed = TryParse();
		}

		private bool TryParse()
		{
			if(!FileSystemHelper.TryGetFileName(FilePath, out var fileName))
			{
				return false;
			}

			FileName = fileName;
			
			var regexMatch = new Regex(Regexp).Match(FileName);

			if (!regexMatch.Success)
			{
				return false;
			}

			try
			{
				Year = int.Parse(GetNamedRegexGroup(regexMatch, "year"));
				Month = int.Parse(GetNamedRegexGroup(regexMatch, "month"));

				return true;
			}
			catch
			{
				return false;
			}
		}

		private string GetNamedRegexGroup(Match regexMatch, string groupName)
		{
			if (!regexMatch.Groups.ContainsKey(groupName))
			{
				return null;
			}

			var matchGroup = regexMatch.Groups[groupName];
			if (!matchGroup.Success)
			{
				return null;
			}

			return matchGroup.Value;
		}

		public int Year { get; private set; }

		public int Month { get; private set; }

		public string FileName { get; private set; }

		public string FilePath { get; }

		public bool IsParsed { get; }

		public string Regexp = $@"(?<year>\d\d\d\d)-(?<month>\d\d)-[0-3]\d .*\.";
	}
}
