using System;

namespace binlookup
{
	public class AppSettings
	{
		public string DatabaseConnectionString { get; private set; }
		public string ReportingRequestUri { get; private set; }
	}
}