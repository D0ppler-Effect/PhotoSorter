using Microsoft.Extensions.Configuration;

namespace PhotoSorter
{
	public static class ConfigurationProvider
	{
		public static Configuration Configuration
		{
			get
			{
				if (_configuration == null)
				{
					lock (LockToken)
					{
						if (_configuration == null)
						{
							var config = new ConfigurationBuilder().AddJsonFile(ConfigFileName).Build();
							_configuration = config.Get<Configuration>();
						}
					}
				}

				return _configuration;
			}
		}

		private static Configuration _configuration;

		private static readonly object LockToken = new object();

		public const string ConfigFileName = "appsettings.json";
	}
}
