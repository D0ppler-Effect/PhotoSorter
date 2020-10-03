using System;

namespace PhotoSorter
{
	public class Configuration
	{
		public string FileNameRegexp { get; set; }

		public string YearGroupName { get; set; }

		public string MonthGroupName { get; set; }

		public bool Validate()
		{
			var result = true;

			if (string.IsNullOrWhiteSpace(FileNameRegexp))
			{
				Console.WriteLine("FileNameRegexp is empty!");
				result = false;
			}

			if (string.IsNullOrWhiteSpace(YearGroupName))
			{
				Console.WriteLine("YearGroupName is empty!");
				result = false;
			}

			if (string.IsNullOrWhiteSpace(MonthGroupName))
			{
				Console.WriteLine("MonthGroupName is empty!");
				result = false;
			}

			return result;
		}
	}
}
