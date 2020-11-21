using Serilog;
using System;

namespace PhotoSorter
{
	public class Configuration
	{
		public string SourceDirectory { get; set; }

		public string TargetRootDirectory { get; set; }

		public string FileNameRegexp { get; set; }

		public string YearGroupName { get; set; }

		public string MonthGroupName { get; set; }

		public bool Validate()
		{
			Logger.Information("Checking parsed configuration...");

			var result = true;

			if (string.IsNullOrWhiteSpace(FileNameRegexp))
			{
				Logger.Fatal("FileNameRegexp is empty!");
				result = false;
			}

			if (string.IsNullOrWhiteSpace(YearGroupName))
			{
				Logger.Fatal("YearGroupName is empty!");
				result = false;
			}

			if (string.IsNullOrWhiteSpace(MonthGroupName))
			{
				Logger.Fatal("MonthGroupName is empty!");
				result = false;
			}

			if (string.IsNullOrWhiteSpace(SourceDirectory))
			{
				Logger.Fatal("SourceDirectory is empty!");
				result = false;
			}

			if (string.IsNullOrWhiteSpace(TargetRootDirectory))
			{
				Logger.Fatal("TargetRootDirectory is empty!");
				result = false;
			}

			return result;
		}

		static readonly ILogger Logger = Log.ForContext<Configuration>();
	}
}
