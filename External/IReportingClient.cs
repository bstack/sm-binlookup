using System;


namespace binlookup.External
{
	public interface IReportingClient
    {
        void LogActivity(
            string requestId,
            string correlationId,
            string activity,
            string activityDetail);
    }
}