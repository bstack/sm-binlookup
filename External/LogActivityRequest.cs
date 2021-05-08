using System;


namespace binlookup.External
{
    public class LogActivityRequest
    {
        public string Service { get; }
        public string Activity { get; }
        public string ActivityDetail { get; }


        public LogActivityRequest(
            string activity,
            string activityDetail)
        {
            this.Service = "binlookup";
            this.Activity = activity;
            this.ActivityDetail = activityDetail;
        } 
    }
}
