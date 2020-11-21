using Serilog;
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
				Logger.Fatal("Could not get FileName from path '{path}'", FilePath);
				return false;
			}

			FileName = fileName;
			
			var regexMatch = new Regex(Regexp).Match(FileName);

			if (!regexMatch.Success)
			{
				Logger.Debug("Matching regular expression '{regexp}' on file '{fileName}' was unsuccessful", Regexp, fileName);

				return false;
			}

			try
			{
				Year = int.Parse(GetNamedRegexGroup(regexMatch, RegexpYearGroupName));
				Month = int.Parse(GetNamedRegexGroup(regexMatch, RegexpMonthGroupName));

				return true;
			}
			catch
			{
				Logger.Debug("Caught an error parsing regexp match '{match}'", regexMatch.Value);
				return false;
			}
		}

		private string GetNamedRegexGroup(Match regexMatch, string groupName)
		{
			if(regexMatch.Groups == null || regexMatch.Groups.Count == 0)
			{
				Logger.Error("Regular expression match '{regexpMatch}' doesn't contain any groups", regexMatch.Value);
			}

			if (!regexMatch.Groups.ContainsKey(groupName))
			{
				Logger.Debug("Regular expression match '{regexpMatch}' doesn't contain a group named '{groupName}'", regexMatch.Value, groupName);
				return null;
			}

			var matchGroup = regexMatch.Groups[groupName];
			if (!matchGroup.Success)
			{
				Logger.Debug("Regular expression match '{groupMatch}' for group '{groupName}' was unsuccessful", matchGroup.Value, groupName);
				return null;
			}

			return matchGroup.Value;
		}

		public int Year { get; private set; }

		public int Month { get; private set; }

		public string FileName { get; private set; }

		public string FilePath { get; }

		public bool IsParsed { get; }

		public static string Regexp = ConfigurationProvider.Configuration.FileNameRegexp;

		public static string RegexpYearGroupName = ConfigurationProvider.Configuration.YearGroupName;

		public static string RegexpMonthGroupName = ConfigurationProvider.Configuration.MonthGroupName;

		static readonly ILogger Logger = Log.ForContext<FileWithDate>();
	}
}
